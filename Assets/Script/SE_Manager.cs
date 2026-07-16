using UnityEngine;

public class SE_Manager : MonoBehaviour
{
    AudioSource Audios;

    public AudioClip eatSE;
    public AudioClip levelUpSE;
    public AudioClip gameOver;
    AudioSource SE;

    void OnEnable(){
        Head.OnFoodEaten += SE_Eat;
        Head.OnNeedleHit += SE_GameOver;
    }
    void OnDisable()
    {
        Head.OnFoodEaten -= SE_Eat;
        Head.OnNeedleHit -= SE_GameOver;
    }

    void Start(){
        Audios=this.GetComponent<AudioSource>();
        SE=Audios;
    }

    public void SE_Eat(){
        SE.PlayOneShot(eatSE);
    }

    public void SE_LevelUp(){
        SE.PlayOneShot(levelUpSE);
    }

    public void SE_GameOver(){
        SE.PlayOneShot(gameOver);
    }
}
