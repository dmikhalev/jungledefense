using System;
using UnityEngine;

public enum GameState
{
    None,
    PreparingWave,
    WaveRunning,
    Paused,
    Victory,
    Defeat
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event Action<GameState, GameState> StateChanged;

    public GameState CurrentState { get; private set; } = GameState.None;
    public GameState PreviousState { get; private set; } = GameState.None;

    public bool IsGameplayActive =>
        CurrentState == GameState.PreparingWave ||
        CurrentState == GameState.WaveRunning;

    public bool IsTerminalState =>
        CurrentState == GameState.Victory ||
        CurrentState == GameState.Defeat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPreparingWave()
    {
        SetState(GameState.PreparingWave);
    }

    public void SetWaveRunning()
    {
        SetState(GameState.WaveRunning);
    }

    public void SetPaused()
    {
        if (IsTerminalState)
        {
            return;
        }

        SetState(GameState.Paused);
    }

    public void ResumeFromPause()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        GameState stateToRestore =
            PreviousState == GameState.WaveRunning
                ? GameState.WaveRunning
                : GameState.PreparingWave;

        SetState(stateToRestore);
    }

    public void SetVictory()
    {
        SetState(GameState.Victory);
    }

    public void SetDefeat()
    {
        SetState(GameState.Defeat);
    }

    private void SetState(GameState nextState)
    {
        if (CurrentState == nextState)
        {
            return;
        }

        GameState oldState = CurrentState;
        PreviousState = oldState;
        CurrentState = nextState;

        StateChanged?.Invoke(oldState, nextState);
    }
}
