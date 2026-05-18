using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int money = 100;
    public int lives = 10;
    public bool isGameOver;

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

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        Debug.Log("Game Over");

        RestartManager restartManager = FindObjectOfType<RestartManager>();

        if (restartManager != null)
        {
            restartManager.ShowRestart();
        }
    }
}
