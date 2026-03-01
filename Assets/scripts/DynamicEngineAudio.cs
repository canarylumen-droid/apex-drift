using UnityEngine;

/// <summary>
/// Dynamic Engine Audio script for Apex Drift.
/// Connects the car's RPM and throttle data from RCC_CarControllerV3
/// to the CinematicAudioManager to create realistic engine sounds.
/// </summary>
[RequireComponent(typeof(RCC_CarControllerV3))]
public class DynamicEngineAudio : MonoBehaviour
{
    public enum CarProfile { Standard, Racing, Supercar, BMW_i8, Lamborghini, Audi_R8, Dodge_Viper, Aventador }
    
    private RCC_CarControllerV3 carController;
    private AudioSource engineSource;

    [Header("Car Identity")]
    public CarProfile carProfile = CarProfile.Standard;
    public string carName = "Default";

    [Header("Engine Clips")]
    public AudioClip engineIdle;
    public AudioClip engineHigh;

    [Header("Mixing Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 2.2f;
    public float maxRPM = 8000f;
    
    [Header("Profile Modifiers")]
    public float loudnessBoost = 1.0f;
    public float pitchMultiplier = 1.0f;

    void Start()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        
        // Setup the local audio source for the engine
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.loop = true;
        engineSource.spatialBlend = 1.0f; 
        engineSource.playOnAwake = false;

        ApplyProfileStats();

        if (engineIdle != null)
        {
            engineSource.clip = engineIdle;
            engineSource.Play();
        }
    }

    private void ApplyProfileStats()
    {
        switch (carProfile)
        {
            case CarProfile.BMW_i8:
                loudnessBoost = 1.8f; 
                pitchMultiplier = 1.25f; // "Loudest" and high revs
                break;
            case CarProfile.Lamborghini:
            case CarProfile.Aventador:
                loudnessBoost = 1.6f;
                pitchMultiplier = 1.15f; 
                break;
            case CarProfile.Audi_R8:
                loudnessBoost = 1.5f;
                pitchMultiplier = 1.2f; // High-pitch V10 scream
                break;
            case CarProfile.Dodge_Viper:
                loudnessBoost = 1.7f;
                pitchMultiplier = 0.9f; // Low-end V10 grunt
                break;
            case CarProfile.Supercar:
                loudnessBoost = 1.4f;
                pitchMultiplier = 1.1f;
                break;
            case CarProfile.Racing:
                loudnessBoost = 1.5f;
                break;
        }
    }

    void Update()
    {
        if (carController == null || engineSource == null) return;

        float rpm = carController.engineRPM;
        float basePitch = CinematicAudioManager.Instance != null 
            ? CinematicAudioManager.Instance.GetDynamicPitch(rpm, maxRPM)
            : Mathf.Lerp(minPitch, maxPitch, rpm / maxRPM);

        engineSource.pitch = basePitch * pitchMultiplier;

        // Wise Mixing: Advanced Volume Curve
        float throttle = carController.throttleInput;
        float speed = carController.speed;
        
        // High speed wind resistance simulation: slightly compress volume at very high speeds
        float windComp = Mathf.Lerp(1.0f, 0.85f, speed / 250f);
        
        float currentTargetVolume = Mathf.Lerp(0.45f, 1.0f, Mathf.Abs(throttle));
        engineSource.volume = currentTargetVolume * loudnessBoost * windComp;

        // Handle deceleration "wise" reduction
        if (throttle <= 0 && speed > 5)
        {
            // Rev drop on coasting
            engineSource.pitch *= 0.92f; 
            engineSource.volume *= 0.8f;
        }
    }
}
