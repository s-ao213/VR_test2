using UnityEngine;
using UnityEngine.InputSystem;



public class MouseFollow : MonoBehaviour
{
    // カメラからオブジェクトまでの距離（Z軸）
    public float distance = 10f;

    public Vector3 sphereCenter = Vector3.zero;
    [Tooltip("球の半径")]
    public float sphereRadius = 10f;

    [Tooltip("追従するスピード")]
    public float moveSpeed = 5f;

    bool isMoving = true; // 移動を有効にするかどうかのフラグ

    void OnEnable()
    {
        GameManager.Finished += StopMoving;
    }
    void OnDisable()
    {
        GameManager.Finished -= StopMoving;
    }

    void Update()
    {
        Camera cam = Camera.main;
        if (cam == null|| !isMoving)
        {
            // Main Camera が設定されていないと動作しない
            return;
        }

        // マウスのスクリーン座標を取得 (X, Yは画面上のピクセル座標)
        Vector3 mousePosition;

        // 新しい Input System が有効であれば優先して使用
        if (Mouse.current != null)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            mousePosition = new Vector3(pos.x, pos.y, 0f);
        }
        else
        {
            // 旧 Input を使う場合は例外が出る可能性があるため保護
            try
            {
                mousePosition = Input.mousePosition;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        // 深度は現在のオブジェクトのスクリーンZを使う方が安定する
        mousePosition.z = cam.WorldToScreenPoint(transform.position).z;

        // スクリーン座標からワールド座標に変換
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        // オブジェクトの座標をワールド座標に移動
        MoveOnSphere(worldPosition);
    }

    void StopMoving()
    {
        isMoving = false;
    }

    private void MoveOnSphere(Vector3 worldPosition)
    {
        // 1. ターゲットの方向へ直進させる（この時点では球にめり込むか浮く）
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, worldPosition, moveSpeed * Time.deltaTime);

        // 2. 現在地から球の中心へ向かうベクトル（法線）を計算
        Vector3 directionFromCenter = (nextPosition - sphereCenter).normalized;

        // 3. 球の表面（半径の距離）に座標を補正して張り付かせる
        transform.position = sphereCenter + directionFromCenter * sphereRadius;

        // 4. ターゲットの方向を向く（上方向は球の中心からの法線とする）
        Vector3 forward = (worldPosition - transform.position).normalized;
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward, directionFromCenter);
        }
    }
}
