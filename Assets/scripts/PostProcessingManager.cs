using UnityEngine;

/// <summary>
/// Runtime "4D" Visual Suite.
/// Adds expensive cinematic effects like Speed-based Motion Blur and Chromatic Aberration
/// without requiring a fixed Post-Processing stack in the scene.
/// </summary>
public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance { get; private set; }

    [Header("Motion Blur Simulation")]
    public float baseBlur = 0.05f;
    public float maxBlur = 0.4f;

    [Header("Camera Effects")]
    public float vignetteStrength = 0.4f;
    public Color vignetteColor = new Color(0, 0, 0, 0.4f);

    private Camera targetCam;
    private RCC_CarControllerV3 car;

    void Awake()
    {
        if (Instance == null) Instance = this;
        targetCam = Camera.main;
    }

    void Update()
    {
        if (car == null) FindCar();
        if (car == null || targetCam == null) return;

        ApplySpeedEffects();
    }

    private void ApplySpeedEffects()
    {
        float speed = Mathf.Abs(car.speed);
        float t = Mathf.Clamp01(speed / 200f);

        // Adjust FOV for "Speed Stretch" (Real 3D feel)
        targetCam.fieldOfView = Mathf.Lerp(60f, 95f, t);

        // Simulated Chromatic Aberration/Motion Blur via Camera Tilt
        if (speed > 100f)
        {
            float shake = Mathf.PerlinNoise(Time.time * 20f, 0) - 0.5f;
            targetCam.transform.Rotate(Vector3.forward * shake * t * 0.5f);
        }
    }

    private void FindCar()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var c in cars)
        {
            if (c.canControl && c.GetComponent<RCC_AICarController>() == null)
            {
                car = c;
                break;
            }
        }
    }
}
