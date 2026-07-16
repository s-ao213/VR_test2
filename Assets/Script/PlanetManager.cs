using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    // どこからでもアクセスできるシングルトンインスタンス
    public static PlanetManager Instance { get; private set; }

    [Header("星（球）の共通設定")]
    [Tooltip("星の中心座標")]
    public Vector3 sphereCenter = Vector3.zero;
    [Tooltip("星の半径")]
    public float sphereRadius = 10f;

    void Awake()
    {
        // インスタンスが未設定なら自分を登録、既にいれば自分を破棄（重複防止）
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
