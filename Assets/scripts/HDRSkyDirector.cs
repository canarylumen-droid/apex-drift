using UnityEngine;

/// <summary>
/// Phase 13: HDR Sky Director.
/// Manages the night sky visuals: stars, moon, and dynamic clouds.
/// Links with DayNightCycleManager for seamless transitions.
/// </summary>
public class HDRSkyDirector : MonoBehaviour
{
    public static HDRSkyDirector Instance { get; private set; }

    [Header("Sky Settings")]
    public Material skyboxMaterial;
    public GameObject starPrefab;
    public int starCount = 500;

    private List<GameObject> stars = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        GenerateStarField();
    }

    private void GenerateStarField()
    {
        for (int i = 0; i < starCount; i++)
        {
            Vector3 randomDir = Random.onUnitSphere;
            if (randomDir.y < 0) randomDir.y *= -1; // Only upper hemisphere

            GameObject star = Instantiate(starPrefab, transform.position + randomDir * 1000f, Quaternion.identity);
            star.transform.SetParent(transform);
            star.transform.localScale = Vector3.one * Random.Range(1f, 3f);
            stars.Add(star);
        }
        ToggleStars(false);
    }

    void Update()
    {
        // Link to DayNightCycleManager time
        if (DayNightCycleManager.Instance != null)
        {
            // Simple logic: if intensity is low, show stars
            bool showStars = RenderSettings.sun.intensity < 0.2f;
            ToggleStars(showStars);

            // Update Skybox Exposure
            if (skyboxMaterial != null)
            {
                float exposure = Mathf.Lerp(0.2f, 1.2f, RenderSettings.sun.intensity);
                skyboxMaterial.SetFloat("_Exposure", exposure);
            }
        }
    }

    public void ToggleStars(bool enabled)
    {
        foreach (var star in stars)
        {
            if (star != null) star.SetActive(enabled);
        }
    }
}
