using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Economy")]
    public int cost = 50;

    [Header("Combat")]
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 1;
    [SerializeField] private float targetRefreshInterval = 0.15f;

    [SerializeField] private TowerShootFeedback shootFeedback;
    [SerializeField] private Transform rotatingPart;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("UI")]
    public Sprite icon;

    [Header("Info")]
    public string towerName = "Tower";

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
    private Tile occupiedTile;

    public int Level => level;
    public bool IsMaxLevel => level >= maxLevel;
    public int SellRefund => cost / 2;

    private void Update()
    {
        TickCooldown();

        FindTarget();

        if (target == null || fireCooldown > 0f)
        {
            return;
        }

        Shoot();

        fireCooldown = GetFireInterval();
    }

    private void TickCooldown()
    {
        if (fireCooldown <= 0f)
        {
            return;
        }

        fireCooldown = Mathf.Max(0f, fireCooldown - Time.deltaTime);
    }

    private float GetFireInterval()
    {
        return 1f / Mathf.Max(0.01f, fireRate);
    }

    private bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null || !enemy.IsAlive)
        {
            return false;
        }

        float rangeSqr = range * range;
        return (enemy.transform.position - transform.position).sqrMagnitude <= rangeSqr;
    }

    private void FindTarget()
    {
        IReadOnlyList<Enemy> enemies = EnemyRegistry.Enemies;

        float rangeSqr = range * range;
        Enemy leadingEnemy = null;
        int bestWaypointIndex = int.MinValue;
        float bestDistanceToWaypointSqr = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];

            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            float distanceToTowerSqr = (enemy.transform.position - transform.position).sqrMagnitude;

            if (distanceToTowerSqr > rangeSqr)
            {
                continue;
            }

            int waypointIndex = enemy.CurrentWaypointIndex;
            float distanceToWaypointSqr = enemy.DistanceToCurrentWaypointSqr;

            bool enemyIsFurtherAlongPath =
                waypointIndex > bestWaypointIndex ||
                (waypointIndex == bestWaypointIndex &&
                 distanceToWaypointSqr < bestDistanceToWaypointSqr);

            if (enemyIsFurtherAlongPath)
            {
                bestWaypointIndex = waypointIndex;
                bestDistanceToWaypointSqr = distanceToWaypointSqr;
                leadingEnemy = enemy;
            }
        }

        target = leadingEnemy;
    }

    private void RotateToTarget()
    {
        Transform part = rotatingPart != null ? rotatingPart : transform;

        if (target == null)
        {
            return;
        }

        Vector3 direction = target.transform.position - part.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        part.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    private Vector3 GetDirectionToTarget()
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        Transform part = rotatingPart != null ? rotatingPart : transform;
        Vector3 direction = target.transform.position - part.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return Vector3.zero;
        }

        return direction.normalized;
    }

    private void Shoot()
    {
        if (target == null)
        {
            return;
        }

        Vector3 shotDirection = GetDirectionToTarget();

        RotateToTarget();

        if (shootFeedback != null)
        {
            shootFeedback.Play(shotDirection);
        }

        if (projectilePrefab == null)
        {
            Debug.LogError($"{name} has no projectile prefab assigned.");
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
            Debug.Log("Tower already max level.");
            return false;
        }

        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(upgradeCost))
        {
            Debug.Log("Not enough money.");
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

    public void SetOccupiedTile(Tile tile)
    {
        occupiedTile = tile;
    }

    public void DeleteTower()
    {
        if (occupiedTile != null)
        {
            occupiedTile.isOccupied = false;
            occupiedTile = null;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(SellRefund);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public bool CanUpgrade()
    {
        return !IsMaxLevel && GameManager.Instance.money >= upgradeCost;
    }

    public string GetTitleText()
    {
        return $"{towerName} Lv. {level}/{maxLevel}";
    }

    public string GetStatsText()
    {
        if (IsMaxLevel)
        {
            return
                $"Damage: {damage}\n" +
                $"Range: {range:0.0}\n" +
                $"Fire Rate: {fireRate:0.0}\n" +
                $"Upgrade: MAX\n" +
                $"Sell: +{SellRefund}";
        }

        return
            $"Damage: {damage} → {damage + damageIncrease}\n" +
            $"Range: {range:0.0} → {range + rangeIncrease:0.0}\n" +
            $"Fire Rate: {fireRate:0.0} → {fireRate + fireRateIncrease:0.0}\n" +
            $"Upgrade: {upgradeCost}\n" +
            $"Sell: +{SellRefund}";
    }
}
