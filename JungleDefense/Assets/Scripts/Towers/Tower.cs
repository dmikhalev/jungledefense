using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private const float EnemyRadiusCompensation = 0.2f;

    [Header("Data")]
    [SerializeField] private TowerData towerData;

    [Header("Runtime References")]
    [SerializeField] private TowerShootFeedback shootFeedback;
    [SerializeField] private Transform rotatingPart;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentLevel = 1;
    private int currentDamage;
    private float currentRange;
    private float currentFireRate;
    private int currentUpgradeCost;

    private float fireCooldown;
    private Enemy target;
    private RangeCircleRenderer rangeCircle;
    private Tile occupiedTile;

    public TowerData Data => towerData;

    public int Level => currentLevel;
    public int MaxLevel => towerData != null ? towerData.MaxLevel : 1;
    public bool IsMaxLevel => currentLevel >= MaxLevel;

    public int Cost => towerData != null ? towerData.Cost : 0;
    public float PlacementRange => GetConfiguredRangeForLevel(1);
    public int SellRefund => Mathf.RoundToInt(GetTotalInvestedCost() * GetSellRefundPercent() / 100f);
    public Sprite Icon => towerData != null ? towerData.Icon : null;
    public string DisplayName => towerData != null ? towerData.DisplayName : "Missing TowerData";

    public int Damage => currentDamage;
    public float Range => currentRange;
    public float FireRate => currentFireRate;
    public int UpgradeCost => currentUpgradeCost;

    // Compatibility properties for older UI/manager code.
    public int cost => Cost;
    public int damage => currentDamage;
    public float range => currentRange;
    public float fireRate => currentFireRate;
    public int upgradeCost => currentUpgradeCost;
    public int level => currentLevel;
    public int maxLevel => MaxLevel;
    public Sprite icon => Icon;
    public string towerName => DisplayName;
    public GameObject projectilePrefab => towerData != null ? towerData.ProjectilePrefab : null;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (towerData == null)
        {
            Debug.LogError($"{name} has no TowerData assigned. Assign TowerData on the tower prefab.");
            enabled = false;
            return;
        }

        ApplyCurrentLevelData();
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
        return 1f / Mathf.Max(0.01f, currentFireRate);
    }

    private bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null || !enemy.IsAlive)
        {
            return false;
        }

        float effectiveRange = currentRange + EnemyRadiusCompensation;
        float effectiveRangeSqr = effectiveRange * effectiveRange;

        return (enemy.transform.position - transform.position).sqrMagnitude <= effectiveRangeSqr;
    }

    private void FindTarget()
    {
        IReadOnlyList<Enemy> enemies = EnemyRegistry.Enemies;

        float effectiveRange = currentRange + EnemyRadiusCompensation;
        float effectiveRangeSqr = effectiveRange * effectiveRange;

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

            if (distanceToTowerSqr > effectiveRangeSqr)
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
            SpawnProjectile(shotDirection, towerData.DoubleShotOffset);
        }
    }

    private void SpawnProjectile(Vector3 shotDirection, float sideOffset)
    {
        GameObject prefab = towerData.ProjectilePrefab;

        if (prefab == null)
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
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        if (projectile == null)
        {
            Debug.LogError($"Projectile prefab {prefab.name} has no Projectile component.");
            return;
        }

        projectile.Launch(target.transform, CalculateProjectileDamage());

        if (towerData.EnableSplashStun &&
            currentLevel >= towerData.SplashStunLevel &&
            projectile is SplashProjectile splashProjectile)
        {
            splashProjectile.SetStunDuration(towerData.SplashStunDuration);
        }
    }

    private int CalculateProjectileDamage()
    {
        if (!towerData.EnableCriticalHit ||
            currentLevel < towerData.CriticalHitLevel ||
            Random.value > towerData.CriticalChance)
        {
            return currentDamage;
        }

        return Mathf.Max(1, Mathf.RoundToInt(currentDamage * towerData.CriticalMultiplier));
    }

    private bool ShouldDoubleShot()
    {
        return towerData.EnableDoubleShot &&
               currentLevel >= towerData.DoubleShotLevel &&
               Random.value <= towerData.DoubleShotChance;
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
        rangeCircle.Draw(currentRange);
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

        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(currentUpgradeCost))
        {
            Debug.Log("Not enough money.");
            return false;
        }

        currentLevel++;
        ApplyCurrentLevelData();

        ShowRange();

        EventBus.Raise(new TowerUpgradedEvent(this, currentLevel));

        Debug.Log("Tower upgraded to level " + currentLevel);
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
        TowerLevelData levelData = towerData.GetLevel(currentLevel);

        if (levelData == null)
        {
            Debug.LogError($"{name} has no TowerLevelData for level {currentLevel} in {towerData.name}.");
            return;
        }

        currentDamage = levelData.damage;
        currentRange = levelData.range;
        currentFireRate = levelData.fireRate;
        currentUpgradeCost = levelData.upgradeCost;

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

        return configuredLevel != null
            ? configuredLevel.sprite
            : null;
    }

    private float GetConfiguredRangeForLevel(int levelNumber)
    {
        if (towerData == null)
        {
            return 0f;
        }

        TowerLevelData configuredLevel = towerData.GetLevel(levelNumber);

        return configuredLevel != null
            ? configuredLevel.range
            : 0f;
    }

    private float GetSellRefundPercent()
    {
        return towerData != null ? towerData.SellRefundPercent : 50f;
    }

    private int GetTotalInvestedCost()
    {
        int total = Cost;

        if (towerData == null)
        {
            return total;
        }

        for (int levelNumber = 1; levelNumber < currentLevel; levelNumber++)
        {
            TowerLevelData levelData = towerData.GetLevel(levelNumber);

            if (levelData != null)
            {
                total += levelData.upgradeCost;
            }
        }

        return total;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }

    public bool CanUpgrade()
    {
        return !IsMaxLevel &&
               GameManager.Instance != null &&
               GameManager.Instance.money >= currentUpgradeCost;
    }

    public string GetTitleText()
    {
        return $"{DisplayName} Lv. {currentLevel}/{MaxLevel}";
    }

    public string GetStatsText()
    {
        if (IsMaxLevel)
        {
            return
                $"Damage: {currentDamage}\n" +
                $"Range: {currentRange:0.0}\n" +
                $"Fire Rate: {currentFireRate:0.0}\n" +
                $"Upgrade: MAX\n" +
                $"Sell: +{SellRefund}";
        }

        TowerLevelData nextLevel = towerData.GetLevel(currentLevel + 1);

        if (nextLevel != null)
        {
            return
                $"Damage: {currentDamage} → {nextLevel.damage}\n" +
                $"Range: {currentRange:0.0} → {nextLevel.range:0.0}\n" +
                $"Fire Rate: {currentFireRate:0.0} → {nextLevel.fireRate:0.0}\n" +
                $"Upgrade: {currentUpgradeCost}\n" +
                $"Sell: +{SellRefund}";
        }

        return
            $"Damage: {currentDamage}\n" +
            $"Range: {currentRange:0.0}\n" +
            $"Fire Rate: {currentFireRate:0.0}\n" +
            $"Upgrade: {currentUpgradeCost}\n" +
            $"Sell: +{SellRefund}";
    }

    public Sprite GetPreviewSprite()
    {
        Sprite sprite = GetConfiguredSpriteForLevel(1);

        if (sprite != null)
        {
            return sprite;
        }

        return spriteRenderer != null
            ? spriteRenderer.sprite
            : null;
    }
}
