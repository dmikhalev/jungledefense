using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("References")]
    [SerializeField] private LevelBuilder levelBuilder;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private LevelBackgroundManager backgroundManager;
    [SerializeField] private LevelCompleteScreenUI levelCompleteScreen;

    private int currentLevelIndex;

    public int CurrentLevelIndex => currentLevelIndex;
    public int CurrentLevelNumber => currentLevelIndex + 1;
    public int LevelCount => levels != null ? levels.Length : 0;

    public LevelData CurrentLevel =>
        levels != null && levels.Length > 0 && currentLevelIndex >= 0 && currentLevelIndex < levels.Length
            ? levels[currentLevelIndex]
            : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;

        if (GameSpeedManager.Instance != null)
        {
            GameSpeedManager.Instance.ResetSpeed();
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResetPauseState();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameState();
        }

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No levels assigned in LevelManager");
            return;
        }

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        currentLevelIndex = levelIndex;

        ClearRuntimeObjects();

        LevelData level = levels[currentLevelIndex];

        if (backgroundManager != null)
        {
            backgroundManager.SetBackground(level.backgroundSprite);
        }


        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetMoney(level.startMoney);
        }

        if (levelBuilder == null || waveManager == null)
        {
            Debug.LogError("LevelManager references are not assigned.");
            return;
        }

        levelBuilder.BuildLevel(level);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetPreparingWave();
        }

        waveManager.StartLevel(level, OnLevelCompleted);

        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.HideInstant();
        }

        FindFirstObjectByType<RestartManager>()?.HideRestart();
        FindObjectOfType<CurrentLevelLabel>()?.Refresh();
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex >= levels.Length)
        {
            Debug.Log("All levels completed");
            return;
        }

        LoadLevel(nextIndex);
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    private void ClearRuntimeObjects()
    {
        TowerPlacementManager.Instance?.ClearSelection();

        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            enemy.DespawnImmediately();
        }

        foreach (Tower tower in FindObjectsByType<Tower>(FindObjectsSortMode.None))
        {
            Destroy(tower.gameObject);
        }

        foreach (Projectile projectile in FindObjectsByType<Projectile>(FindObjectsSortMode.None))
        {
            Destroy(projectile.gameObject);
        }

        foreach (FloatingDamageText damageText in FindObjectsByType<FloatingDamageText>(FindObjectsSortMode.None))
        {
            Destroy(damageText.gameObject);
        }

        EnemyRegistry.Clear();

        if (PathManager.Instance != null)
        {
            PathManager.Instance.ClearPath();
        }
    }

    private void OnLevelCompleted()
    {
        int stars = CalculateStarsForCurrentLevel();

        SaveManager.Instance?.CompleteLevel(currentLevelIndex, stars);

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetVictory();
        }

        HideGameplayUI();

        bool hasNextLevel = levels != null && currentLevelIndex + 1 < levels.Length;

        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.Show(currentLevelIndex, stars, hasNextLevel);
        }

        Time.timeScale = 0f;
    }

    private int CalculateStarsForCurrentLevel()
    {
        if (GameManager.Instance == null)
        {
            return 1;
        }

        int startLives = Mathf.Max(0, GameManager.Instance.startLives);
        int currentLives = Mathf.Clamp(GameManager.Instance.lives, 0, startLives);

        if (startLives <= 0)
        {
            return 1;
        }

        int lostLives = startLives - currentLives;

        if (lostLives == 0)
        {
            return 3;
        }

        if (lostLives <= 2)
        {
            return 2;
        }

        return 1;
    }

    private void HideGameplayUI()
    {
        TowerUpgradeManager towerUpgrade = FindFirstObjectByType<TowerUpgradeManager>();

        if (towerUpgrade != null)
        {
            towerUpgrade.HideUI();
        }

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();

        if (waveManager != null)
        {
            waveManager.HideStartWaveButton();
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.HideAll();
        }
    }
}