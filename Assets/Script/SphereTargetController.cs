 using UnityEngine;
using UnityEngine.InputSystem;

public class SphereTargetController : MonoBehaviour
{
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
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 mousePos = Mouse.current != null 
            ? Mouse.current.position.ReadValue() 
            : (Vector2)Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(mousePos);

        if (RaySphereIntersect(ray, center, radius, out Vector3 hitPoint))
        {
            transform.position = hitPoint;
            transform.rotation = Quaternion.LookRotation((hitPoint - center).normalized, Vector3.up);
        }
    }

    private bool RaySphereIntersect(Ray ray, Vector3 sphereCenter, float sphereRadius, out Vector3 hitPoint)
    {
        Vector3 oc = ray.origin - sphereCenter;
        float a = Vector3.Dot(ray.direction, ray.direction);
        float b = 2f * Vector3.Dot(oc, ray.direction);
        float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0)
        {
            hitPoint = Vector3.zero;
            return false;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t = (-b + sqrtDisc) / (2f * a); // 内側視点なので奥側の交点を採用

        hitPoint = ray.origin + ray.direction * t;
        return true;
    }
}
