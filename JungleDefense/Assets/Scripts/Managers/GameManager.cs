using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int money = 100;
    public int lives = 10;
    public bool isGameOver;
    public int startLives = 10;

    public void ResetGameState()
    {
        lives = startLives;
        isGameOver = false;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    public void ResetState(int startingMoney, int startingLives)
    {
        money = startingMoney;
        lives = startingLives;
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
        {
            return false;
        }

        money -= amount;
        return true;
    }

    public void LoseLife(int amount)
    {
        if (isGameOver)
        {
            return;
        }

        lives -= amount;
        Debug.Log($"Lives: {lives}");

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        Time.timeScale = 0f;

        Debug.Log("Game Over");

        RestartManager restartManager = FindFirstObjectByType<RestartManager>();

        if (restartManager != null)
        {
            restartManager.ShowRestart();
        }
    }

    public void ResetMoney(int amount)
    {
        money = amount;
    }
}
