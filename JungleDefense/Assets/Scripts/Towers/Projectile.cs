using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    [SerializeField] private GameObject hitEffectPrefab;

    protected Transform target;

    private Vector3 originalScale;
    private float initialDistanceToTarget;

    protected virtual float SpinSpeedDegrees => 0f;
    protected virtual float ArcScaleAmount => 0f;

    protected virtual void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            initialDistanceToTarget = Vector3.Distance(transform.position, target.position);
        }
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
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
        Destroy(gameObject);
    }
}
