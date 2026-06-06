using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }

    [SerializeField] private float autoSaveInterval = 15f;

    private float saveTimer;
    private bool isDirty;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        EventBus.Subscribe<EnemyReachedBaseEvent>(OnEnemyReachedBase);
        EventBus.Subscribe<TowerPlacedEvent>(OnTowerPlaced);
        EventBus.Subscribe<TowerUpgradedEvent>(OnTowerUpgraded);
        EventBus.Subscribe<WaveStartedEvent>(OnWaveStarted);
        EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
        EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        EventBus.Unsubscribe<EnemyReachedBaseEvent>(OnEnemyReachedBase);
        EventBus.Unsubscribe<TowerPlacedEvent>(OnTowerPlaced);
        EventBus.Unsubscribe<TowerUpgradedEvent>(OnTowerUpgraded);
        EventBus.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
        EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
        EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void Update()
    {
        if (!isDirty)
        {
            return;
        }

        saveTimer += Time.unscaledDeltaTime;

        if (saveTimer >= autoSaveInterval)
        {
            Flush();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Flush();
        }
    }

    private void OnApplicationQuit()
    {
        Flush();
    }

    private void OnEnemyKilled(EnemyKilledEvent e)
    {
        SaveManager.Instance.Data.statistics.enemiesKilled++;
        MarkDirty();
    }

    private void OnEnemyReachedBase(EnemyReachedBaseEvent e)
    {
        SaveManager.Instance.Data.statistics.enemiesReachedBase++;
        MarkDirty();
    }

    private void OnTowerPlaced(TowerPlacedEvent e)
    {
        SaveManager.Instance.Data.statistics.towersPlaced++;
        MarkDirty();
    }

    private void OnTowerUpgraded(TowerUpgradedEvent e)
    {
        SaveManager.Instance.Data.statistics.towersUpgraded++;
        MarkDirty();
    }

    private void OnWaveStarted(WaveStartedEvent e)
    {
        SaveManager.Instance.Data.statistics.wavesStarted++;
        MarkDirty();
    }

    private void OnWaveCompleted(WaveCompletedEvent e)
    {
        SaveManager.Instance.Data.statistics.wavesCompleted++;
        MarkDirty();
    }

    private void OnLevelCompleted(LevelCompletedEvent e)
    {
        SaveManager.Instance.Data.statistics.levelsCompleted++;
        Flush();
    }

    private void MarkDirty()
    {
        isDirty = true;
    }

    private void Flush()
    {
        if (!isDirty || SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.Save();

        isDirty = false;
        saveTimer = 0f;
    }
}