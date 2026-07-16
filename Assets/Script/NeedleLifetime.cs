using UnityEngine;

public class NeedleLifetime : MonoBehaviour
{

    void OnEnable()
    {
        GameManager.Started += DestroyAfterLifetime;
    }
    // Spawnerから秒数を指定して初期化される
    public void Initialize(float timeToLive)
    {
        // 指定された秒数が経過したら、このゲームオブジェクトを自動的に破棄する
        Destroy(gameObject, timeToLive);
    }

    public void DestroyAfterLifetime()
    {
        // NeedleLifetimeスクリプトがアタッチされているオブジェクトを破棄する
        Destroy(gameObject);
    }
}
