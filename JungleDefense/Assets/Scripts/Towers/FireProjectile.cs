using UnityEngine;

public class FireProjectile : Projectile
{
    [SerializeField] private float critChance = 0.2f;
    [SerializeField] private int critMultiplier = 2;

    protected override int CalculateDamage()
    {
        if (Random.value <= critChance)
        {
            return damage * critMultiplier;
        }

        return damage;
    }
}