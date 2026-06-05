using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private const int DefaultInitialSize = 8;

    private static ProjectilePool instance;

    public static ProjectilePool Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            GameObject poolObject = new GameObject("ProjectilePool");
            instance = poolObject.AddComponent<ProjectilePool>();
            return instance;
        }
    }

    private readonly Dictionary<GameObject, Queue<Projectile>> pools = new();
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

    public Projectile Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return null;
        }

        if (!pools.TryGetValue(prefab, out Queue<Projectile> pool))
        {
            pool = CreatePool(prefab);
        }

        Projectile projectile =
            pool.Count > 0
            ? pool.Dequeue()
            : CreateProjectile(prefab);

        if (projectile == null)
        {
            return null;
        }

        Transform projectileTransform = projectile.transform;
        projectileTransform.SetPositionAndRotation(position, rotation);
        projectileTransform.SetParent(null, true);
        projectile.gameObject.SetActive(true);

        return projectile;
    }

    public void Release(GameObject prefab, Projectile projectile)
    {
        if (prefab == null || projectile == null)
        {
            return;
        }

        if (!pools.TryGetValue(prefab, out Queue<Projectile> pool))
        {
            pool = CreatePool(prefab);
        }

        projectile.gameObject.SetActive(false);

        if (poolRoots.TryGetValue(prefab, out Transform root))
        {
            projectile.transform.SetParent(root, false);
        }

        pool.Enqueue(projectile);
    }

    public void Prewarm(GameObject prefab, int count = DefaultInitialSize)
    {
        if (prefab == null || count <= 0)
        {
            return;
        }

        if (!pools.TryGetValue(prefab, out Queue<Projectile> pool))
        {
            pool = CreatePool(prefab);
        }

        for (int i = 0; i < count; i++)
        {
            Projectile projectile = CreateProjectile(prefab);

            if (projectile == null)
            {
                continue;
            }

            projectile.gameObject.SetActive(false);
            pool.Enqueue(projectile);
        }
    }

    private Queue<Projectile> CreatePool(GameObject prefab)
    {
        Queue<Projectile> pool = new();
        pools[prefab] = pool;

        GameObject rootObject = new GameObject($"{prefab.name}_Pool");
        rootObject.transform.SetParent(transform, false);
        poolRoots[prefab] = rootObject.transform;

        return pool;
    }

    private Projectile CreateProjectile(GameObject prefab)
    {
        Transform parent =
            poolRoots.TryGetValue(prefab, out Transform root)
            ? root
            : transform;

        GameObject instance = Instantiate(prefab, parent);
        Projectile projectile = instance.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError($"Projectile prefab {prefab.name} does not have a Projectile component.");
            Destroy(instance);
            return null;
        }

        projectile.SetPool(this, prefab);
        return projectile;
    }
}
