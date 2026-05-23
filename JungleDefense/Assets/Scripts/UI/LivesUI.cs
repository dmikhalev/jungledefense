using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private GameManager subscribedGameManager;
    private int lastDisplayedLives = int.MinValue;

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void Update()
    {
        if (subscribedGameManager == null)
        {
            TrySubscribe();
        }

        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (subscribedGameManager != null || GameManager.Instance == null)
        {
            return;
        }

        subscribedGameManager = GameManager.Instance;
        subscribedGameManager.LivesChanged += UpdateLives;
    }

    private void Unsubscribe()
    {
        if (subscribedGameManager == null)
        {
            return;
        }

        subscribedGameManager.LivesChanged -= UpdateLives;
        subscribedGameManager = null;
    }

    private void Refresh()
    {
        if (GameManager.Instance != null)
        {
            UpdateLives(GameManager.Instance.lives);
        }
    }

    private void UpdateLives(int value)
    {
        if (livesText == null || value == lastDisplayedLives)
        {
            return;
        }

        lastDisplayedLives = value;
        livesText.text = $"Lives: {value}";
    }
}
