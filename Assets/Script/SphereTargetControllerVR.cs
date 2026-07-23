using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class SphereTargetControllerVR : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] private float sphereRadius = 10f;
    [SerializeField] private Vector3 sphereCenter = Vector3.zero;

    [Header("Smoothing")]
    [Tooltip("0なら平滑化なし。20～30程度がおすすめ")]
    [SerializeField] private float smoothing = 25f;

    private Camera mainCamera;
    private Transform headTransform;

    private readonly List<InputDevice> eyeDevices = new();
    private InputDevice eyeDevice;

    private Quaternion smoothedRotation;
    private bool initialized;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Cameraが見つかりません");
            enabled = false;
            return;
        }

        headTransform = mainCamera.transform;

        if (PlanetManager.Instance != null)
        {
            sphereCenter = PlanetManager.Instance.sphereCenter;
            sphereRadius = PlanetManager.Instance.sphereRadius;
        }

        FindEyeDevice();
    }

    void LateUpdate()
    {
        if (!eyeDevice.isValid)
        {
            FindEyeDevice();
            return;
        }

        if (!eyeDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) || !tracked)
            return;

        if (!eyeDevice.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion gazeRotation))
            return;

        Quaternion relativeRotation =
            Quaternion.Inverse(headTransform.rotation) * gazeRotation;

        if (!initialized)
        {
            smoothedRotation = relativeRotation;
            initialized = true;
        }

        if (smoothing > 0)
        {
            float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            smoothedRotation = Quaternion.Slerp(smoothedRotation, relativeRotation, t);
        }
        else
        {
            smoothedRotation = relativeRotation;
        }

        Ray ray = new Ray(
            headTransform.position,
            headTransform.rotation * (smoothedRotation * Vector3.forward)
        );

        if (RaySphereIntersect(ray, sphereCenter, sphereRadius, out Vector3 hit))
        {
            transform.position = hit;
            transform.rotation = Quaternion.LookRotation(
                (hit - sphereCenter).normalized,
                Vector3.up
            );
        }
    }

    void FindEyeDevice()
    {
        eyeDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.EyeTracking,
            eyeDevices
        );

        if (eyeDevices.Count > 0)
        {
            eyeDevice = eyeDevices[0];
            Debug.Log("Eye Tracker : " + eyeDevice.name);
        }
    }

    bool RaySphereIntersect(
        Ray ray,
        Vector3 center,
        float radius,
        out Vector3 hit)
    {
        Vector3 oc = ray.origin - center;

        float a = Vector3.Dot(ray.direction, ray.direction);
        float b = 2f * Vector3.Dot(oc, ray.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;

        float d = b * b - 4f * a * c;

        if (d < 0)
        {
            hit = Vector3.zero;
            return false;
        }

        float t = (-b + Mathf.Sqrt(d)) / (2f * a);

        hit = ray.origin + ray.direction * t;
        return true;
    }
}