using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceGuide : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> voiceClips;
    [SerializeField] private AudioClip gameOverClip;

    private  Coroutine PlayCoroutine;

    void OnEnable()
    {
        GameManager.Finished += GameOverVoice;
    }

    private void Start()
    {
        PlayCoroutine = StartCoroutine(PlayVoicesSequentially());
    }

    private IEnumerator PlayVoicesSequentially()
    {
        // リスト内の音声を順番に処理
        foreach (var clip in voiceClips)
        {
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();

                // 音声の再生が終わるまで待機する
                yield return new WaitForSeconds(clip.length);
            }
        }
    }
    private void GameOverVoice()
    {
        if (PlayCoroutine != null)
        {
            StopCoroutine(PlayCoroutine);
        }
        audioSource.Stop();
        audioSource.clip = gameOverClip;
        audioSource.Play();
    }
}