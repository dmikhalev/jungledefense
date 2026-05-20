using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Economy")]
    public int cost = 50;

    [Header("Combat")]
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 1;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Upgrade")]
    public int level = 1;
    public int maxLevel = 3;
    public int upgradeCost = 50;

    public int damageIncrease = 1;
    public float rangeIncrease = 0.5f;
    public float fireRateIncrease = 0.3f;

    private float fireCooldown;
    private Enemy target;
    private RangeCircleRenderer rangeCircle;

    public int Level => level;
    public bool IsMaxLevel => level >= maxLevel;

    private void Update()
    {
        FindTarget();

        if (target == null)
        {
            return;
        }

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private void FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < shortestDistance && distance <= range)
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
            Debug.LogError("Projectile prefab is missing");
            return;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError("Projectile component missing");
            Destroy(projectileObject);
            return;
        }

        projectile.damage = damage;
        projectile.SetTarget(target.transform);
    }


    public void ShowRange()
    {
        if (rangeCircle == null)
        {
            GameObject rangeObject = new GameObject("RangeCircle");
            rangeObject.transform.SetParent(transform);
            rangeObject.transform.localPosition = Vector3.zero;

            rangeCircle = rangeObject.AddComponent<RangeCircleRenderer>();
        }

        rangeCircle.gameObject.SetActive(true);
        rangeCircle.Draw(range);
    }

    public void HideRange()
    {
        if (rangeCircle != null)
        {
            rangeCircle.gameObject.SetActive(false);
        }
    }

    public bool UpgradeTower()
    {
        if (IsMaxLevel)
        {
            Debug.Log("Tower already max level");
            return false;
        }

        if (!GameManager.Instance.SpendMoney(upgradeCost))
        {
            Debug.Log("Not enough money");
            return false;
        }

        level++;

        damage += damageIncrease;
        range += rangeIncrease;
        fireRate += fireRateIncrease;

        upgradeCost += 50;

        ShowRange();

        Debug.Log("Tower upgraded to level " + level);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}