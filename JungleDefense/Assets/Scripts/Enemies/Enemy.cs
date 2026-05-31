using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private EnemyHitFlash hitFlash;

    private float currentHealth;
    private int waypointIndex;
    private bool isDead;
    private float stunTimer;
    private List<Transform> waypoints;

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

    public void SetPath(List<Transform> path)
    {
        waypoints = path;
        waypointIndex = 0;
    }

    private void OnEnable()
    {
        EnemyRegistry.Register(this);
    }

    private void OnDestroy()
    {
        EnemyRegistry.Unregister(this);
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError($"{name} has no EnemyData assigned.");
            enabled = false;
            return;
        }

        currentHealth = data.maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, data.maxHealth);
        }

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

        stunTimer = Mathf.Max(stunTimer, duration);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (DamageTextSpawner.Instance != null)
        {
            DamageTextSpawner.Instance.Spawn(transform.position, Mathf.RoundToInt(damage));
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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(data.reward);
        }
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
            GameManager.Instance.LoseLife(1);
        }
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
        OnRemoved?.Invoke();
        Destroy(gameObject);
    }

    private void RemoveEnemyWithFeedback()
    {
        OnRemoved?.Invoke();

        if (healthBar != null)
        {
            healthBar.Hide();
        }

        EnemyDeathFeedback deathFeedback = GetComponent<EnemyDeathFeedback>();

        if (deathFeedback == null)
        {
            deathFeedback = gameObject.AddComponent<EnemyDeathFeedback>();
        }

        deathFeedback.Play();
    }
}
