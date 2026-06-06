public readonly struct EnemyKilledEvent
{
    public readonly Enemy Enemy;
    public readonly int Reward;

    public EnemyKilledEvent(Enemy enemy, int reward)
    {
        Enemy = enemy;
        Reward = reward;
    }
}

public readonly struct EnemyReachedBaseEvent
{
    public readonly Enemy Enemy;
    public readonly int DamageToBase;

    public EnemyReachedBaseEvent(Enemy enemy, int damageToBase)
    {
        Enemy = enemy;
        DamageToBase = damageToBase;
    }
}

public readonly struct TowerPlacedEvent
{
    public readonly Tower Tower;

    public TowerPlacedEvent(Tower tower)
    {
        Tower = tower;
    }
}

public readonly struct TowerUpgradedEvent
{
    public readonly Tower Tower;
    public readonly int Level;

    public TowerUpgradedEvent(Tower tower, int level)
    {
        Tower = tower;
        Level = level;
    }
}

public readonly struct WaveStartedEvent
{
    public readonly int WaveIndex;
    public readonly int WaveNumber;

    public WaveStartedEvent(int waveIndex)
    {
        WaveIndex = waveIndex;
        WaveNumber = waveIndex + 1;
    }
}

public readonly struct WaveCompletedEvent
{
    public readonly int WaveIndex;
    public readonly int WaveNumber;

    public WaveCompletedEvent(int waveIndex)
    {
        WaveIndex = waveIndex;
        WaveNumber = waveIndex + 1;
    }
}

public readonly struct LevelCompletedEvent
{
}


public readonly struct BossSpawnedEvent
{
    public readonly Enemy Boss;
    public readonly string BossName;
    public readonly float CurrentHealth;
    public readonly float MaxHealth;

    public BossSpawnedEvent(Enemy boss, string bossName, float currentHealth, float maxHealth)
    {
        Boss = boss;
        BossName = bossName;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}

public readonly struct BossHealthChangedEvent
{
    public readonly Enemy Boss;
    public readonly float CurrentHealth;
    public readonly float MaxHealth;

    public BossHealthChangedEvent(Enemy boss, float currentHealth, float maxHealth)
    {
        Boss = boss;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}

public readonly struct BossRemovedEvent
{
    public readonly Enemy Boss;

    public BossRemovedEvent(Enemy boss)
    {
        Boss = boss;
    }
}
