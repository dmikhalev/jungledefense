using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelSelectPanel;

    private float cachedTimeScale = 1f;
    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetPauseState();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (isPaused)
        {
            return;
        }

        cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        isPaused = true;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetPaused();
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused)
        {
            return;
        }

        Time.timeScale = cachedTimeScale <= 0f ? 1f : cachedTimeScale;
        isPaused = false;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ResumeFromPause();
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void ResetPauseState()
    {
        isPaused = false;
        cachedTimeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }
    }

    public void HideAll()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    }

    public void ShowPauseButton()
    {
        if (pauseButton != null &&
            (GameManager.Instance == null || !GameManager.Instance.isGameOver))
        {
            pauseButton.SetActive(true);
        }
    }

    public void HidePauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
    }

    public void OpenLevelSelect()
    {
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
    }
}