using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private EnemyHitFlash hitFlash;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentHealth;
    private int waypointIndex;
    private bool isDead;
    private float stunTimer;
    private List<Transform> waypoints;
    private EnemyPool pool;
    private GameObject sourcePrefab;
    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalRendererColors;
    private Sprite originalSprite;
    private Vector3 originalScale;

    public Action OnRemoved;

    public bool IsAlive => !isDead;
    public int CurrentWaypointIndex => waypointIndex;

    public float DistanceToCurrentWaypointSqr
    {
        get
        {
            if (waypoints == null ||
                waypointIndex < 0 ||
                waypointIndex >= waypoints.Count ||
                waypoints[waypointIndex] == null)
            {
                return Mathf.Infinity;
            }

            return (waypoints[waypointIndex].position - transform.position).sqrMagnitude;
        }
    }

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>(true);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalRendererColors = new Color[spriteRenderers.Length];
        originalScale = transform.localScale;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null && spriteRenderers.Length > 0)
            {
                spriteRenderer = spriteRenderers[0];
            }
        }

        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalRendererColors[i] = spriteRenderers[i].color;
        }
    }

    private void OnEnable()
    {
        EnemyRegistry.Register(this);
    }

    private void OnDisable()
    {
        EnemyRegistry.Unregister(this);
        OnRemoved = null;
    }

    public void SetPool(EnemyPool ownerPool, GameObject prefab)
    {
        pool = ownerPool;
        sourcePrefab = prefab;
    }

    public void InitializeForSpawn(List<Transform> path)
    {
        if (data == null)
        {
            Debug.LogError($"{name} has no EnemyData assigned.");
            enabled = false;
            return;
        }

        enabled = true;
        waypoints = path;
        waypointIndex = 0;
        isDead = false;
        stunTimer = 0f;
        currentHealth = data.maxHealth;

        ResetVisualState();
        ApplyDataVisuals();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, data.maxHealth);
        }
    }

    public void SetPath(List<Transform> path)
    {
        InitializeForSpawn(path);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        MoveAlongPath();
    }

    public void Stun(float duration)
    {
        if (duration <= 0f || isDead)
        {
            return;
        }

        float adjustedDuration = data != null
            ? data.GetAdjustedStunDuration(duration)
            : duration;

        if (adjustedDuration <= 0f)
        {
            return;
        }

        stunTimer = Mathf.Max(stunTimer, adjustedDuration);
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, DamageType.Direct);
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        if (isDead)
        {
            return;
        }

        float finalDamage = damage;

        if (data != null)
        {
            finalDamage *= data.GetDamageMultiplier(damageType);
        }

        currentHealth -= finalDamage;

        if (DamageTextSpawner.Instance != null)
        {
            DamageTextSpawner.Instance.Spawn(transform.position, Mathf.RoundToInt(finalDamage));
        }

        if (hitFlash != null)
        {
            hitFlash.Flash();
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, data.maxHealth);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void DespawnImmediately()
    {
        if (isDead)
        {
            ReleaseToPool();
            return;
        }

        isDead = true;
        NotifyRemoved();
        ReleaseToPool();
    }

    private void MoveAlongPath()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count)
        {
            return;
        }

        Transform waypoint = waypoints[waypointIndex];

        if (waypoint == null)
        {
            waypointIndex++;
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = waypoint.position;
        Vector3 direction = targetPosition - currentPosition;

        RotateToDirection(direction);

        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            data.speed * Time.deltaTime
        );

        if ((transform.position - targetPosition).sqrMagnitude <= 0.01f)
        {
            waypointIndex++;
        }

        if (waypointIndex >= waypoints.Count)
        {
            ReachEnd();
        }
    }

    private void RotateToDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Sprite смотрит вниз, поэтому добавляем 90 градусов
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        int reward = data != null ? data.reward : 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(reward);
        }

        EventBus.Raise(new EnemyKilledEvent(this, reward));

        SpawnDeathEffect();
        RemoveEnemyWithFeedback();
    }

    private void ReachEnd()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        int damageToBase = data != null ? data.damageToBase : 1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife(damageToBase);
        }

        EventBus.Raise(new EnemyReachedBaseEvent(this, damageToBase));

        RemoveEnemy();
    }

    private void SpawnDeathEffect()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
    }

    private void RemoveEnemy()
    {
        NotifyRemoved();
        ReleaseToPool();
    }

    private void RemoveEnemyWithFeedback()
    {
        NotifyRemoved();

        if (healthBar != null)
        {
            healthBar.Hide();
        }

        EnemyDeathFeedback deathFeedback = GetComponent<EnemyDeathFeedback>();

        if (deathFeedback == null)
        {
            deathFeedback = gameObject.AddComponent<EnemyDeathFeedback>();
        }

        deathFeedback.Play(ReleaseToPool);
    }

    private void NotifyRemoved()
    {
        Action removed = OnRemoved;
        OnRemoved = null;
        removed?.Invoke();
    }

    private void ReleaseToPool()
    {
        waypoints = null;
        stunTimer = 0f;

        if (pool != null && sourcePrefab != null)
        {
            pool.Release(sourcePrefab, this);
            return;
        }

        Destroy(gameObject);
    }

    private void ApplyDataVisuals()
    {
        if (spriteRenderer != null && data != null && data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
        }
    }

    private void ResetVisualState()
    {
        transform.localScale = originalScale;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = true;
            }
        }

        if (spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = originalRendererColors[i];
            }
        }
    }
}
