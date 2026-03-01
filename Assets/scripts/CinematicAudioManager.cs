using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cinematic Audio Manager for Apex Drift.
/// Handles dynamic car engine sounds, ambient city noise, and cinematic audio events.
/// This is a persistent singleton created by GameBootstrap.
/// </summary>
public class CinematicAudioManager : MonoBehaviour
{
    public static CinematicAudioManager Instance { get; private set; }

    [Header("Ambient Settings")]
    public float ambientVolume = 0.5f;
    private AudioSource ambientSource;

    [Header("Car Sound Settings")]
    public float engineBasePitch = 0.8f;
    public float engineMaxPitch = 2.0f;

    // Internal registry of loaded clips
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSystem()
    {
        // Setup ambient city noise source
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.volume = ambientVolume;
        ambientSource.spatialBlend = 0; // 2D background

        Debug.Log("[CinematicAudioManager] System initialized.");
    }

    /// <summary>
    /// Loads all sounds from the /sounds directory if possible, 
    /// or expects them to be assigned in the inspector in a real Unity workflow.
    /// For this automated setup, we'll provide hooks for the scripts to call.
    /// </summary>
    public void PlayCinematicSound(string soundName, float volume = 1.0f)
    {
        // Placeholder for dynamic sound triggering
        Debug.Log("[CinematicAudioManager] Playing sound: " + soundName);
    }

    /// <summary>
    /// Starts the background city ambience
    /// </summary>
    public void StartCityAmbience(AudioClip cityClip)
    {
        if (cityClip != null)
        {
            ambientSource.clip = cityClip;
            ambientSource.Play();
        }
    }

    /// <summary>
    /// Logic to calculate dynamic pitch based on car RPM
    /// </summary>
    public float GetDynamicPitch(float currentRPM, float maxRPM)
    {
        float ratio = currentRPM / maxRPM;
        return Mathf.Lerp(engineBasePitch, engineMaxPitch, ratio);
    }
}
