using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    private TMP_Text scoreText; // スコア表示用のTextMeshProUGUIコンポーネント
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        scoreText.text = "Score: " + newScore.ToString();
    }
}
