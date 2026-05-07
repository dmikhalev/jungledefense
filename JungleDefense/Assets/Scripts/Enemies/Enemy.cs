using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float currentHealth;
    public int reward = 10;
    public EnemyData data;
    public GameObject deathEffect;

    private int waypointIndex = 0;
    private List<Transform> waypoints;

    public System.Action onDeath;

    public void SetPath(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        waypointIndex = 0;
    }

    void Start()
    {
        currentHealth = data.maxHealth;

        GetComponent<SpriteRenderer>().color = data.color;
    }

    void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count)
            return;

        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * data.speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            waypointIndex++;
        }

        if (waypointIndex >= waypoints.Count)
        {
            ReachEnd();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.AddMoney(data.reward);

        onDeath?.Invoke();

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void ReachEnd()
    {
        GameManager.Instance.LoseLife(1);
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}