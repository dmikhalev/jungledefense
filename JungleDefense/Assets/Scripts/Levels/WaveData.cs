using UnityEngine;

[CreateAssetMenu(
    fileName = "WaveData",
    menuName = "Jungle Defense/Wave Data")]
public class WaveData : ScriptableObject
{
    [SerializeField]
    private WaveEnemyGroup[] enemyGroups;

    public WaveEnemyGroup[] EnemyGroups => enemyGroups;

    public bool IsEmpty =>
        enemyGroups == null ||
        enemyGroups.Length == 0;
}