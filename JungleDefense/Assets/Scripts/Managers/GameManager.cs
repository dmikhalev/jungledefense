using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int money = 100;
    public int lives = 10;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void LoseLife(int amount)
    {
        lives -= amount;

        if (lives <= 0)
        {
            Debug.Log("Game Over");
        }
    }
}