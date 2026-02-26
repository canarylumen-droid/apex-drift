using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages coins and gems currency system.
/// Persistent via PlayerPrefs. Singleton accessible from anywhere.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    // Current balances
    public int Coins { get; private set; }
    public int Gems { get; private set; }

    // Optional UI elements (assign in inspector if available)
    public Text coinsLabel;
    public Text gemsLabel;

    // Events for UI updates
    public delegate void OnCurrencyChanged(int coins, int gems);
    public static event OnCurrencyChanged CurrencyChanged;

    void Awake()
    {
        // Singleton pattern — persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCurrency();
    }

    void Start()
    {
        UpdateUI();
    }

    // ===== LOAD / SAVE =====

    void LoadCurrency()
    {
        if (!PlayerPrefs.HasKey("coins"))
        {
            Coins = GameConfig.STARTING_COINS;
            Gems = GameConfig.STARTING_GEMS;
            SaveCurrency();
        }
        else
        {
            Coins = PlayerPrefs.GetInt("coins", GameConfig.STARTING_COINS);
            Gems = PlayerPrefs.GetInt("gems", GameConfig.STARTING_GEMS);
        }
    }

    void SaveCurrency()
    {
        PlayerPrefs.SetInt("coins", Coins);
        PlayerPrefs.SetInt("gems", Gems);
        PlayerPrefs.Save();
    }

    // ===== EARN =====

    public void AddCoins(int amount)
    {
        Coins += amount;
        SaveCurrency();
        UpdateUI();
        if (CurrencyChanged != null) CurrencyChanged(Coins, Gems);
    }

    public void AddGems(int amount)
    {
        Gems += amount;
        SaveCurrency();
        UpdateUI();
        if (CurrencyChanged != null) CurrencyChanged(Coins, Gems);
    }

    // ===== SPEND =====

    public bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        SaveCurrency();
        UpdateUI();
        if (CurrencyChanged != null) CurrencyChanged(Coins, Gems);
        return true;
    }

    public bool SpendGems(int amount)
    {
        if (Gems < amount) return false;
        Gems -= amount;
        SaveCurrency();
        UpdateUI();
        if (CurrencyChanged != null) CurrencyChanged(Coins, Gems);
        return true;
    }

    public bool CanAffordCoins(int amount) { return Coins >= amount; }
    public bool CanAffordGems(int amount) { return Gems >= amount; }

    // ===== RACE REWARDS =====

    /// <summary>
    /// Give coins based on finishing position (1-5)
    /// </summary>
    public void AwardRaceReward(int position)
    {
        int reward = 0;
        switch (position)
        {
            case 1: reward = GameConfig.REWARD_1ST; break;
            case 2: reward = GameConfig.REWARD_2ND; break;
            case 3: reward = GameConfig.REWARD_3RD; break;
            case 4: reward = GameConfig.REWARD_4TH; break;
            case 5: reward = GameConfig.REWARD_5TH; break;
        }
        if (reward > 0)
            AddCoins(reward);
    }

    /// <summary>
    /// Double the last race reward (for rewarded ad "Double Coins")
    /// </summary>
    public void DoubleLastReward(int position)
    {
        int reward = 0;
        switch (position)
        {
            case 1: reward = GameConfig.REWARD_1ST; break;
            case 2: reward = GameConfig.REWARD_2ND; break;
            case 3: reward = GameConfig.REWARD_3RD; break;
            case 4: reward = GameConfig.REWARD_4TH; break;
            case 5: reward = GameConfig.REWARD_5TH; break;
        }
        if (reward > 0)
            AddCoins(reward); // adds another equal amount = double
    }

    // ===== REWARDED AD PAYOUTS =====

    public void OnRewardedAdCoins()
    {
        AddCoins(GameConfig.REWARDED_AD_COINS);
    }

    public void OnRewardedAdGems()
    {
        AddGems(GameConfig.REWARDED_AD_GEMS);
    }

    // ===== UI =====

    void UpdateUI()
    {
        if (coinsLabel != null)
            coinsLabel.text = Coins.ToString("N0");
        if (gemsLabel != null)
            gemsLabel.text = Gems.ToString("N0");
    }

    // ===== RESET (for testing / settings) =====

    public void ResetCurrency()
    {
        Coins = GameConfig.STARTING_COINS;
        Gems = GameConfig.STARTING_GEMS;
        SaveCurrency();
        UpdateUI();
        if (CurrencyChanged != null) CurrencyChanged(Coins, Gems);
    }
}
