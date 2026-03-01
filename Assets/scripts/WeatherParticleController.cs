using UnityEngine;

/// <summary>
/// Controls weather-dependent particle effects like Rain and Fog clouds.
/// Hooks into WorldEnvironmentManager.
/// </summary>
public class WeatherParticleController : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem rainParticles;
    public ParticleSystem fogParticles;

    [Header("Settings")]
    public float transitionSpeed = 1f;

    void Update()
    {
        if (WorldEnvironmentManager.Instance == null) return;

        HandleRain();
        HandleFog();
    }

    private void HandleRain()
    {
        if (rainParticles == null) return;

        bool shouldRain = WorldEnvironmentManager.Instance.currentWeather == WorldEnvironmentManager.WeatherState.Rainy;
        var emission = rainParticles.emission;

        if (shouldRain && !rainParticles.isPlaying)
            rainParticles.Play();
        else if (!shouldRain && rainParticles.isPlaying)
            rainParticles.Stop();
    }

    private void HandleFog()
    {
        if (fogParticles == null) return;

        bool shouldFog = WorldEnvironmentManager.Instance.currentWeather == WorldEnvironmentManager.WeatherState.Foggy ||
                         WorldEnvironmentManager.Instance.currentWeather == WorldEnvironmentManager.WeatherState.Overcast;

        var emission = fogParticles.emission;
        
        if (shouldFog && !fogParticles.isPlaying)
            fogParticles.Play();
        else if (!shouldFog && fogParticles.isPlaying)
            fogParticles.Stop();
    }
}
