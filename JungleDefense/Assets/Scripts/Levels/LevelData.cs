using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Jungle Defense/Level Data")]
public class LevelData : ScriptableObject
{
    public int width;
    public int height;

    public string[] rows;

    public Wave[] waves;

    public int startMoney = 100;

    [Header("Visual")]
    public Sprite backgroundSprite;
}