using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TD/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    public float maxHealth = 10f;
    public float speed = 2f;
    public int reward = 10;

    public Color color = Color.white; // удобно для быстрого различия
}