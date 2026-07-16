using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public int score = 0; // スコアの初期値
    public int highScore = 0; // ハイスコアの初期値

    public static event Action<int> OnScoreChanged; // スコアが変化したときのイベント
    public static event Action<int> OnHighScoreChanged; // ハイスコアが変化したときのイベント

    //イベント購読用
    protected Action _onFoodEaten;

    void OnEnable()
    {
        Head.OnFoodEaten += AddScore;
    }
    void OnDisable()
    {
        Head.OnFoodEaten -= AddScore;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddScore()
    {
        score += 10; // スコアを10増加
        OnScoreChanged?.Invoke(score); // スコアが変化したことを通知

        if(score > highScore)
        {
            highScore = score; // ハイスコアを更新
            OnHighScoreChanged?.Invoke(highScore); // ハイスコアが変化したことを通知
        }
    }
}
