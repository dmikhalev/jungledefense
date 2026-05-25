using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> MoneyChanged;
    public event Action<int> LivesChanged;

    public int money = 100;
    public int lives = 10;
    public bool isGameOver;
    public int startLives = 10;

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

    private void Start()
    {
        NotifyStateChanged();
    }

    public void ResetGameState()
    {
        lives = startLives;
        isGameOver = false;
        Time.timeScale = 1f;

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResetPauseState();
        }

        NotifyStateChanged();
    }

    public void ResetState(int startingMoney, int startingLives)
    {
        money = Mathf.Max(0, startingMoney);
        lives = Mathf.Max(0, startingLives);
        isGameOver = false;
        Time.timeScale = 1f;

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResetPauseState();
        }

        NotifyStateChanged();
    }

    public void ResetMoney(int amount)
    {
        money = Mathf.Max(0, amount);
        MoneyChanged?.Invoke(money);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        money += amount;
        MoneyChanged?.Invoke(money);
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("SpendMoney called with negative amount.");
            return false;
        }

        if (money < amount)
        {
            return false;
        }

        money -= amount;
        MoneyChanged?.Invoke(money);
        return true;
    }

    public void LoseLife(int amount)
    {
        if (isGameOver || amount <= 0)
        {
            return;
        }

        lives = Mathf.Max(0, lives - amount);
        LivesChanged?.Invoke(lives);

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

        TowerUpgradeManager towerUpgrade = FindFirstObjectByType<TowerUpgradeManager>();
        if (towerUpgrade != null)
        {
            towerUpgrade.HideUI();
        }

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.HideStartWaveButton();
        }

        RestartManager restartManager = FindFirstObjectByType<RestartManager>();
        if (restartManager != null)
        {
            restartManager.ShowRestart();
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.HideAll();
        }
    }

    private void NotifyStateChanged()
    {
        MoneyChanged?.Invoke(money);
        LivesChanged?.Invoke(lives);
    }
}
