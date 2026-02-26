using UnityEngine;

/// <summary>
/// Central configuration for Apex Drift: 3D Racing
/// All game-wide constants, AdMob IDs, and settings live here.
/// </summary>
public static class GameConfig
{
    // ===== BRANDING =====
    public const string GAME_NAME = "Apex Drift: 3D Racing";
    public const string PACKAGE_NAME = "com.codemobx.apexdrift";
    public const string VERSION = "2026.1.0"; // Updated to 2026
    public const int VERSION_CODE = 1;
    public const string DEVELOPER = "CodeMobX";

    // ===== ADMOB IDS =====
    // >>> REAL IDS INJECTED <<<
    // Live AdMob IDs provided by user:
    public const string ADMOB_APP_ID = "ca-app-pub-6506000583751050~8371230716";
    public const string BANNER_AD_ID = "ca-app-pub-6506000583751050/5745067374";
    public const string INTERSTITIAL_AD_ID = "ca-app-pub-6506000583751050/5141927569";
    public const string REWARDED_AD_ID = "ca-app-pub-6506000583751050/2692216984";

    // ===== AD TIMING (seconds) =====
    public const float BANNER_ROTATE_INTERVAL = 60f;    // Banner refreshes every 60 seconds
    public const float INTERSTITIAL_INTERVAL = 300f;     // Interstitial every 5 minutes
    public const int MAX_REWARDED_PER_DAY = 4;           // Max 4 rewarded ads per day (policy safe)

    // ===== CURRENCY =====
    public const int STARTING_COINS = 500;
    public const int STARTING_GEMS = 5;

    // Race rewards (coins)
    public const int REWARD_1ST = 500;
    public const int REWARD_2ND = 300;
    public const int REWARD_3RD = 150;
    public const int REWARD_4TH = 50;
    public const int REWARD_5TH = 25;

    // Rewarded ad payouts
    public const int REWARDED_AD_COINS = 200;
    public const int REWARDED_AD_GEMS = 10;

    // ===== UPGRADE COSTS (coins) =====
    public const int UPGRADE_COST_LEVEL_1 = 300;
    public const int UPGRADE_COST_LEVEL_2 = 800;
    public const int UPGRADE_COST_LEVEL_3 = 1500;

    // ===== UPGRADE COSTS (gems) — premium path =====
    public const int UPGRADE_GEM_COST_1 = 5;
    public const int UPGRADE_GEM_COST_2 = 15;
    public const int UPGRADE_GEM_COST_3 = 30;

    // ===== CAR UNLOCK COSTS (coins) =====
    public static readonly int[] CAR_UNLOCK_COSTS = { 0, 1000, 2000, 3500, 5000 };

    // ===== DAILY REWARD =====
    public const int DAILY_COINS = 100;
    public const int DAILY_GEMS = 2;

    // ===== BOOST =====
    public const float NITRO_MAX = 100f;
    public const float NITRO_DRAIN_RATE = 25f;     // per second while boosting
    public const float NITRO_RECHARGE_RATE = 8f;    // per second passive
    public const float NITRO_SPEED_MULTIPLIER = 1.5f;
}
