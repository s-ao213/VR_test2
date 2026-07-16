using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class EyeTrackingConsole : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("EyeTrackingConsole started");
    }

    [Header("Console表示間隔（秒）")]
    [SerializeField, Min(0.1f)]
    private float logInterval = 0.5f;

    private readonly List<InputDevice> eyeDevices = new();

    private InputDevice eyeDevice;
    private float nextSearchTime;
    private float nextLogTime;

    private void Update()
    {
        if (!eyeDevice.isValid)
        {
            FindEyeTracker();
            return;
        }

        ReadEyeGaze();
    }

    private void FindEyeTracker()
    {
        // 毎フレーム検索せず、1秒ごとに検索
        if (Time.unscaledTime < nextSearchTime)
        {
            return;
        }

        nextSearchTime = Time.unscaledTime + 1.0f;

        eyeDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.EyeTracking,
            eyeDevices
        );

        if (eyeDevices.Count > 0)
        {
            eyeDevice = eyeDevices[0];
            Debug.Log($"Eye Tracker detected: {eyeDevice.name}");
        }
        else
        {
            Debug.LogWarning(
                "Eye Trackerが見つかりません。SR_Runtimeとキャリブレーションを確認してください。"
            );
        }
    }

    private void ReadEyeGaze()
    {
        bool trackedResult = eyeDevice.TryGetFeatureValue(
            CommonUsages.isTracked,
            out bool isTracked
        );

        bool positionResult = eyeDevice.TryGetFeatureValue(
            EyeTrackingUsages.gazePosition,
            out Vector3 gazePosition
        );

        bool rotationResult = eyeDevice.TryGetFeatureValue(
            EyeTrackingUsages.gazeRotation,
            out Quaternion gazeRotation
        );

        if (!trackedResult ||
            !isTracked ||
            !positionResult ||
            !rotationResult)
        {
            LogUnavailable();
            return;
        }

        // Console出力の負荷を抑える
        if (Time.unscaledTime < nextLogTime)
        {
            return;
        }

        nextLogTime = Time.unscaledTime + logInterval;

        Vector3 gazeDirection =
            gazeRotation * Vector3.forward;

        string horizontalDirection;

        if (gazeDirection.x < -0.1f)
        {
            horizontalDirection = "LEFT";
        }
        else if (gazeDirection.x > 0.1f)
        {
            horizontalDirection = "RIGHT";
        }
        else
        {
            horizontalDirection = "CENTER";
        }

        Debug.Log(
            $"[Eye Gaze]\n" +
            $"Tracked: {isTracked}\n" +
            $"Position: {gazePosition:F4}\n" +
            $"Direction: {gazeDirection:F4}\n" +
            $"Rotation: {gazeRotation.eulerAngles:F2}\n" +
            $"Look: {horizontalDirection}"
        );
    }

    private void LogUnavailable()
    {
        if (Time.unscaledTime < nextLogTime)
        {
            return;
        }

        nextLogTime = Time.unscaledTime + 2.0f;

        Debug.LogWarning(
            "視線データを取得できません。HMDを装着してキャリブレーションを確認してください。"
        );
    }
}


