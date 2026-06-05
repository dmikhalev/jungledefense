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
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Level Visuals")]
    [SerializeField] private Sprite[] levelSprites;

    [Header("Special Abilities")]
    [SerializeField] private bool enableDoubleShot;
    [SerializeField] private int doubleShotLevel = 3;
    [SerializeField, Range(0f, 1f)] private float doubleShotChance = 0.3f;
    [SerializeField] private float doubleShotOffset = 0.18f;

    [SerializeField] private bool enableCriticalHit;
    [SerializeField] private int criticalHitLevel = 3;
    [SerializeField, Range(0f, 1f)] private float criticalChance = 0.25f;
    [SerializeField] private float criticalMultiplier = 2f;

    [SerializeField] private bool enableSplashStun;
    [SerializeField] private int splashStunLevel = 3;
    [SerializeField] private float splashStunDuration = 0.35f;

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

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        ApplyLevelSprite();
    }

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

        SpawnProjectile(shotDirection, 0f);

        if (ShouldDoubleShot())
        {
            SpawnProjectile(shotDirection, doubleShotOffset);
        }
    }

    private void SpawnProjectile(Vector3 shotDirection, float sideOffset)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{name} has no projectile prefab assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (Mathf.Abs(sideOffset) > 0.001f && shotDirection.sqrMagnitude > 0.001f)
        {
            Vector3 sideDirection = new Vector3(-shotDirection.y, shotDirection.x, 0f);
            spawnPosition += sideDirection.normalized * sideOffset;
        }

        Projectile projectile = ProjectilePool.Instance.Spawn(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (projectile == null)
        {
            Debug.LogError($"Projectile prefab {projectilePrefab.name} has no Projectile component.");
            return;
        }

        projectile.Launch(target.transform, CalculateProjectileDamage());

        if (enableSplashStun &&
            level >= splashStunLevel &&
            projectile is SplashProjectile splashProjectile)
        {
            splashProjectile.SetStunDuration(splashStunDuration);
        }
    }

    private int CalculateProjectileDamage()
    {
        if (!enableCriticalHit ||
            level < criticalHitLevel ||
            Random.value > criticalChance)
        {
            return damage;
        }

        return Mathf.Max(1, Mathf.RoundToInt(damage * criticalMultiplier));
    }

    private bool ShouldDoubleShot()
    {
        return enableDoubleShot &&
               level >= doubleShotLevel &&
               Random.value <= doubleShotChance;
    }

    public void ShowRange()
    {
        if (rangeCircle == null)
        {
            GameObject rangeObject = new GameObject($"{name}_RangeCircle");
            rangeCircle = rangeObject.AddComponent<RangeCircleRenderer>();
        }

        rangeCircle.transform.position = transform.position;
        rangeCircle.transform.rotation = Quaternion.identity;
        rangeCircle.transform.localScale = Vector3.one;

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
        ApplyLevelSprite();

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

    private void OnDestroy()
    {
        if (rangeCircle != null)
        {
            Destroy(rangeCircle.gameObject);
            rangeCircle = null;
        }
    }

    private void ApplyLevelSprite()
    {
        if (spriteRenderer == null ||
            levelSprites == null ||
            levelSprites.Length == 0)
        {
            return;
        }

        int spriteIndex = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);

        if (levelSprites[spriteIndex] != null)
        {
            spriteRenderer.sprite = levelSprites[spriteIndex];
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public bool CanUpgrade()
    {
        return !IsMaxLevel &&
               GameManager.Instance != null &&
               GameManager.Instance.money >= upgradeCost;
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
