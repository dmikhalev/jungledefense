using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}