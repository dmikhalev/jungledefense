using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public PathManager pathManager;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, 2f);
    }

    void SpawnEnemy()
    {
        if (pathManager.waypoints.Length == 0)
        {
            Debug.LogError("No waypoints!");
            return;
        }

        GameObject enemy = Instantiate(
            enemyPrefab,
            pathManager.waypoints[0].position,
            Quaternion.identity
        );

        Enemy e = enemy.GetComponent<Enemy>();
        e.SetPath(pathManager.waypoints);
    }
}