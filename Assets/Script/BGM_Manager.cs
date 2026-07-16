using UnityEngine;

public class BGM_Manager : MonoBehaviour
{
    AudioSource Audios;

    public AudioClip titleBGM;
    public AudioClip playingBGM;
    AudioSource BGM;

    void OnEnable(){
        GameManager.TitleRequested += BGM_Title;
        GameManager.Started += BGM_Playing;
    }
    void OnDisable()
    {
        GameManager.TitleRequested -= BGM_Title;
        GameManager.Started -= BGM_Playing;
    }

    void Awake()
    {
        Audios=this.GetComponent<AudioSource>();
        BGM=Audios;
        BGM.loop = true;
    }

    public void BGM_Title(){
        BGM.clip=titleBGM;
        BGM.Play();
    }

    public void BGM_Playing(){
        BGM.clip=playingBGM;
        BGM.Play();
    }

}
