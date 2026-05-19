using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("References")]
    [SerializeField] private LevelBuilder levelBuilder;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject victoryText;

    private int currentLevelIndex;

    public LevelData CurrentLevel => levels[currentLevelIndex];

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

        ClearLevel();

        LevelData level = levels[currentLevelIndex];

        GameManager.Instance.ResetMoney(level.startMoney);

        levelBuilder.BuildLevel(level);
        waveManager.StartLevel(level, OnLevelCompleted);
        if (victoryText != null)
        {
            victoryText.SetActive(false);
        }

        FindFirstObjectByType<RestartManager>()?.HideRestart();
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

    private void ClearLevel()
    {
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }

        foreach (Tower tower in FindObjectsByType<Tower>(FindObjectsSortMode.None))
        {
            Destroy(tower.gameObject);
        }

        foreach (Tile tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            Destroy(tile.gameObject);
        }

        if (PathManager.Instance != null)
        {
            PathManager.Instance.ClearPath();
        }
    }

    private void OnLevelCompleted()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        if (nextLevelIndex < levels.Length)
        {
            Debug.Log("Loading next level: " + nextLevelIndex);
            LoadLevel(nextLevelIndex);
            return;
        }

        Debug.Log("All levels completed");

        if (victoryText != null)
        {
            victoryText.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}