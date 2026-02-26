using UnityEngine;

/// <summary>
/// Stores and manages per-car upgrade levels.
/// Each car has 4 upgradeable stats, each with 3 levels (0=stock, 1, 2, 3).
/// Persistent via PlayerPrefs.
/// </summary>
public class CarUpgradeData : MonoBehaviour
{
    public static CarUpgradeData Instance { get; private set; }

    // Stat types
    public enum StatType { Speed, Acceleration, Handling, Braking }

    // Max upgrade level per stat
    public const int MAX_LEVEL = 3;

    // Number of cars in the game
    public const int NUM_CARS = 5;

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

    // ===== GET / SET UPGRADE LEVELS =====

    public int GetUpgradeLevel(int carIndex, StatType stat)
    {
        string key = GetKey(carIndex, stat);
        return PlayerPrefs.GetInt(key, 0);
    }

    public bool SetUpgradeLevel(int carIndex, StatType stat, int level)
    {
        if (level < 0 || level > MAX_LEVEL) return false;
        string key = GetKey(carIndex, stat);
        PlayerPrefs.SetInt(key, level);
        PlayerPrefs.Save();
        return true;
    }

    // ===== UPGRADE =====

    /// <summary>
    /// Attempt to upgrade a stat. Returns true if successful.
    /// </summary>
    public bool TryUpgradeWithCoins(int carIndex, StatType stat)
    {
        int currentLevel = GetUpgradeLevel(carIndex, stat);
        if (currentLevel >= MAX_LEVEL) return false;

        int cost = GetCoinCost(currentLevel + 1);
        if (CurrencyManager.Instance == null) return false;

        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            SetUpgradeLevel(carIndex, stat, currentLevel + 1);
            return true;
        }
        return false;
    }

    public bool TryUpgradeWithGems(int carIndex, StatType stat)
    {
        int currentLevel = GetUpgradeLevel(carIndex, stat);
        if (currentLevel >= MAX_LEVEL) return false;

        int cost = GetGemCost(currentLevel + 1);
        if (CurrencyManager.Instance == null) return false;

        if (CurrencyManager.Instance.SpendGems(cost))
        {
            SetUpgradeLevel(carIndex, stat, currentLevel + 1);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Free upgrade from watching a rewarded ad
    /// </summary>
    public bool UpgradeForFree(int carIndex, StatType stat)
    {
        int currentLevel = GetUpgradeLevel(carIndex, stat);
        if (currentLevel >= MAX_LEVEL) return false;

        SetUpgradeLevel(carIndex, stat, currentLevel + 1);
        return true;
    }

    // ===== STAT MULTIPLIERS =====
    // Apply these to the car controller values

    /// <summary>
    /// Returns a multiplier (1.0 = stock, up to ~1.3 at max level)
    /// </summary>
    public float GetStatMultiplier(int carIndex, StatType stat)
    {
        int level = GetUpgradeLevel(carIndex, stat);
        return 1f + (level * 0.1f); // +10% per level
    }

    // ===== COST HELPERS =====

    public int GetCoinCost(int targetLevel)
    {
        switch (targetLevel)
        {
            case 1: return GameConfig.UPGRADE_COST_LEVEL_1;
            case 2: return GameConfig.UPGRADE_COST_LEVEL_2;
            case 3: return GameConfig.UPGRADE_COST_LEVEL_3;
            default: return 99999;
        }
    }

    public int GetGemCost(int targetLevel)
    {
        switch (targetLevel)
        {
            case 1: return GameConfig.UPGRADE_GEM_COST_1;
            case 2: return GameConfig.UPGRADE_GEM_COST_2;
            case 3: return GameConfig.UPGRADE_GEM_COST_3;
            default: return 99999;
        }
    }

    // ===== RESET =====

    public void ResetAllUpgrades()
    {
        for (int car = 0; car < NUM_CARS; car++)
        {
            foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
            {
                PlayerPrefs.DeleteKey(GetKey(car, stat));
            }
        }
        PlayerPrefs.Save();
    }

    // ===== INTERNAL =====

    string GetKey(int carIndex, StatType stat)
    {
        return "car_" + carIndex + "_" + stat.ToString().ToLower();
    }
}
