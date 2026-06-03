using UnityEngine;

public class GameStateUIBinder : MonoBehaviour
{
    [Header("Gameplay Buttons")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject speedButton;
    [SerializeField] private GameObject startWaveButton;

    [Header("Panels")]
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject towerInfoPanel;

    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StateChanged += OnGameStateChanged;
            Apply(GameStateManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Apply(newState);
    }

    private void Apply(GameState state)
    {
        bool gameplayActive = state == GameState.PreparingWave || state == GameState.WaveRunning;
        bool canStartWave = state == GameState.PreparingWave;
        bool finished = state == GameState.Victory || state == GameState.Defeat;

        SetActive(pauseButton, gameplayActive);
        SetActive(speedButton, gameplayActive);
        SetActive(startWaveButton, canStartWave);
        SetActive(restartButton, finished);

        if (!gameplayActive)
        {
            SetActive(towerInfoPanel, false);
        }
    }

    private void SetActive(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }
}
