using UnityEngine;

public class SplashProjectile : Projectile
{
    [SerializeField] private float splashRadius = 1.5f;

    protected override void HitTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            splashRadius
        );

        SpawnHitEffect();

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage());
            }
        }

        Destroy(gameObject);
    }
}