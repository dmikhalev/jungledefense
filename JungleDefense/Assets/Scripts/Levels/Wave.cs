using UnityEngine;

[System.Serializable]
public class WaveEnemyGroup
{
    public GameObject enemyPrefab;
    [Min(1)] public int count = 1;
    [Min(0.01f)] public float delayBetweenEnemies = 1f;
}
