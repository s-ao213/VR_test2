using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

/// <summary>
/// VIVE Pro Eyeの視線データを取得し、ゲーム操作用の GazeX / GazeY として公開する。
///
/// これまでマウスの Input.mousePosition を正規化して使っていた箇所を、
/// このコンポーネントの GazeX / GazeY に差し替えるだけで視線操作に置き換えられる。
///
///   GazeX : -1(左) 〜 0(正面) 〜 +1(右)
///   GazeY : -1(下) 〜 0(正面) 〜 +1(上)
///
/// マウス版と同じく「-1〜+1の連続値」なので、
/// 円柱フィールドの角度(θ)や高さ(y)への変換方法はマウス版と揃えられる。
/// </summary>
public class GazeXYProvider : MonoBehaviour
{
    [Header("中央と判定する角度（デバッグ用の左/中央/右判定にのみ使用）")]
    [SerializeField, Range(1f, 30f)]
    private float centerAngle = 5f;

    [Header("GazeX/GazeYの正規化に使う角度")]
    [Tooltip("この角度(度)で GazeX / GazeY が ±1.0 になる")]
    [SerializeField, Range(5f, 45f)]
    private float maxAngle = 25f;

    [Header("平滑化設定")]
    [Tooltip("大きいほど反応が速い、小さいほど滑らか")]
    [SerializeField, Range(1f, 20f)]
    private float smoothing = 8f;

    [Header("Console表示間隔（秒）")]
    [SerializeField, Min(0.1f)]
    private float logInterval = 0.25f;

    [SerializeField] private bool logToConsole = true;

    // ---- ゲームロジックが読む値（マウスXYの代わり） ----

    /// <summary>左右の視線位置。-1(左) 〜 +1(右)。マウスの正規化X相当。</summary>
    public float GazeX { get; private set; }

    /// <summary>上下の視線位置。-1(下) 〜 +1(上)。マウスの正規化Y相当。</summary>
    public float GazeY { get; private set; }

    /// <summary>視線トラッキングが有効かどうか</summary>
    public bool IsTracking { get; private set; }

    // ---- 内部状態 ----

    private readonly List<InputDevice> eyeDevices = new();
    private InputDevice eyeDevice;
    private Transform headTransform;

    private float smoothedHorizontalAngle;
    private float smoothedVerticalAngle;

    private float nextSearchTime;
    private float nextLogTime;
    private float nextErrorTime;

    private void Start()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "Main Cameraが見つかりません。" +
                "Main CameraのTagを「MainCamera」にしてください。"
            );

            enabled = false;
            return;
        }

        headTransform = mainCamera.transform;

        Debug.Log($"[GazeXYProvider] 開始 Camera={headTransform.name}");
    }

    private void Update()
    {
        if (!eyeDevice.isValid)
        {
            FindEyeDevice();
            IsTracking = false;
            // トラッキングが切れている間はXYを中央(0,0)に戻す（暴走防止）
            GazeX = 0f;
            GazeY = 0f;
            return;
        }

        ReadEyeDirection();
    }

    private void FindEyeDevice()
    {
        if (Time.unscaledTime < nextSearchTime) return;
        nextSearchTime = Time.unscaledTime + 1f;

        eyeDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.EyeTracking,
            eyeDevices
        );

        if (eyeDevices.Count == 0)
        {
            LogError("Eye Trackerが見つかりません。");
            return;
        }

        eyeDevice = eyeDevices[0];

        Debug.Log($"[GazeXYProvider] 検出成功 Device={eyeDevice.name}");
    }

    private void ReadEyeDirection()
    {
        bool trackedAvailable = eyeDevice.TryGetFeatureValue(
            CommonUsages.isTracked, out bool tracked
        );

        bool rotationAvailable = eyeDevice.TryGetFeatureValue(
            EyeTrackingUsages.gazeRotation, out Quaternion gazeRotation
        );

        if (!trackedAvailable || !tracked || !rotationAvailable)
        {
            IsTracking = false;

            LogError(
                $"視線方向を取得できません。 " +
                $"trackedAvailable={trackedAvailable}, " +
                $"tracked={tracked}, " +
                $"rotationAvailable={rotationAvailable}"
            );

            return;
        }

        IsTracking = true;

        // 視線回転から頭部回転を差し引き、頭を基準にした相対方向にする
        Quaternion relativeRotation =
            Quaternion.Inverse(headTransform.localRotation) * gazeRotation;

        Vector3 eyeDirection = relativeRotation * Vector3.forward;

        // 水平角度（左右）
        float rawHorizontalAngle =
            Mathf.Atan2(eyeDirection.x, eyeDirection.z) * Mathf.Rad2Deg;

        // 垂直角度（上下）。上を見たときGazeYが+になるよう符号を調整
        float rawVerticalAngle =
            -Mathf.Atan2(eyeDirection.y, eyeDirection.z) * Mathf.Rad2Deg;

        // 平滑化（サッケードによるガクつきを抑える）
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        smoothedHorizontalAngle = Mathf.Lerp(smoothedHorizontalAngle, rawHorizontalAngle, t);
        smoothedVerticalAngle = Mathf.Lerp(smoothedVerticalAngle, rawVerticalAngle, t);

        // -1〜+1に正規化（ここがマウスXYと同じ形の出力）
        GazeX = Mathf.Clamp(smoothedHorizontalAngle / maxAngle, -1f, 1f);
        GazeY = Mathf.Clamp(smoothedVerticalAngle / maxAngle, -1f, 1f);

        // 既存のデバッグ用：左/中央/右の3値判定はそのまま残す
        string result;
        if (rawHorizontalAngle < -centerAngle) result = "左 / LEFT";
        else if (rawHorizontalAngle > centerAngle) result = "右 / RIGHT";
        else result = "中央 / CENTER";

        if (logToConsole && Time.unscaledTime >= nextLogTime)
        {
            nextLogTime = Time.unscaledTime + logInterval;

            Debug.Log(
                $"[視線] {result} | " +
                $"GazeX={GazeX:F2} GazeY={GazeY:F2} | " +
                $"角度(H,V)=({rawHorizontalAngle:F1}, {rawVerticalAngle:F1}) | " +
                $"Tracked={tracked}"
            );
        }
    }

    private void LogError(string message)
    {
        if (Time.unscaledTime < nextErrorTime) return;
        nextErrorTime = Time.unscaledTime + 2f;

        Debug.LogWarning($"[Eye Tracking] {message}");
    }
}
