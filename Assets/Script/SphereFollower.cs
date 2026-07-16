using UnityEngine;

public class SphereFollower : MonoBehaviour
{
    [Header("追従設定")]
    [Tooltip("追いかけるターゲット（目的地となる空のオブジェクト等）")]
    public Transform target;
    [Tooltip("このタグが付いている子オブジェクトのトリガーに触れたら止まる")]
    public string targetChildTag = "StopArea";
    [Tooltip("移動スピード")]
    public float moveSpeed = 5f;

    // トリガー内にいるかどうかのフラグ
    private bool isInsideTrigger = false;

    // 設定キャッシュ用変数
    private Vector3 center = Vector3.zero;
    private float radius = 10f;

    void Start()
    {
        if (PlanetManager.Instance != null)
        {
            center = PlanetManager.Instance.sphereCenter;
            radius = PlanetManager.Instance.sphereRadius;
        }
    }

    void Update()
    {
        if (target != null && !isInsideTrigger)
        {
            MoveOnSphere();
        }
        else if (target == null)
        {
            Debug.LogWarning($"【注意】{gameObject.name} の SphereFollower に『Target』が設定されていません！インスペクタから追従するオブジェクトをセットしてください。");
        }
    }

    private void MoveOnSphere()
    {
        Vector3 currentDir = (transform.position - center).normalized;
        Vector3 targetDir = (target.position - center).normalized;

        float angularSpeed = 0f;
        if (radius > 0.001f)
        {
            angularSpeed = (moveSpeed / radius) * Time.deltaTime;
        }

        Vector3 nextDir = Vector3.RotateTowards(currentDir, targetDir, angularSpeed, 0f);
        Vector3 nextPosition = center + nextDir * radius;
        
        Vector3 forwardDir = (nextPosition - transform.position).normalized;
        transform.position = nextPosition;

        if (forwardDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forwardDir, nextDir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ぶつかった相手が「指定したタグ」を持ち、かつ「ターゲットの子(または孫)オブジェクト」であるか判定
        if (other.CompareTag(targetChildTag) && other.transform.IsChildOf(target))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // トリガーエリアから出たら追従を再開する
        if (other.CompareTag(targetChildTag) && other.transform.IsChildOf(target))
        {
            isInsideTrigger = false;
        }
    }
}
