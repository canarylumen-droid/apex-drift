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
        CreateManager<RuntimeUIBuilder>("RuntimeUIBuilder");
        CreateManager<DrunkVisualEffects>("DrunkVisualEffects");
    }

    void Start()
    {
        // Start interstitial timer
        if (InterstitialAdController.Instance != null)
            InterstitialAdController.Instance.StartTimer();

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
