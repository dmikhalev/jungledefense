using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int damage = 1;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float range = 5f;

    [Header("Upgrade")]
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private int upgradeCost = 50;
    [SerializeField] private int upgradeCostIncrease = 50;
    [SerializeField] private int damageIncrease = 1;
    [SerializeField] private float rangeIncrease = 0.5f;
    [SerializeField] private float fireRateIncrease = 0.3f;

    private Enemy target;
    private float fireCountdown;

    public int Level => level;
    public int MaxLevel => maxLevel;
    public int UpgradeCost => upgradeCost;
    public bool IsMaxLevel => level >= maxLevel;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        FindTarget();

        if (target == null)
        {
            return;
        }

        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }

    private void FindTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= range && distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy;
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{name} has no projectile prefab assigned.");
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError("Projectile prefab does not have Projectile component.");
            Destroy(projectileObject);
            return;
        }

        projectile.damage = damage;
        projectile.SetTarget(target.transform);
    }

    public bool UpgradeTower()
    {
        if (IsMaxLevel)
        {
            Debug.Log("Tower is already at max level.");
            return false;
        }

        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(upgradeCost))
        {
            Debug.Log("Not enough money to upgrade tower.");
            return false;
        }

        level++;
        damage += damageIncrease;
        range += rangeIncrease;
        fireRate += fireRateIncrease;
        upgradeCost += upgradeCostIncrease;

        Debug.Log($"Tower upgraded to level {level}.");

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
