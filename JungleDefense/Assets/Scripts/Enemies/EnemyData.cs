using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TD/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;
    public EnemyType enemyType = EnemyType.Normal;
    public Sprite sprite;

    [Header("Stats")]
    [Min(1f)] public float maxHealth = 10f;
    [Min(0.1f)] public float speed = 2f;
    [Min(0)] public int reward = 10;
    [Min(1)] public int damageToBase = 1;

    [Header("Damage Multipliers")]
    [Tooltip("1 = normal damage, 0.7 = 30% resistance, 1.3 = 30% weakness")]
    [Min(0f)] public float directDamageMultiplier = 1f;

    [Tooltip("1 = normal splash damage, 0.7 = 30% splash resistance, 1.3 = 30% splash weakness")]
    [Min(0f)] public float splashDamageMultiplier = 1f;

    [Header("Control Multipliers")]
    [Tooltip("1 = normal stun duration, 0.5 = 50% shorter stun, 0 = immune to stun")]
    [Min(0f)] public float stunDurationMultiplier = 1f;

    public float GetDamageMultiplier(DamageType damageType)
    {
        return damageType == DamageType.Splash
            ? splashDamageMultiplier
            : directDamageMultiplier;
    }

    public float GetAdjustedStunDuration(float baseDuration)
    {
        return Mathf.Max(0f, baseDuration * stunDurationMultiplier);
    }
}
