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
        if (pathManager.waypoints.Count == 0)
        {
            Debug.LogError("No waypoints!");
            return;
        }

        Transform start = PathManager.Instance.startPoint;

        GameObject enemyGO = Instantiate(enemyPrefab, start.position, Quaternion.identity);

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.SetPath(PathManager.Instance.waypoints);

        Enemy e = enemy.GetComponent<Enemy>();
        e.SetPath(pathManager.waypoints);
    }
}