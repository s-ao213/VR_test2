using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbitTest : MonoBehaviour
{
    [Header("回転設定")]
    [Tooltip("星（球）の中心座標")]
    public Vector3 targetCenter = Vector3.zero;
    [Tooltip("回転スピード")]
    public float rotateSpeed = 90f;

    void Update()
    {
        if (Keyboard.current == null) return;

        float h = 0f;
        float v = 0f;

        // 新しいInput Systemでのキー判定（WASD または 矢印キー）
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v = 1f;

        // 左右の入力：Y軸（全体の上方向）を基準に公転
        if (h != 0f)
        {
            transform.RotateAround(targetCenter, Vector3.up, -h * rotateSpeed * Time.deltaTime);
        }

        // 上下の入力：カメラ自身のローカルX軸（右方向）を基準に公転
        if (v != 0f)
        {
            transform.RotateAround(targetCenter, transform.right, v * rotateSpeed * Time.deltaTime);
        }

        // 常に星の中心を見つめ続ける
        transform.LookAt(targetCenter);
    }
}
