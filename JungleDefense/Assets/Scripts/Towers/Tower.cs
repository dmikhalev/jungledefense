using UnityEngine;

public class Tower : MonoBehaviour
{
    private Enemy target;

    public GameObject projectilePrefab;

    private float fireCountdown = 0f;

    public int damage = 1;   // ����
    public float fireRate = 1f;  // �������� ��������
    public float range = 5f;  // ������

    public int level = 1;
    public int maxLevel = 3;

    public int upgradeCost = 50;

    public int damageIncrease = 1;
    public float rangeIncrease = 0.5f;
    public float fireRateIncrease = 0.3f;


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

            // �������� �����, ������� ����� ����� � ������
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
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError("Projectile component is missing on projectile prefab");
            Destroy(projectileObject);
            return;
        }

        projectile.damage = damage;
        projectile.SetTarget(target.transform);
    }

    public bool UpgradeTower()
    {
        if (level >= maxLevel)
        {
            Debug.Log("Башня уже максимального уровня");
            return false;
        }

        if (!GameManager.Instance.SpendMoney(upgradeCost))
        {
            Debug.Log("Недостаточно денег для улучшения");
            return false;
        }

        level++;

        damage += damageIncrease;
        range += rangeIncrease;
        fireRate += fireRateIncrease;

        upgradeCost += 50;

        Debug.Log("Башня улучшена до уровня: " + level);

        return true;
    }
}