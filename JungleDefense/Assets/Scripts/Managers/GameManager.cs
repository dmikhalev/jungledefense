using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int money = 100;
    public int lives = 10;
    public int score = 0; // Очки за убитых врагов

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void AddScore(int amount)
    {
        score += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        return true;
    }
}