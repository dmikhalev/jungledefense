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
    private float healthMultiplier = 1f;
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
    private bool bossUiActive;
    private float currentSpeed;
    private float regenerationTimer;
    private bool[] triggeredRageThresholds;

    public Action OnRemoved;

    public bool IsAlive => !isDead;
    public EnemyData Data => data;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => data != null ? data.maxHealth : 0f;
    public bool IsBoss => data != null && data.enemyType == EnemyType.Boss;
    public string DisplayName => data != null && !string.IsNullOrWhiteSpace(data.enemyName)
        ? data.enemyName
        : name;
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

    public void InitializeForSpawn(List<Transform> path, float healthMultiplier = 1f)
    {
        if (data == null)
        {
            Debug.LogError($"{name} has no EnemyData assigned.");
            enabled = false;
            return;
        }

        this.healthMultiplier = healthMultiplier;
        enabled = true;
        waypoints = path;
        waypointIndex = 0;
        isDead = false;
        stunTimer = 0f;
        bossUiActive = false;
        currentHealth = data.maxHealth * healthMultiplier;
        currentSpeed = data.speed;
        regenerationTimer = 0f;
        InitializeBossAbilityRuntimeState();

        ResetVisualState();
        ApplyDataVisuals();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, data.maxHealth);
        }

        ShowBossUIIfNeeded();
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

        UpdateBossAbilities();
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

        currentHealth = Mathf.Max(0f, currentHealth - finalDamage);

        UpdateBossRageIfNeeded();

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

        RaiseBossHealthChangedIfNeeded();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void DespawnImmediately()
    {
        if (isDead)
        {
            HideBossUIIfNeeded();
            ReleaseToPool();
            return;
        }

        isDead = true;
        HideBossUIIfNeeded();
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
            currentSpeed * Time.deltaTime
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

        if (IsBoss && CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.25f, 0.18f);
        }

        EventBus.Raise(new EnemyKilledEvent(this, reward));
        HideBossUIIfNeeded();

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

        if (GameManager.Instance != null)
        {
            if (IsBoss)
            {
                GameManager.Instance.LoseLife(GameManager.Instance.lives);
            }
            else
            {
                int damageToBase = data != null ? data.damageToBase : 1;
                GameManager.Instance.LoseLife(damageToBase);
            }
        }

        EventBus.Raise(
            new EnemyReachedBaseEvent(
                this,
                IsBoss
                    ? 999
                    : (data != null ? data.damageToBase : 1)
            )
        );

        HideBossUIIfNeeded();

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
        regenerationTimer = 0f;
        currentSpeed = data != null ? data.speed : currentSpeed;

        if (pool != null && sourcePrefab != null)
        {
            pool.Release(sourcePrefab, this);
            return;
        }

        Destroy(gameObject);
    }


    private void InitializeBossAbilityRuntimeState()
    {
        if (!IsBoss || data == null)
        {
            triggeredRageThresholds = null;
            return;
        }

        if (data.bossAbility == BossAbilityType.Rage &&
            data.rageHealthThresholds != null)
        {
            triggeredRageThresholds = new bool[data.rageHealthThresholds.Length];
        }
        else
        {
            triggeredRageThresholds = null;
        }
    }

    private void UpdateBossAbilities()
    {
        if (!IsBoss || data == null || isDead)
        {
            return;
        }

        if (data.bossAbility == BossAbilityType.Regeneration)
        {
            UpdateBossRegeneration();
        }
    }

    private void UpdateBossRegeneration()
    {
        if (currentHealth <= 0f || currentHealth >= data.maxHealth)
        {
            regenerationTimer = 0f;
            return;
        }

        regenerationTimer += Time.deltaTime;

        if (regenerationTimer < data.regenerationInterval)
        {
            return;
        }

        regenerationTimer = 0f;

        float healAmount = data.maxHealth * data.regenerationPercentOfMaxHealth;

        if (healAmount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(data.maxHealth, currentHealth + healAmount);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, data.maxHealth);
        }

        RaiseBossHealthChangedIfNeeded();
    }

    private void UpdateBossRageIfNeeded()
    {
        if (!IsBoss ||
            data == null ||
            data.bossAbility != BossAbilityType.Rage ||
            data.rageHealthThresholds == null ||
            triggeredRageThresholds == null)
        {
            return;
        }

        float healthPercent = data.maxHealth <= 0f
            ? 0f
            : currentHealth / data.maxHealth;

        for (int i = 0; i < data.rageHealthThresholds.Length; i++)
        {
            if (triggeredRageThresholds[i])
            {
                continue;
            }

            if (healthPercent > data.rageHealthThresholds[i])
            {
                continue;
            }

            triggeredRageThresholds[i] = true;
            currentSpeed *= data.rageSpeedMultiplier;

            if (hitFlash != null)
            {
                hitFlash.Flash();
            }
        }
    }

    private void ShowBossUIIfNeeded()
    {
        if (!IsBoss || bossUiActive)
        {
            return;
        }

        bossUiActive = true;

        EventBus.Raise(new BossSpawnedEvent(
            this,
            DisplayName,
            currentHealth,
            MaxHealth
        ));
    }

    private void RaiseBossHealthChangedIfNeeded()
    {
        if (!bossUiActive)
        {
            return;
        }

        EventBus.Raise(new BossHealthChangedEvent(
            this,
            currentHealth,
            MaxHealth
        ));
    }

    private void HideBossUIIfNeeded()
    {
        if (!bossUiActive)
        {
            return;
        }

        bossUiActive = false;
        EventBus.Raise(new BossRemovedEvent(this));
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
