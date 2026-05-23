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
    private List<Transform> waypoints;

    public Action OnRemoved;

    public bool IsAlive => !isDead;

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

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = data.color;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        MoveAlongPath();
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

        Transform target = waypoints[waypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        RotateToDirection(direction);

        transform.position += direction * data.speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
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

        GameManager.Instance.AddMoney(data.reward);
        SpawnDeathEffect();
        RemoveEnemy();
    }

    private void ReachEnd()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        GameManager.Instance.LoseLife(1);
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
}
