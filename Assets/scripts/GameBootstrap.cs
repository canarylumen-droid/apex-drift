using UnityEngine;

/// <summary>
/// Game Bootstrap for Apex Drift: 3D Racing.
/// Creates all persistent managers on game start.
/// Attach this to an empty GameObject in the FIRST scene (Main Menu).
/// This ensures all singleton managers exist before anything else runs.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("Manager Prefabs (optional — will auto-create if null)")]
    public GameObject currencyManagerPrefab;
    public GameObject carUpgradeDataPrefab;
    public GameObject adManagerPrefab;

    private static bool initialized = false;

    void Awake()
    {
        if (initialized) return;
        initialized = true;

        Debug.Log("=== " + GameConfig.GAME_NAME + " v" + GameConfig.VERSION + " ===");
        Debug.Log("=== Package: " + GameConfig.PACKAGE_NAME + " ===");
        Debug.Log("=== Developer: " + GameConfig.DEVELOPER + " ===");

        // Create persistent managers
        CreateManager<CurrencyManager>("CurrencyManager");
        CreateManager<CarUpgradeData>("CarUpgradeData");
        CreateManager<AdManager>("AdManager");
        CreateManager<BannerAdController>("BannerAdController");
        CreateManager<InterstitialAdController>("InterstitialAdController");
        CreateManager<RewardedAdController>("RewardedAdController");
        CreateManager<CinematicAudioManager>("CinematicAudioManager");
        CreateManager<WorldEnvironmentManager>("WorldEnvironmentManager");
        CreateManager<CinematicInteractionManager>("CinematicInteractionManager");
        CreateManager<CinematicHUD>("CinematicHUD");
        CreateManager<PoliceChaseManager>("PoliceChaseManager");
        CreateManager<RuntimeWorldBuilder>("RuntimeWorldBuilder");
        CreateManager<PremiumHUD>("PremiumHUD");
        CreateManager<RealisticModelManager>("RealisticModelManager");
        CreateManager<CinematicCrashEngine>("CinematicCrashEngine");
        CreateManager<HighlifeSocialManager>("HighlifeSocialManager");
        CreateManager<MissionController>("MissionController");
        CreateManager<MobileInteractionUI>("MobileInteractionUI");
        CreateManager<TrafficManager>("TrafficManager");
        CreateManager<GarageManager>("GarageManager");
        CreateManager<MissionSelectUI>("MissionSelectUI");
        CreateManager<SocialEventController>("SocialEventController");
        CreateManager<CrowdManager>("CrowdManager");
        CreateManager<QualityManager>("QualityManager");
        CreateManager<PhysicsMaster>("PhysicsMaster");
        CreateManager<PoliceAI>("PoliceAI");
        CreateManager<HelicopterPursuit>("HelicopterPursuit");
        CreateManager<ReputationSystem>("ReputationSystem");
        CreateManager<WantedSystem>("WantedSystem");
        CreateManager<PostProcessingManager>("PostProcessingManager");
        CreateManager<MissionManager>("MissionManager");
        CreateManager<CinematicDirector>("CinematicDirector");
        CreateManager<DrunkVisualEffects>("DrunkVisualEffects");
        CreateManager<AdvancedProceduralTTS>("AdvancedProceduralTTS");
        CreateManager<ArrestCutsceneManager>("ArrestCutsceneManager");
        CreateManager<CinemaTheatreManager>("CinemaTheatreManager");
        CreateManager<EconomyExpansion>("EconomyExpansion");
        CreateManager<ApexSaveSystem>("ApexSaveSystem");
        CreateManager<ClubCinematicDirector>("ClubCinematicDirector");
        CreateManager<DrugSystemManager>("DrugSystemManager");
        CreateManager<MissionIconManager>("MissionIconManager");
        CreateManager<MissionMatrixManager>("MissionMatrixManager");
        CreateManager<MobileIntegration>("MobileIntegration");
        CreateManager<WorldStreaming>("WorldStreaming");
        CreateManager<GameSettingsApp>("GameSettingsApp");
        CreateManager<FootstepSoundManager>("FootstepSoundManager");
        CreateManager<DayNightCycleManager>("DayNightCycleManager");
        CreateManager<MissionMatrixUI>("MissionMatrixUI");
        CreateManager<InGameVideoEngine>("InGameVideoEngine");
        CreateManager<EnvironmentPopulator>("EnvironmentPopulator");
        CreateManager<AssetAutoBinder>("AssetAutoBinder");
        CreateManager<UrbanMeshDirector>("UrbanMeshDirector");
        CreateManager<HDRSkyDirector>("HDRSkyDirector");
        CreateManager<LuxuryHUDManager>("LuxuryHUDManager");
        CreateManager<SoundInteractionEngine>("SoundInteractionEngine");
        CreateManager<BillboardVideoManager>("BillboardVideoManager");
        CreateManager<CohesionMaster>("CohesionMaster");
        CreateManager<ArmyHelicopterAI>("ArmyHelicopterAI");
        CreateManager<BridgeCinematicManager>("BridgeCinematicManager");
        CreateManager<GlobalVoiceHook>("GlobalVoiceHook");
        gameObject.AddComponent<SampleMissionLoader>();
    }

    void Start()
    {
        // Start interstitial timer
        if (InterstitialAdController.Instance != null)
            InterstitialAdController.Instance.StartTimer();

        // Phase 8: Auto-Attach Exit Logic to all Supercars
        // This allows player to press 'F' to exit any car
        RCC_CarControllerV3[] allCars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in allCars)
        {
            if (car.GetComponent<VehicleInteractionTrigger>() == null)
            {
                car.gameObject.AddComponent<VehicleInteractionTrigger>();
            }
        }

        // Show banner on main menu
        if (BannerAdController.Instance != null)
        {
            BannerAdController.Instance.LoadBanner();
            BannerAdController.Instance.ShowBanner();
        }

        // Load interstitial
        if (InterstitialAdController.Instance != null)
            InterstitialAdController.Instance.LoadInterstitial();

        // Load rewarded
        if (RewardedAdController.Instance != null)
            RewardedAdController.Instance.LoadRewardedAd();
    }

    /// <summary>
    /// Creates a manager if it doesn't already exist
    /// </summary>
    void CreateManager<T>(string name) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject go = new GameObject("[" + name + "]");
            go.AddComponent<T>();
            DontDestroyOnLoad(go);
            Debug.Log("[GameBootstrap] Created " + name);
        }
    }
}
