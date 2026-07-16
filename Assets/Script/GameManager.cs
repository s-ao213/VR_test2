using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Title,
        Tutorial,
        Playing,
        GameOver
    }
    public GameState _currentState = GameState.Title;

    public static event Action Started;
    public static event Action Finished;
    public static event Action TitleRequested;
    public static event Action TutorialRequested;

    void OnEnable()
    {
        Head.OnNeedleHit += HandleNeedleHit;
        UI_Button.ClickStartButton += StartGame;
        UI_Result.OnRestartRequested += StartGame;
        UI_Result.OnTitleRequested += () => SetState(GameState.Title);
    }

    void OnDisable()
    {
        Head.OnNeedleHit -= HandleNeedleHit;
        UI_Button.ClickStartButton -= StartGame;
    }

    void Start()
    {
        TitleRequested?.Invoke();

    }

    public void SetState(GameState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        // 状態に応じたタイムスケールの管理
        if (newState == GameState.GameOver)
        {
            Time.timeScale = 0f;
        }
        else if (newState == GameState.Playing)
        {
            Time.timeScale = 1f;
        }

        switch (newState)
        {
            case GameState.Title:
                TitleRequested?.Invoke();
                break;
            case GameState.Tutorial:
                TutorialRequested?.Invoke();
                break;
            case GameState.Playing:
                Started?.Invoke();
                break;
            case GameState.GameOver:
                Finished?.Invoke();
                break;
        }
    }

    void HandleNeedleHit()
    {
        Debug.Log("Game Over");
        SetState(GameState.GameOver);
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }
}
