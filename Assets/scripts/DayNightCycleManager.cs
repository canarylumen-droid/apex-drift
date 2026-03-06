using UnityEngine;

/// <summary>
/// Phase 9: Day/Night & Environmental Realism.
/// Rotates the sun, changes lighting, and toggles city lights every 5 minutes.
/// </summary>
public class DayNightCycleManager : MonoBehaviour
{
    public static DayNightCycleManager Instance { get; private set; }

    [Header("Cycle Settings")]
    public Light sun;
    public float cycleDurationMinutes = 5f;
    private float timeOfDay = 0.25f; // 0 to 1 (0.25 is dawn)

    [Header("City Lights")]
    public GameObject[] streetLights;
    public Material buildingWindowsMaterial;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (sun == null) sun = RenderSettings.sun;
    }

    void Update()
    {
        // 1. Advance Time
        timeOfDay += Time.deltaTime / (cycleDurationMinutes * 60f);
        if (timeOfDay >= 1f) timeOfDay = 0f;

        // 2. Rotate Sun
        sun.transform.localRotation = Quaternion.Euler((timeOfDay * 360f) - 90f, 170f, 0f);

        // 3. Change Intensity (Bright at noon, dark at night)
        float intensityMult = 0;
        if (timeOfDay <= 0.23f || timeOfDay >= 0.75f) intensityMult = 0; // Night
        else if (timeOfDay <= 0.25f) intensityMult = Mathf.InverseLerp(0.23f, 0.25f, timeOfDay); // Dawn
        else if (timeOfDay <= 0.73f) intensityMult = 1; // Day
        else intensityMult = Mathf.InverseLerp(0.75f, 0.73f, timeOfDay); // Dusk

        sun.intensity = intensityMult;

        // 4. Toggle Street Lights & Window Glow
        bool isNight = (timeOfDay < 0.25f || timeOfDay > 0.73f);
        ToggleCityLights(isNight);
    }

    private void ToggleCityLights(bool enabled)
    {
        // Window Glow logic
        if (buildingWindowsMaterial != null)
        {
            buildingWindowsMaterial.SetColor("_EmissionColor", enabled ? Color.white * 2f : Color.black);
        }

        // We search for lights if not assigned
        if (streetLights == null || streetLights.Length == 0)
        {
            streetLights = GameObject.FindGameObjectsWithTag("Streetlight");
        }

        foreach (var light in streetLights)
        {
            if (light != null) light.SetActive(enabled);
        }
    }
}
