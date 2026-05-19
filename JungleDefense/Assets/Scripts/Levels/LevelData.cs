using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "TD/Level")]
public class LevelData : ScriptableObject
{
    public int width;
    public int height;

    public string[] rows;

    public Wave[] waves;
    public int startMoney;
}