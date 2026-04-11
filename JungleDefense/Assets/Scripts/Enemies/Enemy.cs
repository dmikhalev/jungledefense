using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int health = 5;

    private int waypointIndex = 0;
    private Transform[] waypoints;

    public void SetPath(Transform[] points)
    {
        waypoints = points;
    }

    void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Length)
            return;

        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            waypointIndex++;
        }

        if (waypointIndex >= waypoints.Length)
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
        GameManager.Instance.AddMoney(10);
        Destroy(gameObject);
    }

    void ReachEnd()
    {
        GameManager.Instance.LoseLife(1);
        Destroy(gameObject);
    }
}