using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TD/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Min(1f)] public float maxHealth = 10f;
    [Min(0.1f)] public float speed = 2f;
    [Min(0)] public int reward = 10;
}
