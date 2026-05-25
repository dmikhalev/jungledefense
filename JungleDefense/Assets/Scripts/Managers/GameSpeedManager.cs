using TMPro;
using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    public static GameSpeedManager Instance { get; private set; }

    [SerializeField] private TMP_Text speedButtonText;

    private bool isFastMode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetSpeed();
    }

    public void ToggleSpeed()
    {
        SetSpeed(!isFastMode);
    }

    public void ResetSpeed()
    {
        SetSpeed(false);
    }

    private void SetSpeed(bool fast)
    {
        isFastMode = fast;

        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = isFastMode ? 2f : 1f;
        }

        if (speedButtonText != null)
        {
            speedButtonText.text = isFastMode ? "x1" : "x2";
        }
    }
}
