using TMPro;
using UnityEngine;
using System;
public class UI_Result : MonoBehaviour
{
    private TMP_Text scoreText; // スコア表示用のTextMeshProUGUIコンポーネント
    public static event Action OnRestartRequested; // 再スタートが要求されたときのイベント
    public static event Action OnTitleRequested; // タイトル画面への遷移が要求されたときのイベント
    void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScore;
    }
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        if (scoreText == null) {
            ScoreManager.OnScoreChanged -= UpdateScore; // イベントの購読を解除
            Debug.LogError("TMP_Textコンポーネントがアタッチされていません。");
            return;
        }
    }

    // Update is called once per frame
    void UpdateScore(int newScore)
    {
        // スコアのUIを更新する処理
        Debug.Log("Score : " + newScore);
        scoreText.text = "最終結果\nスコア: " + newScore.ToString();
    }

    public void OnRestartButtonClicked()
    {
        OnRestartRequested?.Invoke(); // 再スタートが要求されたことを通知
    }
    public void OnTitleButtonClicked()
    {
        OnTitleRequested?.Invoke(); // タイトル画面への遷移が要求されたことを通知
    }
}
