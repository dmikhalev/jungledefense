using UnityEngine;

[System.Serializable]
public class WaveEnemyGroup
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float delayBetweenEnemies = 1f;
}

[System.Serializable]
public class Wave
{
    public WaveEnemyGroup[] enemyGroups;
}
