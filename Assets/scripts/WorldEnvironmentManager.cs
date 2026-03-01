using UnityEngine;

/// <summary>
/// World Environment Manager for Apex Drift.
/// Handles dynamic lighting, Day/Night intensity, and cinematic shadows.
/// </summary>
public class WorldEnvironmentManager : MonoBehaviour
{
    public static WorldEnvironmentManager Instance { get; private set; }

    public enum WeatherState { Clear, Overcast, Foggy, Rainy }

    [Header("Lighting")]
    public Light sunLight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.25f);
    public float nightIntensity = 0.2f;
    public float dayIntensity = 1.0f;

    [Header("Shadow & Backdrop")]
    public LightShadows shadowType = LightShadows.Soft;
    [Range(0,1)] public float shadowNormalBias = 0.05f;
    public float shadowStrength = 1.0f;

    [Header("Weather Settings")]
    public WeatherState currentWeather = WeatherState.Clear;
    public float fogDensityDay = 0.01f;
    public float fogDensityNight = 0.03f;
    public Color rainFogColor = new Color(0.4f, 0.4f, 0.45f);

    [Header("States")]
    public bool isNight = false;
    public bool autoCycle = true;
    public float cycleSpeed = 0.01f;
    [Range(0, 1)] public float currentTimeOfDay = 0.5f; 

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

        // Global shadow quality for "human have shadow" requirement
        QualitySettings.shadowDistance = 150f;
        QualitySettings.shadowCascades = 4;
    }

    void Update()
    {
        if (autoCycle)
        {
            currentTimeOfDay += Time.deltaTime * cycleSpeed;
            if (currentTimeOfDay > 1) currentTimeOfDay = 0;
        }

        ApplyLighting();
        ApplyWeather();
    }

    private void ApplyLighting()
    {
        if (sunLight == null) return;

        float intensityMult = Mathf.Sin(currentTimeOfDay * Mathf.PI);
        sunLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, intensityMult);
        sunLight.color = Color.Lerp(nightColor, dayColor, intensityMult);

        RenderSettings.ambientIntensity = sunLight.intensity;
        
        sunLight.shadows = shadowType;
        sunLight.shadowStrength = shadowStrength;
        sunLight.shadowNormalBias = shadowNormalBias;

        // Backdrop/Skybox exposure sync
        RenderSettings.skybox.SetFloat("_Exposure", sunLight.intensity);
    }

    private void ApplyWeather()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        switch (currentWeather)
        {
            case WeatherState.Clear:
                RenderSettings.fogDensity = isNight ? fogDensityNight : fogDensityDay;
                RenderSettings.fogColor = RenderSettings.ambientLight;
                break;
            case WeatherState.Foggy:
                RenderSettings.fogDensity = 0.08f;
                RenderSettings.fogColor = Color.gray;
                break;
            case WeatherState.Rainy:
                RenderSettings.fogDensity = 0.05f;
                RenderSettings.fogColor = rainFogColor;
                // Hook for rain particle system activation would go here
                break;
        }
    }
}

    public void ToggleNightMode(bool night)
    {
        isNight = night;
        currentTimeOfDay = night ? 0.05f : 0.5f;
    }
}
