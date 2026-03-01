using UnityEngine;

/// <summary>
/// Dynamic Engine Audio script for Apex Drift.
/// Connects the car's RPM and throttle data from RCC_CarControllerV3
/// to the CinematicAudioManager to create realistic engine sounds.
/// </summary>
[RequireComponent(typeof(RCC_CarControllerV3))]
public class DynamicEngineAudio : MonoBehaviour
{
    public enum CarProfile { Standard, Racing, Supercar, BMW_i8 }
    
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
        engineSource.spatialBlend = 1.0f; // 3D sound
        engineSource.playOnAwake = false;

        // Apply profile-based tweaks
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
                loudnessBoost = 1.6f; // "Loudest" as requested
                pitchMultiplier = 1.2f; // "Rev hire"
                break;
            case CarProfile.Supercar:
                loudnessBoost = 1.3f;
                pitchMultiplier = 1.1f;
                break;
            case CarProfile.Racing:
                loudnessBoost = 1.4f;
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

        // Advanced Volume Logic: Dynamic mixing for acceleration vs deceleration
        float throttle = carController.throttleInput;
        
        // Use a higher floor for racing cars, but allow lower volume on deceleration
        float currentTargetVolume = Mathf.Lerp(0.4f, 1.0f, Mathf.Abs(throttle));
        
        // Apply the loudness boost
        engineSource.volume = currentTargetVolume * loudnessBoost;

        // Handle deceleration "whine" or "drop"
        if (throttle <= 0 && carController.speed > 10)
        {
            // Slightly drop pitch or add a filter effect if we had more complex DSP setup
            engineSource.pitch *= 0.95f; 
        }
    }
}
