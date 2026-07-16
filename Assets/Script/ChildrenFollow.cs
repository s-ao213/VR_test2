using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // トリガー判定のためにRigidbodyを自動アタッチ
public class ChildrenFollow : MonoBehaviour
{
    [Header("追従設定")]
    [Tooltip("追いかける対象のオブジェクト（前のヘビの胴体など）")]
    public Transform target;
    
    [Tooltip("このタグが付いている子オブジェクトのトリガーに触れたら止まる")]
    public string targetChildTag = "StopArea";
    
    [Tooltip("追従するスピード")]
    public float moveSpeed = 5f;

    // トリガー内にいるかどうかのフラグ
    private bool isInsideTrigger = false;
    private Rigidbody rb;

    // 設定キャッシュ用変数
    private Vector3 center = Vector3.zero;
    private float radius = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 物理演算で勝手に吹き飛ばないようにKinematicをオンにしておく
        rb.isKinematic = true; 

        if (PlanetManager.Instance != null)
        {
            center = PlanetManager.Instance.sphereCenter;
            radius = PlanetManager.Instance.sphereRadius;
        }
    }

    void Update()
    {
        // ターゲットが存在し、かつストップ用のトリガー内にいない時だけ移動する
        if (target != null && !isInsideTrigger)
        {
            MoveOnSphere(); // 球面に沿って追従する
        }
    }

    /// <summary>
    /// 通常の3D空間でターゲットを追従する処理（テスト用）
    /// </summary>
    private void MoveBasic()
    {
        // 1. ターゲットの現在の座標へ向かって直線的に移動
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // 2. ターゲットの方向を向く
        Vector3 forward = (target.position - transform.position).normalized;
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    /// <summary>
    /// 球面に沿ってターゲットを追従する処理
    /// </summary>
    private void MoveOnSphere()
    {
        // 1. ターゲットの方向へ直進させる（この時点では球にめり込むか浮く）
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // 2. 現在地から球の中心へ向かうベクトル（法線）を計算
        Vector3 directionFromCenter = (nextPosition - center).normalized;

        // 3. 球の表面（半径の距離）に座標を補正して張り付かせる
        transform.position = center + directionFromCenter * radius;

        // 4. ターゲットの方向を向く（上方向は球の中心からの法線とする）
        Vector3 forward = (target.position - transform.position).normalized;
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward, directionFromCenter);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null) return; // ★ターゲットが空の時のエラーを完全回避
        
        // ぶつかった相手が「指定したタグ」を持ち、かつ「ターゲットの子(または孫)オブジェクト」であるか判定
        if (other.CompareTag(targetChildTag) && other.transform.IsChildOf(target))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target == null) return; // ★ターゲットが空の時のエラーを完全回避

        // トリガーエリアから出たら追従を再開する
        if (other.CompareTag(targetChildTag) && other.transform.IsChildOf(target))
        {
            isInsideTrigger = false;
        }
    }
}
