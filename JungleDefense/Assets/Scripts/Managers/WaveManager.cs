using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public LevelData levelData;

    private int currentWaveIndex = 0;

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

            Debug.Log("Ñòàðò âîëíû: " + (currentWaveIndex + 1));

            yield return StartCoroutine(SpawnWave(wave));

            currentWaveIndex++;

            // ïàóçà ìåæäó âîëíàìè
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("ÂÑÅ ÂÎËÍÛ ÏÐÎÉÄÅÍÛ");
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
    }
}