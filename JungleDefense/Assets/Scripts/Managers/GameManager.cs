using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int money = 100;
    public int score = 0; // Очки за убитых врагов
    public int lives = 10;
    public bool isGameOver = false;

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
        if (isGameOver)
            return;

        lives -= amount;

        Debug.Log("Жизни: " + lives);

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;

        Debug.Log("GAME OVER");

        Time.timeScale = 0f; // останавливает игру
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