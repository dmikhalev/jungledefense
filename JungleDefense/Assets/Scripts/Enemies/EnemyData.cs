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


    [Header("Shadow Movement")]
    [Min(0.1f)] public float shadowPauseDuration = 2f;
    [Min(0.1f)] public float shadowFinalPauseDuration = 3f;
    [Min(0f)] public float shadowInvulnerabilityDuration = 0.75f;
    [Min(0f)] public float shadowPulseScale = 0.12f;
    [Min(0.1f)] public float shadowPulseSpeed = 1f;


    [Header("Boss Ability")]
    public BossAbilityType bossAbility = BossAbilityType.None;

    [Header("Boss Rage")]
    [Tooltip("HP percentages that trigger rage. 0.75 = 75% HP.")]
    public float[] rageHealthThresholds = { 0.75f, 0.5f, 0.25f };

    [Tooltip("Speed multiplier applied once per triggered rage threshold.")]
    [Min(1f)] public float rageSpeedMultiplier = 1.2f;

    [Header("Boss Regeneration")]
    [Min(0.1f)] public float regenerationInterval = 5f;

    [Tooltip("Percent of max health restored every regeneration tick. 0.03 = 3%.")]
    [Range(0f, 1f)] public float regenerationPercentOfMaxHealth = 0.03f;

    [Header("Shadow King")]
    [Tooltip("Boss jumps forward once when HP falls to this percent. 0.7 = 70% HP.")]
    [Range(0f, 1f)] public float shadowKingTeleportHealthPercent = 0.7f;

    [Tooltip("How much of the route Shadow King skips during the shadow jump. 0.25 = 25% of route.")]
    [Range(0f, 1f)] public float shadowKingTeleportRoutePercent = 0.25f;

    [Tooltip("Below this HP percent Shadow King periodically becomes invulnerable. 0.15 = 15% HP.")]
    [Range(0f, 1f)] public float shadowKingInvulnerabilityHealthPercent = 0.15f;

    [Min(0.1f)] public float shadowKingInvulnerabilityInterval = 3f;
    [Min(0.1f)] public float shadowKingInvulnerabilityDuration = 1f;

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
