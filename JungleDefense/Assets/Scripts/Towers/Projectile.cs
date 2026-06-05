using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private bool spawnFruitSplat = true;
    [SerializeField] private Color fruitSplatColor = new Color(1f, 0.84f, 0.18f, 0.55f);
    [SerializeField] private float fruitSplatSize = 0.35f;

    protected Transform target;

    private Vector3 originalScale;
    private float initialDistanceToTarget;
    private ProjectilePool pool;
    private GameObject sourcePrefab;

    protected virtual float SpinSpeedDegrees => 0f;
    protected virtual float ArcScaleAmount => 0f;

    protected virtual void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetPool(ProjectilePool ownerPool, GameObject prefab)
    {
        pool = ownerPool;
        sourcePrefab = prefab;
    }

    public void Launch(Transform newTarget, int projectileDamage)
    {
        damage = projectileDamage;
        SetTarget(newTarget);
        ResetVisualState();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            initialDistanceToTarget = Vector3.Distance(transform.position, target.position);
        }
        else
        {
            initialDistanceToTarget = 0f;
        }
    }

    protected virtual void OnDisable()
    {
        target = null;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            Release();
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

        UpdateVisual(direction);
    }

    protected virtual void UpdateVisual(Vector3 direction)
    {
        ApplyArcScale();

        if (Mathf.Abs(SpinSpeedDegrees) > 0.01f)
        {
            transform.Rotate(0f, 0f, SpinSpeedDegrees * Time.deltaTime);
            return;
        }

        RotateToDirection(direction);
    }

    protected virtual void RotateToDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ApplyArcScale()
    {
        if (ArcScaleAmount <= 0f || initialDistanceToTarget <= 0.001f || target == null)
        {
            return;
        }

        float remainingDistance = Vector3.Distance(transform.position, target.position);
        float progress = 1f - Mathf.Clamp01(remainingDistance / initialDistanceToTarget);
        float arc = Mathf.Sin(progress * Mathf.PI) * ArcScaleAmount;

        transform.localScale = originalScale * (1f + arc);
    }

    protected void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        SpawnFruitSplat();
    }

    protected virtual void SpawnFruitSplat()
    {
        if (!spawnFruitSplat)
        {
            return;
        }

        FruitSplatDecalSpawner.Spawn(transform.position, fruitSplatColor, fruitSplatSize);
    }

    protected virtual int CalculateDamage()
    {
        return damage;
    }

    protected virtual void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(CalculateDamage());
        }

        SpawnHitEffect();
        Release();
    }

    protected void Release()
    {
        target = null;
        ResetVisualState();

        if (pool != null && sourcePrefab != null)
        {
            pool.Release(sourcePrefab, this);
            return;
        }

        Destroy(gameObject);
    }

    private void ResetVisualState()
    {
        transform.localScale = originalScale;
    }
}
