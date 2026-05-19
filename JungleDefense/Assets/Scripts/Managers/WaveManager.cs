using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private LevelData currentLevelData;
    private Action onLevelCompleted;

    private int currentWaveIndex;
    private int aliveEnemies;
    private Coroutine waveRoutine;

    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWaves => currentLevelData != null && currentLevelData.waves != null ? currentLevelData.waves.Length : 0;
    public int AliveEnemies => aliveEnemies;

    public void StartLevel(LevelData level, System.Action levelCompletedCallback)
    {
        StopAllCoroutines();

        currentLevelData = level;
        currentWaveIndex = 0;
        aliveEnemies = 0;
        onLevelCompleted = levelCompletedCallback;

        StartCoroutine(RunWaves());
    }

    public void StopCurrentLevel()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        currentWaveIndex = 0;
        aliveEnemies = 0;
        currentLevelData = null;
        onLevelCompleted = null;
    }

    private IEnumerator RunWaves()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("WaveManager cannot start: LevelData is null.");
            yield break;
        }

        if (currentLevelData.waves == null || currentLevelData.waves.Length == 0)
        {
            Debug.LogWarning("Level has no waves. Completing level immediately.");
            onLevelCompleted?.Invoke();
            yield break;
        }

        yield return WaitForPath();

        while (currentWaveIndex < currentLevelData.waves.Length)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield break;
            }

            Wave wave = currentLevelData.waves[currentWaveIndex];

            Debug.Log($"Wave started: {currentWaveIndex + 1}/{currentLevelData.waves.Length}");

            yield return StartCoroutine(SpawnWave(wave));
            currentWaveIndex++;

            if (currentWaveIndex < currentLevelData.waves.Length)
            {
                yield return new WaitForSeconds(3f);
            }
        }

        while (aliveEnemies > 0)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield break;
            }

            yield return null;
        }

        onLevelCompleted?.Invoke();
    }

    private IEnumerator WaitForPath()
    {
        while (PathManager.Instance == null || PathManager.Instance.startPoint == null)
        {
            yield return null;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        if (wave == null)
        {
            yield break;
        }

        for (int i = 0; i < wave.count; i++)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                yield break;
            }

            SpawnEnemy(wave.enemyPrefab);

            yield return new WaitForSeconds(wave.delayBetweenEnemies);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Wave contains an empty enemy prefab.");
            return;
        }

        if (PathManager.Instance == null || PathManager.Instance.startPoint == null)
        {
            Debug.LogError("Cannot spawn enemy: path is not ready.");
            return;
        }

        Transform start = PathManager.Instance.startPoint;
        GameObject enemyObject = Instantiate(enemyPrefab, start.position, Quaternion.identity);

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError("Enemy prefab does not have Enemy component.");
            Destroy(enemyObject);
            return;
        }

        enemy.SetPath(PathManager.Instance.waypoints);
        enemy.OnRemoved += OnEnemyRemoved;

        aliveEnemies++;
    }

    private void OnEnemyRemoved()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}
