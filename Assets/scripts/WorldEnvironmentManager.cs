using UnityEngine;

/// <summary>
/// World Environment Manager for Apex Drift.
/// Handles dynamic lighting, Day/Night intensity, and cinematic shadows.
/// </summary>
public class WorldEnvironmentManager : MonoBehaviour
{
    public static WorldEnvironmentManager Instance { get; private set; }

    [Header("Lighting")]
    public Light sunLight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.25f);
    public float nightIntensity = 0.2f;
    public float dayIntensity = 1.0f;

    [Header("Shadow Settings")]
    public LightShadows shadowType = LightShadows.Soft;
    public float shadowStrength = 1.0f;

    [Header("States")]
    public bool isNight = false;
    [Range(0, 1)] public float currentTimeOfDay = 0.5f; // 0 is night, 0.5 is day, 1 is night

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (sunLight == null)
            sunLight = RenderSettings.sun;
    }

    void Update()
    {
        ApplyLighting();
    }

    private void ApplyLighting()
    {
        if (sunLight == null) return;

        // Simple sun intensity based on time of day
        // 0.5 is peak noon (highest intensity)
        float intensityMult = Mathf.Sin(currentTimeOfDay * Mathf.PI);
        sunLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, intensityMult);
        sunLight.color = Color.Lerp(nightColor, dayColor, intensityMult);

        // Ambient light
        RenderSettings.ambientIntensity = sunLight.intensity;
        
        // Ensure high-quality shadows for cinematic look
        sunLight.shadows = shadowType;
        sunLight.shadowStrength = shadowStrength;
    }

    public void ToggleNightMode(bool night)
    {
        isNight = night;
        currentTimeOfDay = night ? 0.05f : 0.5f;
    }
}
