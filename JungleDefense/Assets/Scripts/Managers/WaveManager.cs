using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject startWaveButton;

    [Header("UI")]
    [SerializeField] private TMP_Text waveButtonText;

    private LevelData currentLevel;
    private int currentWaveIndex;
    private int aliveEnemies;
    private bool isWaveRunning;

    private Action onLevelCompleted;

    public void StartLevel(LevelData level, Action levelCompletedCallback)
    {
        StopAllCoroutines();

        currentLevel = level;
        currentWaveIndex = 0;
        aliveEnemies = 0;
        isWaveRunning = false;
        onLevelCompleted = levelCompletedCallback;

        ShowStartWaveButton();
    }

    public void StartNextWave()
    {
        if (currentLevel == null)
        {
            Debug.LogError("No level assigned to WaveManager");
            return;
        }

        if (isWaveRunning)
        {
            return;
        }

        if (currentWaveIndex >= currentLevel.waves.Length)
        {
            CompleteLevel();
            return;
        }

        HideStartWaveButton();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetWaveRunning();
        }

        EventBus.Raise(new WaveStartedEvent(currentWaveIndex));

        StartCoroutine(SpawnWave(currentLevel.waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveRunning = true;

        Debug.Log("Starting wave " + (currentWaveIndex + 1));

        if (wave.enemyGroups == null || wave.enemyGroups.Length == 0)
        {
            Debug.LogWarning("Wave has no enemy groups");
        }
        else
        {
            foreach (WaveEnemyGroup group in wave.enemyGroups)
            {
                yield return StartCoroutine(SpawnEnemyGroup(group));
            }
        }

        while (aliveEnemies > 0)
        {
            yield return null;
        }

        int completedWaveIndex = currentWaveIndex;

        currentWaveIndex++;
        isWaveRunning = false;

        EventBus.Raise(new WaveCompletedEvent(completedWaveIndex));

        if (GameManager.Instance != null &&
            GameManager.Instance.isGameOver)
        {
            yield break;
        }

        if (currentWaveIndex >= currentLevel.waves.Length)
        {
            CompleteLevel();
        }
        else
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetPreparingWave();
            }

            ShowStartWaveButton();
        }
    }

    private IEnumerator SpawnEnemyGroup(WaveEnemyGroup group)
    {
        if (group == null || group.enemyPrefab == null || group.count <= 0)
        {
            yield break;
        }

        for (int i = 0; i < group.count; i++)
        {
            SpawnEnemy(group.enemyPrefab);
            yield return new WaitForSeconds(group.delayBetweenEnemies);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (PathManager.Instance == null || PathManager.Instance.startPoint == null)
        {
            Debug.LogError("Path is not ready");
            return;
        }

        Enemy enemy = EnemyPool.Instance.Spawn(
            enemyPrefab,
            PathManager.Instance.startPoint.position,
            Quaternion.identity
        );

        if (enemy == null)
        {
            Debug.LogError("Enemy prefab does not have an Enemy component");
            return;
        }

        aliveEnemies++;

        enemy.OnRemoved += OnEnemyRemoved;
        enemy.InitializeForSpawn(PathManager.Instance.waypoints);
    }

    private void OnEnemyRemoved()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }

    private void CompleteLevel()
    {
        HideStartWaveButton();

        EventBus.Raise(new LevelCompletedEvent());

        onLevelCompleted?.Invoke();
    }

    private void ShowStartWaveButton()
    {
        if (startWaveButton == null)
        {
            return;
        }

        UpdateWaveButtonText();

        if (GameManager.Instance != null &&
            GameManager.Instance.isGameOver)
        {
            startWaveButton.SetActive(false);
            return;
        }

        startWaveButton.SetActive(true);
    }

    private void UpdateWaveButtonText()
    {
        if (waveButtonText == null ||
            currentLevel == null)
        {
            return;
        }

        int waveNumber = currentWaveIndex + 1;
        int totalWaves = currentLevel.waves.Length;

        string prefix =
            currentWaveIndex == 0
            ? "START WAVE"
            : "NEXT WAVE";

        waveButtonText.text = prefix + " " + waveNumber + " / " + totalWaves;
    }

    public void HideStartWaveButton()
    {
        if (startWaveButton != null)
        {
            startWaveButton.SetActive(false);
        }
    }
}