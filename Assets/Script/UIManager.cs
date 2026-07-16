using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _titleUI;
    [SerializeField] private CanvasGroup _gameUI;
    [SerializeField] private CanvasGroup _resultUI;

    void OnEnable() {
        GameManager.TitleRequested += ShowTitle;
        GameManager.Started += ShowGameUI;
        GameManager.Finished += ShowResultUI;
    }
    void OnDisable() {
        GameManager.TitleRequested -= ShowTitle;
        GameManager.Started -= ShowGameUI;
        GameManager.Finished -= ShowResultUI;
    }

    // Button OnClick から呼ぶ
    // public void OpenAchievementList() {
    //     StartCoroutine(ShowAchievementList());
    // }

    public void ShowTitle()
    {
        AllalphaZero();
        _titleUI.alpha = 1f;
        _titleUI.interactable = true;
    }

    public void ShowGameUI()
    {
        AllalphaZero();
        _gameUI.alpha = 1f;
        _gameUI.interactable = true;
    }

    public void ShowResultUI()
    {
        AllalphaZero();
        _resultUI.alpha = 1f;
        _resultUI.interactable = true;
    }

    private void AllalphaZero()
    {
        _titleUI.alpha = 0f;
        _titleUI.interactable = false;
        _gameUI.alpha = 0f;
        _gameUI.interactable = false;
        _resultUI.alpha = 0f;
        _resultUI.interactable = false;
    }

}
