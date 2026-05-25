using UnityEngine;

public class RestartManager : MonoBehaviour
{
    [SerializeField] private GameObject restartButton;

    private void Awake()
    {
        HideRestart();
    }

    public void ShowRestart()
    {
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }
    }

    public void HideRestart()
    {
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResetPauseState();
        }

        if (GameSpeedManager.Instance != null)
        {
            GameSpeedManager.Instance.ResetSpeed();
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartCurrentLevel();
        }

        HideRestart();
    }
}