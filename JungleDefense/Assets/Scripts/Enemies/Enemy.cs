using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int health = 5;
    public int reward = 10;

    private int waypointIndex = 0;
    private List<Transform> waypoints;

    public void SetPath(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        waypointIndex = 0;
    }

    void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count)
            return;

        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            waypointIndex++;
        }

        if (waypointIndex >= waypoints.Count)
        {
            ReachEnd();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.AddMoney(reward);
        Destroy(gameObject);
        GameManager.Instance.AddScore(10); // Добавляем очки за убитого врага
    }

    void ReachEnd()
    {
        GameManager.Instance.LoseLife(1);
        Destroy(gameObject);
    }
}