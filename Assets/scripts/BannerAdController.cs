using UnityEngine;

/// <summary>
/// Banner Ad Controller for Apex Drift: 3D Racing.
/// Displays banner ads that rotate every 60 seconds.
/// Banner is shown on: Main Menu, Race Results, Shop.
/// Banner is HIDDEN during active racing (not distracting).
/// </summary>
public class BannerAdController : MonoBehaviour
{
    public static BannerAdController Instance { get; private set; }

    private bool bannerLoaded = false;
    private bool bannerVisible = false;
    private float rotateTimer = 0f;

    // Uncomment when AdMob plugin is imported:
    // private GoogleMobileAds.Api.BannerView bannerView;

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
        if (!bannerVisible || !bannerLoaded) return;

        // Rotate (reload) banner every 60 seconds
        rotateTimer += Time.unscaledDeltaTime;
        if (rotateTimer >= GameConfig.BANNER_ROTATE_INTERVAL)
        {
            rotateTimer = 0f;
            RefreshBanner();
        }
    }

    // ===== LOAD =====

    public void LoadBanner()
    {
        /*
        // UNCOMMENT AFTER IMPORTING ADMOB PLUGIN:

        // Clean up old banner
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create banner at bottom of screen
        bannerView = new GoogleMobileAds.Api.BannerView(
            GameConfig.BANNER_AD_ID,
            GoogleMobileAds.Api.AdSize.Banner,
            GoogleMobileAds.Api.AdPosition.Bottom
        );

        // Event handlers
        bannerView.OnBannerAdLoaded += () => {
            bannerLoaded = true;
            Debug.Log("[BannerAd] Loaded successfully.");
        };

        bannerView.OnBannerAdLoadFailed += (GoogleMobileAds.Api.LoadAdError error) => {
            bannerLoaded = false;
            Debug.Log("[BannerAd] Failed to load: " + error.GetMessage());
            // Retry after 30 seconds
            Invoke("LoadBanner", 30f);
        };

        // Load the ad
        GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest();
        bannerView.LoadAd(request);
        */

        // TEMP: Simulate loaded
        bannerLoaded = true;
        Debug.Log("[BannerAd] Test mode — banner simulated.");
    }

    // ===== SHOW / HIDE =====

    public void ShowBanner()
    {
        bannerVisible = true;
        rotateTimer = 0f;

        /*
        if (bannerView != null)
            bannerView.Show();
        */

        Debug.Log("[BannerAd] Banner shown.");
    }

    public void HideBanner()
    {
        bannerVisible = false;

        /*
        if (bannerView != null)
            bannerView.Hide();
        */

        Debug.Log("[BannerAd] Banner hidden.");
    }

    // ===== REFRESH =====

    void RefreshBanner()
    {
        /*
        if (bannerView != null)
        {
            GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest();
            bannerView.LoadAd(request);
        }
        */

        Debug.Log("[BannerAd] Banner refreshed (60s cycle).");
    }

    void OnDestroy()
    {
        /*
        if (bannerView != null)
            bannerView.Destroy();
        */
    }
}
