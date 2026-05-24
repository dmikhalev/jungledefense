using UnityEngine;

public class FireProjectile : Projectile
{
    [Header("Orange Projectile")]
    [SerializeField] private Color orangeSplatColor = new Color(1f, 0.42f, 0.04f, 0.6f);
    [SerializeField] private float orangeSplatSize = 0.38f;
    [SerializeField] private float critChance = 0.2f;
    [SerializeField] private int critMultiplier = 2;

    protected override float SpinSpeedDegrees => 540f;

    protected override void SpawnFruitSplat()
    {
        FruitSplatDecalSpawner.Spawn(transform.position, orangeSplatColor, orangeSplatSize);
    }

    protected override int CalculateDamage()
    {
        if (Random.value <= critChance)
        {
            return damage * critMultiplier;
        }

        return damage;
    }
}
