using UnityEngine;
using System;

/// <summary>
/// Rewarded Ad Controller for Apex Drift: 3D Racing.
/// Handles: "Double Coins", "Free Gems", "Revive", "Free Upgrade"
/// Enforces max 4 rewarded ads per day for policy compliance.
/// </summary>
public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance { get; private set; }

    private bool adLoaded = false;
    private string pendingRewardType = "";

    // Daily limit tracking
    private int rewardedAdsToday = 0;
    private string lastRewardDate = "";

#if USE_ADMOB
    private GoogleMobileAds.Api.RewardedAd rewardedAd;
#endif

    // Event for external listeners
    public delegate void OnRewardGranted(string rewardType);
    public static event OnRewardGranted RewardGranted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDailyCount();
    }

    // ===== DAILY LIMIT =====

    void LoadDailyCount()
    {
        lastRewardDate = PlayerPrefs.GetString("reward_date", "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (lastRewardDate != today)
        {
            // New day — reset count
            rewardedAdsToday = 0;
            lastRewardDate = today;
            PlayerPrefs.SetString("reward_date", today);
            PlayerPrefs.SetInt("reward_count", 0);
            PlayerPrefs.Save();
        }
        else
        {
            rewardedAdsToday = PlayerPrefs.GetInt("reward_count", 0);
        }
    }

    void IncrementDailyCount()
    {
        rewardedAdsToday++;
        PlayerPrefs.SetInt("reward_count", rewardedAdsToday);
        PlayerPrefs.Save();
    }

    public bool CanWatchRewardedAd()
    {
        LoadDailyCount(); // Refresh in case day changed
        return rewardedAdsToday < GameConfig.MAX_REWARDED_PER_DAY;
    }

    public int RemainingAdsToday()
    {
        LoadDailyCount();
        return Mathf.Max(0, GameConfig.MAX_REWARDED_PER_DAY - rewardedAdsToday);
    }

    // ===== LOAD =====

    public void LoadRewardedAd()
    {
#if USE_ADMOB
        // Clean up previous ad
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        GoogleMobileAds.Api.AdRequest request = new GoogleMobileAds.Api.AdRequest();

        GoogleMobileAds.Api.RewardedAd.Load(
            GameConfig.REWARDED_AD_ID,
            request,
            (GoogleMobileAds.Api.RewardedAd ad, GoogleMobileAds.Api.LoadAdError error) =>
            {
                if (error != null)
                {
                    adLoaded = false;
                    Debug.Log("[RewardedAd] Failed to load: " + error.GetMessage());
                    Invoke("LoadRewardedAd", 30f); // Retry
                    return;
                }

                rewardedAd = ad;
                adLoaded = true;
                Debug.Log("[RewardedAd] Loaded successfully.");

                // Handle ad closed
                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("[RewardedAd] Ad closed.");
                    adLoaded = false;
                    LoadRewardedAd(); // Preload next
                };
            }
        );
#else
        // Simulation mode
        adLoaded = true;
        Debug.Log("[RewardedAd] Simulation Mode — ad loaded.");
#endif
    }

    // ===== SHOW =====

    /// <summary>
    /// Show a rewarded ad with the specified reward type.
    /// Types: "double_coins", "free_gems", "revive", "free_upgrade", "shop_coins"
    /// </summary>
    public void ShowRewardedAd(string rewardType)
    {
        if (!CanWatchRewardedAd())
        {
            Debug.Log("[RewardedAd] Daily limit reached (" + GameConfig.MAX_REWARDED_PER_DAY + "/day). Try again tomorrow!");
            return;
        }

        if (!adLoaded)
        {
            Debug.Log("[RewardedAd] Ad not loaded yet, loading...");
            pendingRewardType = rewardType;
            LoadRewardedAd();
            return;
        }

        pendingRewardType = rewardType;

#if USE_ADMOB
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((GoogleMobileAds.Api.Reward reward) =>
            {
                Debug.Log("[RewardedAd] User earned reward: " + reward.Amount + " " + reward.Type);
                GrantReward(pendingRewardType);
            });
        }
#else
        // Simulation mode
        Debug.Log("[RewardedAd] Simulation Mode — showing ad for reward: " + rewardType);
        GrantReward(rewardType);
#endif
    }

    // ===== GRANT REWARDS =====

    void GrantReward(string rewardType)
    {
        IncrementDailyCount();

        switch (rewardType)
        {
            case "double_coins":
                // Double the last race reward — caller should handle position
                Debug.Log("[RewardedAd] Double coins reward granted.");
                if (CurrencyManager.Instance != null)
                    CurrencyManager.Instance.OnRewardedAdCoins();
                break;

            case "free_gems":
            case "shop_gems":
                Debug.Log("[RewardedAd] Free gems reward granted.");
                if (CurrencyManager.Instance != null)
                    CurrencyManager.Instance.OnRewardedAdGems();
                break;

            case "shop_coins":
                Debug.Log("[RewardedAd] Shop coins reward granted.");
                if (CurrencyManager.Instance != null)
                    CurrencyManager.Instance.OnRewardedAdCoins();
                break;

            case "revive":
                // Restart from last checkpoint — handled by race manager
                Debug.Log("[RewardedAd] Revive reward granted.");
                break;

            case "free_upgrade":
                // Free upgrade — caller should specify car/stat after this
                Debug.Log("[RewardedAd] Free upgrade reward granted.");
                break;

            default:
                Debug.Log("[RewardedAd] Unknown reward type: " + rewardType);
                break;
        }

        // Notify listeners
        if (RewardGranted != null)
            RewardGranted(rewardType);
    }
}
