using UnityEngine;

public class Tower : MonoBehaviour
{
    private Enemy target;

    public GameObject projectilePrefab;

    private float fireCountdown = 0f;

    public int damage = 1;   // Урон
    public float fireRate = 1f;  // Скорость стрельбы
    public float range = 5f;  // Радиус

    void Update()
    {
        FindTarget();

        if (target == null)
            return;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void FindTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            // Выбираем врага, который ближе всего к выходу
            if (distance < shortestDistance && distance <= range)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Projectile proj = projectileGO.GetComponent<Projectile>();
        proj.SetTarget(target.transform);
    }

    public void UpgradeTower()
    {
        damage += 2;
        fireRate += 0.5f;
        range += 1f;
    }
}