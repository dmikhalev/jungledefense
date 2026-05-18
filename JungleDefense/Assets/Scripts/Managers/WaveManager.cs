using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private GameObject victoryText;

    private int currentWaveIndex;
    private int aliveEnemies;

    private void Start()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        yield return WaitForPath();

        while (currentWaveIndex < levelData.waves.Length)
        {
            if (GameManager.Instance.isGameOver)
            {
                yield break;
            }

            Wave wave = levelData.waves[currentWaveIndex];

            Debug.Log($"Wave started: {currentWaveIndex + 1}");

            yield return StartCoroutine(SpawnWave(wave));

            currentWaveIndex++;

            yield return new WaitForSeconds(3f);
        }

        while (aliveEnemies > 0)
        {
            yield return null;
        }

        Victory();
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
        for (int i = 0; i < wave.count; i++)
        {
            if (GameManager.Instance.isGameOver)
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

    private void Victory()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        Debug.Log("Victory!");

        GameManager.Instance.isGameOver = true;
        Time.timeScale = 0f;

        if (victoryText != null)
        {
            victoryText.SetActive(true);
        }

        RestartManager restartManager = FindObjectOfType<RestartManager>();

        if (restartManager != null)
        {
            restartManager.ShowRestart();
        }
    }
}
