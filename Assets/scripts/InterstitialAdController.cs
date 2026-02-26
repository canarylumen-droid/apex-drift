using UnityEngine;

/// <summary>
/// Interstitial Ad Controller for Apex Drift: 3D Racing.
/// Shows full-screen ads every 5 minutes of gameplay.
/// Triggered at natural break points: race finish, menu transitions.
/// </summary>
public class InterstitialAdController : MonoBehaviour
{
    public static InterstitialAdController Instance { get; private set; }

    private bool adLoaded = false;
    private float timer = 0f;
    private bool timerRunning = false;

    // Uncomment when AdMob plugin is imported:
    // private GoogleMobileAds.Api.InterstitialAd interstitialAd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!timerRunning) return;

        // Count real time (not affected by Time.timeScale / pause)
        timer += Time.unscaledDeltaTime;
    }

    // ===== START / STOP TIMER =====

    /// <summary>
    /// Start the 5-minute countdown. Call this when gameplay begins.
    /// </summary>
    public void StartTimer()
    {
        timerRunning = true;
        timer = 0f;
    }

    /// <summary>
    /// Pause the timer (e.g., when app goes to background).
    /// </summary>
    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    // ===== TRY SHOW =====

    /// <summary>
    /// Call at natural break points (race end, menu load).
    /// Will only show if 5 minutes have passed since last interstitial.
    /// </summary>
    public void TryShowInterstitial()
    {
        if (timer < GameConfig.INTERSTITIAL_INTERVAL)
        {
            Debug.Log("[Interstitial] Not time yet. " + (GameConfig.INTERSTITIAL_INTERVAL - timer).ToString("F0") + "s remaining.");
            return;
        }

        if (!adLoaded)
        {
            Debug.Log("[Interstitial] Ad not loaded yet, loading now...");
            LoadInterstitial();
            return;
        }

        ShowInterstitial();
    }

    // ===== LOAD =====

    public void LoadInterstitial()
    {
        /*
        // UNCOMMENT AFTER IMPORTING ADMOB PLUGIN:

        // Clean up previous ad
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest();

        GoogleMobileAds.Api.InterstitialAd.Load(
            GameConfig.INTERSTITIAL_AD_ID,
            request,
            (GoogleMobileAds.Api.InterstitialAd ad, GoogleMobileAds.Api.LoadAdError error) =>
            {
                if (error != null)
                {
                    adLoaded = false;
                    Debug.Log("[Interstitial] Failed to load: " + error.GetMessage());
                    // Retry after 60 seconds
                    Invoke("LoadInterstitial", 60f);
                    return;
                }

                interstitialAd = ad;
                adLoaded = true;
                Debug.Log("[Interstitial] Loaded successfully.");

                // Handle ad closed — preload next one
                interstitialAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("[Interstitial] Ad closed.");
                    adLoaded = false;
                    LoadInterstitial(); // preload next
                };
            }
        );
        */

        // TEMP: Simulate loaded
        adLoaded = true;
        Debug.Log("[Interstitial] Test mode — ad simulated as loaded.");
    }

    // ===== SHOW =====

    void ShowInterstitial()
    {
        /*
        // UNCOMMENT AFTER IMPORTING ADMOB PLUGIN:
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            timer = 0f; // Reset timer after showing
            Debug.Log("[Interstitial] Showing ad.");
        }
        */

        // TEMP: Simulate show
        timer = 0f;
        Debug.Log("[Interstitial] Test mode — ad shown (simulated). Timer reset.");
    }

    void OnDestroy()
    {
        /*
        if (interstitialAd != null)
            interstitialAd.Destroy();
        */
    }
}
