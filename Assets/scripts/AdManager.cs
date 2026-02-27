#define USE_ADMOB // Enabled for live ads integration as requested by user

using UnityEngine;

/// <summary>
/// AdMob Manager for Apex Drift: 3D Racing.
/// Initializes the Mobile Ads SDK and coordinates all ad types.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    public bool IsInitialized { get; private set; }

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

    void Start()
    {
        InitializeAds();
    }

    void InitializeAds()
    {
#if USE_ADMOB
        // Initialize the Google Mobile Ads SDK
        GoogleMobileAds.Api.MobileAds.Initialize(initStatus =>
        {
            IsInitialized = true;
            Debug.Log("[AdManager] AdMob SDK initialized successfully.");

            // Start loading ads after SDK is ready
            if (BannerAdController.Instance != null)
                BannerAdController.Instance.LoadBanner();

            if (InterstitialAdController.Instance != null)
                InterstitialAdController.Instance.LoadInterstitial();

            if (RewardedAdController.Instance != null)
                RewardedAdController.Instance.LoadRewardedAd();
        });
#else
        IsInitialized = true;
        Debug.Log("[AdManager] Running in Simulation Mode (AdMob plugin not detected).");
        
        // In simulation, trigger loads immediately
        if (BannerAdController.Instance != null) BannerAdController.Instance.LoadBanner();
        if (InterstitialAdController.Instance != null) InterstitialAdController.Instance.LoadInterstitial();
        if (RewardedAdController.Instance != null) RewardedAdController.Instance.LoadRewardedAd();
#endif
    }

    // ===== CONVENIENCE METHODS =====

    /// <summary>
    /// Call from anywhere: AdManager.Instance.ShowInterstitialIfReady()
    /// </summary>
    public void ShowInterstitialIfReady()
    {
        if (InterstitialAdController.Instance != null)
            InterstitialAdController.Instance.TryShowInterstitial();
    }

    /// <summary>
    /// Call from anywhere: AdManager.Instance.ShowRewardedAd("coins")
    /// </summary>
    public void ShowRewardedAd(string rewardType)
    {
        if (RewardedAdController.Instance != null)
            RewardedAdController.Instance.ShowRewardedAd(rewardType);
    }

    /// <summary>
    /// Call from anywhere: AdManager.Instance.ShowBanner()
    /// </summary>
    public void ShowBanner()
    {
        if (BannerAdController.Instance != null)
            BannerAdController.Instance.ShowBanner();
    }

    /// <summary>
    /// Hide banner (e.g., during active racing)
    /// </summary>
    public void HideBanner()
    {
        if (BannerAdController.Instance != null)
            BannerAdController.Instance.HideBanner();
    }
}
