using UnityEngine;

/// <summary>
/// AdMob Manager for Apex Drift: 3D Racing.
/// Initializes the Mobile Ads SDK and coordinates all ad types.
/// Attach to a persistent GameObject (DontDestroyOnLoad).
/// 
/// REQUIRES: Google Mobile Ads Unity Plugin
/// Install via: Unity Package Manager or import GoogleMobileAds.unitypackage
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    // Track if SDK is initialized
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
        // ===================================================================
        // UNCOMMENT THE BLOCK BELOW AFTER IMPORTING Google Mobile Ads Plugin
        // ===================================================================

        /*
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
        */

        // TEMPORARY: For development without AdMob plugin
        IsInitialized = true;
        Debug.Log("[AdManager] Running in test mode (AdMob plugin not imported yet).");
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
