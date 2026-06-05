using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private const int DefaultInitialSize = 6;

    private static EnemyPool instance;

    public static EnemyPool Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            GameObject poolObject = new GameObject("EnemyPool");
            instance = poolObject.AddComponent<EnemyPool>();
            return instance;
        }
    }

    private readonly Dictionary<GameObject, Queue<Enemy>> pools = new();
    private readonly Dictionary<GameObject, Transform> poolRoots = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public Enemy Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return null;
        }

        if (!pools.TryGetValue(prefab, out Queue<Enemy> pool))
        {
            pool = CreatePool(prefab);
        }

        Enemy enemy =
            pool.Count > 0
            ? pool.Dequeue()
            : CreateEnemy(prefab);

        if (enemy == null)
        {
            return null;
        }

        Transform enemyTransform = enemy.transform;
        enemyTransform.SetPositionAndRotation(position, rotation);
        enemyTransform.SetParent(null, true);
        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public void Release(GameObject prefab, Enemy enemy)
    {
        if (prefab == null || enemy == null)
        {
            return;
        }

        if (!pools.TryGetValue(prefab, out Queue<Enemy> pool))
        {
            pool = CreatePool(prefab);
        }

        enemy.gameObject.SetActive(false);

        if (poolRoots.TryGetValue(prefab, out Transform root))
        {
            enemy.transform.SetParent(root, false);
        }

        pool.Enqueue(enemy);
    }

    public void Prewarm(GameObject prefab, int count = DefaultInitialSize)
    {
        if (prefab == null || count <= 0)
        {
            return;
        }

        if (!pools.TryGetValue(prefab, out Queue<Enemy> pool))
        {
            pool = CreatePool(prefab);
        }

        for (int i = 0; i < count; i++)
        {
            Enemy enemy = CreateEnemy(prefab);

            if (enemy == null)
            {
                continue;
            }

            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    private Queue<Enemy> CreatePool(GameObject prefab)
    {
        Queue<Enemy> pool = new();
        pools[prefab] = pool;

        GameObject rootObject = new GameObject($"{prefab.name}_Pool");
        rootObject.transform.SetParent(transform, false);
        poolRoots[prefab] = rootObject.transform;

        return pool;
    }

    private Enemy CreateEnemy(GameObject prefab)
    {
        Transform parent =
            poolRoots.TryGetValue(prefab, out Transform root)
            ? root
            : transform;

        GameObject instance = Instantiate(prefab, parent);
        Enemy enemy = instance.GetComponent<Enemy>();

        if (enemy == null)
        {
            Debug.LogError($"Enemy prefab {prefab.name} does not have an Enemy component.");
            Destroy(instance);
            return null;
        }

        enemy.SetPool(this, prefab);
        return enemy;
    }
}
