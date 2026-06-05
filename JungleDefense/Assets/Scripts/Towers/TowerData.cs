using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Jungle Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string displayName = "Tower";
    [SerializeField] private Sprite icon;

    [Header("Economy")]
    [SerializeField] private int cost = 50;
    [SerializeField, Range(0, 100)] private int sellRefundPercent = 50;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Levels")]
    [SerializeField] private TowerLevelData[] levels =
    {
        new TowerLevelData
        {
            damage = 1,
            range = 3f,
            fireRate = 1f,
            upgradeCost = 50
        },
        new TowerLevelData
        {
            damage = 2,
            range = 3.5f,
            fireRate = 1.2f,
            upgradeCost = 100
        },
        new TowerLevelData
        {
            damage = 3,
            range = 4f,
            fireRate = 1.4f,
            upgradeCost = 0
        }
    };

    [Header("Double Shot")]
    [SerializeField] private bool enableDoubleShot;
    [SerializeField] private int doubleShotLevel = 3;
    [SerializeField, Range(0f, 1f)] private float doubleShotChance = 0.3f;
    [SerializeField] private float doubleShotOffset = 0.18f;

    [Header("Critical Hit")]
    [SerializeField] private bool enableCriticalHit;
    [SerializeField] private int criticalHitLevel = 3;
    [SerializeField, Range(0f, 1f)] private float criticalChance = 0.25f;
    [SerializeField] private float criticalMultiplier = 2f;

    [Header("Splash Stun")]
    [SerializeField] private bool enableSplashStun;
    [SerializeField] private int splashStunLevel = 3;
    [SerializeField] private float splashStunDuration = 0.35f;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int Cost => Mathf.Max(0, cost);
    public int SellRefundPercent => Mathf.Clamp(sellRefundPercent, 0, 100);
    public GameObject ProjectilePrefab => projectilePrefab;
    public int MaxLevel => Mathf.Max(1, levels == null ? 1 : levels.Length);

    public bool EnableDoubleShot => enableDoubleShot;
    public int DoubleShotLevel => doubleShotLevel;
    public float DoubleShotChance => doubleShotChance;
    public float DoubleShotOffset => doubleShotOffset;

    public bool EnableCriticalHit => enableCriticalHit;
    public int CriticalHitLevel => criticalHitLevel;
    public float CriticalChance => criticalChance;
    public float CriticalMultiplier => criticalMultiplier;

    public bool EnableSplashStun => enableSplashStun;
    public int SplashStunLevel => splashStunLevel;
    public float SplashStunDuration => splashStunDuration;

    public TowerLevelData GetLevel(int level)
    {
        if (levels == null || levels.Length == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
        return levels[index];
    }
}

[System.Serializable]
public class TowerLevelData
{
    [Min(1)] public int damage = 1;
    [Min(0.1f)] public float range = 3f;
    [Min(0.01f)] public float fireRate = 1f;
    [Min(0)] public int upgradeCost = 50;
    public Sprite sprite;
}
