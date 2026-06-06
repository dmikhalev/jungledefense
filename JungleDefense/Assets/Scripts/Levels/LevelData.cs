using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Jungle Defense/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Grid")]
    public int width;
    public int height;
    public string[] rows;

    [Header("Economy")]
    public int startMoney = 100;

    [Header("Waves")]
    [SerializeField] private WaveData[] waves;

    [Header("Visual")]
    public Sprite backgroundSprite;

    public int WaveCount => waves == null ? 0 : waves.Length;

    public WaveData GetWave(int index)
    {
        if (waves == null || index < 0 || index >= waves.Length)
        {
            return null;
        }

        return waves[index];
    }
}
