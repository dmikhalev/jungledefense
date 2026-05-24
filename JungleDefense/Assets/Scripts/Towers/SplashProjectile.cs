using UnityEngine;

public class SplashProjectile : Projectile
{
    private const int MaxSplashHits = 32;
    private static readonly Collider2D[] SplashHits = new Collider2D[MaxSplashHits];

    [SerializeField] private float splashRadius = 1.5f;
    [Header("Watermelon Projectile")]
    [SerializeField] private Color watermelonSplatColor = new Color(0.1f, 0.85f, 0.35f, 0.55f);
    [SerializeField] private float watermelonSplatSize = 0.55f;

    protected override float SpinSpeedDegrees => 180f;
    protected override float ArcScaleAmount => 0.18f;

    protected override void SpawnFruitSplat()
    {
        FruitSplatDecalSpawner.Spawn(transform.position, watermelonSplatColor, watermelonSplatSize);
    }

    protected override void HitTarget()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            splashRadius,
            SplashHits
        );

        SpawnHitEffect();

        int calculatedDamage = CalculateDamage();

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = SplashHits[i];
            SplashHits[i] = null;

            if (hit == null)
            {
                continue;
            }

            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(calculatedDamage);
            }
        }

        Destroy(gameObject);
    }
}
