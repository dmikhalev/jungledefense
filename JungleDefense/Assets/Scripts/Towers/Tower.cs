using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TowerData towerData;

    [Header("Scene References")]
    [SerializeField] private TowerShootFeedback shootFeedback;
    [SerializeField] private Transform rotatingPart;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int level = 1;
    private int damage;
    private float range;
    private float fireRate;
    private int upgradeCost;
    private GameObject projectilePrefab;

    private bool enableDoubleShot;
    private int doubleShotLevel;
    private float doubleShotChance;
    private float doubleShotOffset;

    private bool enableCriticalHit;
    private int criticalHitLevel;
    private float criticalChance;
    private float criticalMultiplier;

    private bool enableSplashStun;
    private int splashStunLevel;
    private float splashStunDuration;

    private float fireCooldown;
    private Enemy target;
    private RangeCircleRenderer rangeCircle;
    private Tile occupiedTile;

    public TowerData Data => towerData;
    public int Level => level;
    public int MaxLevel => towerData != null ? towerData.MaxLevel : 1;
    public bool IsMaxLevel => towerData == null || level >= MaxLevel;
    public int Cost => towerData != null ? towerData.Cost : 0;
    public float PlacementRange => GetConfiguredRangeForLevel(1);
    public int SellRefund => Mathf.RoundToInt(Cost * GetSellRefundPercent() / 100f);
    public Sprite Icon => towerData != null ? towerData.Icon : null;
    public string DisplayName => towerData != null ? towerData.DisplayName : "Missing TowerData";

    private void Awake()
    {
        CacheComponents();
        level = Mathf.Clamp(level, 1, MaxLevel);
        ApplyCurrentLevelData();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheComponents();

        if (!Application.isPlaying)
        {
            level = Mathf.Clamp(level, 1, MaxLevel);
            ApplyCurrentLevelData();
        }
    }
#endif

    private void CacheComponents()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (towerData == null)
        {
            return;
        }

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
            Debug.LogError($"{name} has no projectile prefab assigned in TowerData.");
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
        if (towerData == null)
        {
            Debug.LogError($"{name} cannot be upgraded because TowerData is missing.");
            return false;
        }

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
        ApplyCurrentLevelData();

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

    private void ApplyCurrentLevelData()
    {
        if (towerData == null)
        {
            Debug.LogError($"{name} has no TowerData assigned.", this);
            return;
        }

        TowerLevelData levelData = towerData.GetLevel(level);

        if (levelData == null)
        {
            Debug.LogError($"{towerData.name} has no data for tower level {level}.", towerData);
            return;
        }

        damage = levelData.damage;
        range = levelData.range;
        fireRate = levelData.fireRate;
        upgradeCost = levelData.upgradeCost;
        projectilePrefab = towerData.ProjectilePrefab;

        enableDoubleShot = towerData.EnableDoubleShot;
        doubleShotLevel = towerData.DoubleShotLevel;
        doubleShotChance = towerData.DoubleShotChance;
        doubleShotOffset = towerData.DoubleShotOffset;

        enableCriticalHit = towerData.EnableCriticalHit;
        criticalHitLevel = towerData.CriticalHitLevel;
        criticalChance = towerData.CriticalChance;
        criticalMultiplier = towerData.CriticalMultiplier;

        enableSplashStun = towerData.EnableSplashStun;
        splashStunLevel = towerData.SplashStunLevel;
        splashStunDuration = towerData.SplashStunDuration;

        ApplyLevelSprite(levelData);
    }

    private void ApplyLevelSprite(TowerLevelData levelData)
    {
        if (spriteRenderer == null || levelData == null || levelData.sprite == null)
        {
            return;
        }

        spriteRenderer.sprite = levelData.sprite;
    }

    private Sprite GetConfiguredSpriteForLevel(int levelNumber)
    {
        if (towerData == null)
        {
            return null;
        }

        TowerLevelData configuredLevel = towerData.GetLevel(levelNumber);

        return configuredLevel != null ? configuredLevel.sprite : null;
    }

    private float GetConfiguredRangeForLevel(int levelNumber)
    {
        if (towerData == null)
        {
            return 1f;
        }

        TowerLevelData configuredLevel = towerData.GetLevel(levelNumber);

        return configuredLevel != null ? configuredLevel.range : 1f;
    }

    private int GetSellRefundPercent()
    {
        return towerData != null ? towerData.SellRefundPercent : 0;
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
        return $"{DisplayName} Lv. {level}/{MaxLevel}";
    }

    public string GetStatsText()
    {
        if (towerData == null)
        {
            return "Missing TowerData";
        }

        if (IsMaxLevel)
        {
            return
                $"Damage: {damage}\n" +
                $"Range: {range:0.0}\n" +
                $"Fire Rate: {fireRate:0.0}\n" +
                $"Upgrade: MAX\n" +
                $"Sell: +{SellRefund}";
        }

        TowerLevelData nextLevel = towerData.GetLevel(level + 1);

        if (nextLevel == null)
        {
            return
                $"Damage: {damage}\n" +
                $"Range: {range:0.0}\n" +
                $"Fire Rate: {fireRate:0.0}\n" +
                $"Upgrade: MAX\n" +
                $"Sell: +{SellRefund}";
        }

        return
            $"Damage: {damage} → {nextLevel.damage}\n" +
            $"Range: {range:0.0} → {nextLevel.range:0.0}\n" +
            $"Fire Rate: {fireRate:0.0} → {nextLevel.fireRate:0.0}\n" +
            $"Upgrade: {upgradeCost}\n" +
            $"Sell: +{SellRefund}";
    }

    public Sprite GetPreviewSprite()
    {
        return GetConfiguredSpriteForLevel(1);
    }
}
