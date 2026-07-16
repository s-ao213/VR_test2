using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class EyeTrackingDirection : MonoBehaviour
{
    [Header("中央と判定する角度")]
    [SerializeField, Range(1f, 30f)]
    private float centerAngle = 5f;

    [Header("Console表示間隔（秒）")]
    [SerializeField, Min(0.1f)]
    private float logInterval = 0.25f;

    private readonly List<InputDevice> eyeDevices = new();

    private InputDevice eyeDevice;
    private Transform headTransform;

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

        Debug.Log(
            $"[Eye Tracking] 開始 Camera={headTransform.name}"
        );
    }

    private void Update()
    {
        if (!eyeDevice.isValid)
        {
            FindEyeDevice();
            return;
        }

        ReadEyeDirection();
    }

    private void FindEyeDevice()
    {
        if (Time.unscaledTime < nextSearchTime)
        {
            return;
        }

        nextSearchTime = Time.unscaledTime + 1f;

        eyeDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.EyeTracking,
            eyeDevices
        );

        if (eyeDevices.Count == 0)
        {
            LogError(
                "Eye Trackerが見つかりません。"
            );

            return;
        }

        eyeDevice = eyeDevices[0];

        Debug.Log(
            $"[Eye Tracking] 検出成功 Device={eyeDevice.name}"
        );
    }

    private void ReadEyeDirection()
    {
        bool trackedAvailable =
            eyeDevice.TryGetFeatureValue(
                CommonUsages.isTracked,
                out bool tracked
            );

        bool rotationAvailable =
            eyeDevice.TryGetFeatureValue(
                EyeTrackingUsages.gazeRotation,
                out Quaternion gazeRotation
            );

        if (!trackedAvailable ||
            !tracked ||
            !rotationAvailable)
        {
            LogError(
                $"視線方向を取得できません。 " +
                $"trackedAvailable={trackedAvailable}, " +
                $"tracked={tracked}, " +
                $"rotationAvailable={rotationAvailable}"
            );

            return;
        }

        /*
         * 視線回転から頭部回転を差し引く。
         * これにより、頭ではなく眼球の左右方向を判定する。
         */
        Quaternion relativeRotation =
            Quaternion.Inverse(headTransform.localRotation) *
            gazeRotation;

        Vector3 eyeDirection =
            relativeRotation * Vector3.forward;

        float horizontalAngle =
            Mathf.Atan2(
                eyeDirection.x,
                eyeDirection.z
            ) * Mathf.Rad2Deg;

        string result;

        if (horizontalAngle < -centerAngle)
        {
            result = "左 / LEFT";
        }
        else if (horizontalAngle > centerAngle)
        {
            result = "右 / RIGHT";
        }
        else
        {
            result = "中央 / CENTER";
        }

        if (Time.unscaledTime < nextLogTime)
        {
            return;
        }

        nextLogTime =
            Time.unscaledTime + logInterval;

        Debug.Log(
            $"[視線] {result} | " +
            $"左右角度={horizontalAngle:F2}° | " +
            $"X={eyeDirection.x:F4} | " +
            $"Tracked={tracked}"
        );
    }

    private void LogError(string message)
    {
        if (Time.unscaledTime < nextErrorTime)
        {
            return;
        }

        nextErrorTime = Time.unscaledTime + 2f;

        Debug.LogWarning(
            $"[Eye Tracking] {message}"
        );
    }
}


