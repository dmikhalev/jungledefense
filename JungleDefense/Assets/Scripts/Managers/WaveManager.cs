using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public LevelData levelData;

    private int currentWaveIndex = 0;
    private int aliveEnemies = 0;

    public GameObject victoryText;

    void Start()
    {
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        yield return new WaitForSeconds(1f);

        while (currentWaveIndex < levelData.waves.Length)
        {
            Wave wave = levelData.waves[currentWaveIndex];

            Debug.Log("Старт волны: " + (currentWaveIndex + 1));

            yield return StartCoroutine(SpawnWave(wave));

            currentWaveIndex++;

            // пауза между волнами
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("ВСЕ ВОЛНЫ ЗАСПАВНЕНЫ");

        // ждём пока все враги умрут
        while (aliveEnemies > 0)
        {
            yield return null;
        }

        Victory();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);

            yield return new WaitForSeconds(wave.delayBetweenEnemies);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform start = PathManager.Instance.startPoint;

        GameObject enemyGO = Instantiate(enemyPrefab, start.position, Quaternion.identity);

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.SetPath(PathManager.Instance.waypoints);
        enemy.onDeath += OnEnemyDied;

        aliveEnemies++;
    }

    void OnEnemyDied()
    {
        aliveEnemies--;
    }

    void Victory()
    {
        Debug.Log("ПОБЕДА!");

        GameManager.Instance.isGameOver = true;

        Time.timeScale = 0f;
        victoryText.SetActive(true);
    }
}