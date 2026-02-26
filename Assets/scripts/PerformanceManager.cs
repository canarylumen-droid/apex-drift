using UnityEngine;

/// <summary>
/// Performance Manager for Apex Drift: 3D Racing.
/// Optimizes for smooth gameplay on low-end Android devices.
/// Auto-detects device capability and adjusts settings.
/// Attach to a persistent GameObject (via GameBootstrap).
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    public static PerformanceManager Instance { get; private set; }

    public enum QualityTier { Low, Medium, High }
    public QualityTier CurrentTier { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        DetectAndOptimize();
    }

    void DetectAndOptimize()
    {
        int ramMB = SystemInfo.systemMemorySize;
        int gpuMemMB = SystemInfo.graphicsMemorySize;
        int cpuCores = SystemInfo.processorCount;

        Debug.Log("[Performance] RAM: " + ramMB + "MB, GPU: " + gpuMemMB + "MB, Cores: " + cpuCores);

        // Classify device
        if (ramMB < 2048 || gpuMemMB < 512 || cpuCores <= 2)
        {
            CurrentTier = QualityTier.Low;
            ApplyLowSettings();
        }
        else if (ramMB < 4096 || gpuMemMB < 1024 || cpuCores <= 4)
        {
            CurrentTier = QualityTier.Medium;
            ApplyMediumSettings();
        }
        else
        {
            CurrentTier = QualityTier.High;
            ApplyHighSettings();
        }

        // Universal optimizations (all devices)
        ApplyUniversalOptimizations();

        Debug.Log("[Performance] Quality tier set to: " + CurrentTier);
    }

    // ===== LOW-END (1-2GB RAM, old phones) =====
    void ApplyLowSettings()
    {
        QualitySettings.SetQualityLevel(0, true); // Fastest
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.shadowDistance = 0;
        QualitySettings.antiAliasing = 0;
        QualitySettings.vSyncCount = 0;
        QualitySettings.lodBias = 0.5f;
        QualitySettings.masterTextureLimit = 2; // Quarter resolution textures
        QualitySettings.particleRaycastBudget = 4;
        QualitySettings.softParticles = false;
        Application.targetFrameRate = 30;

        // Reduce draw distance
        Camera.main.farClipPlane = 300f;

        // Disable fog
        RenderSettings.fog = false;

        Debug.Log("[Performance] LOW settings applied — 30fps, no shadows, reduced textures");
    }

    // ===== MEDIUM (2-4GB RAM) =====
    void ApplyMediumSettings()
    {
        QualitySettings.SetQualityLevel(2, true); // Good
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowDistance = 50;
        QualitySettings.antiAliasing = 0;
        QualitySettings.vSyncCount = 0;
        QualitySettings.lodBias = 1f;
        QualitySettings.masterTextureLimit = 1; // Half resolution textures
        QualitySettings.particleRaycastBudget = 16;
        QualitySettings.softParticles = false;
        Application.targetFrameRate = 45;

        Camera.main.farClipPlane = 500f;

        Debug.Log("[Performance] MEDIUM settings applied — 45fps, hard shadows");
    }

    // ===== HIGH (4GB+ RAM, modern phones) =====
    void ApplyHighSettings()
    {
        QualitySettings.SetQualityLevel(4, true); // Beautiful
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowDistance = 100;
        QualitySettings.antiAliasing = 2;
        QualitySettings.vSyncCount = 0;
        QualitySettings.lodBias = 1.5f;
        QualitySettings.masterTextureLimit = 0; // Full resolution
        QualitySettings.particleRaycastBudget = 64;
        QualitySettings.softParticles = true;
        Application.targetFrameRate = 60;

        Camera.main.farClipPlane = 1000f;

        Debug.Log("[Performance] HIGH settings applied — 60fps, full quality");
    }

    // ===== UNIVERSAL (all devices) =====
    void ApplyUniversalOptimizations()
    {
        // Disable screen dimming during gameplay
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Optimize physics
        Physics.defaultSolverIterations = 4; // Default is 6, lower = faster
        Physics.defaultSolverVelocityIterations = 1;

        // Limit physics update rate for smoother play
        Time.fixedDeltaTime = 0.02f; // 50fps physics

        // GC optimization — reduce garbage collection pauses
        QualitySettings.asyncUploadTimeSlice = 2;
        QualitySettings.asyncUploadBufferSize = 4;

        // Audio optimization
        AudioSettings.outputSampleRate = 24000; // Lower = less CPU load
    }

    // ===== MANUAL OVERRIDE =====

    /// <summary>
    /// Let player manually switch quality in settings menu
    /// </summary>
    public void SetQuality(QualityTier tier)
    {
        CurrentTier = tier;
        switch (tier)
        {
            case QualityTier.Low: ApplyLowSettings(); break;
            case QualityTier.Medium: ApplyMediumSettings(); break;
            case QualityTier.High: ApplyHighSettings(); break;
        }
        ApplyUniversalOptimizations();

        // Save preference
        PlayerPrefs.SetInt("quality_tier", (int)tier);
        PlayerPrefs.Save();
    }

    void Start()
    {
        // Load saved preference if exists
        if (PlayerPrefs.HasKey("quality_tier"))
        {
            QualityTier saved = (QualityTier)PlayerPrefs.GetInt("quality_tier");
            if (saved != CurrentTier)
                SetQuality(saved);
        }
    }
}
