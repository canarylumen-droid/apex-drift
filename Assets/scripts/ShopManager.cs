using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Upgrade Shop UI Manager for Apex Drift: 3D Racing.
/// Handles car stat upgrades via coins, gems, or rewarded ads.
/// Attach to a Shop panel in the Main Menu scene.
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Car Selection")]
    public int selectedCarIndex = 0;
    public Text carNameLabel;
    public string[] carNames = { "Audi R8 GT", "BMW M2", "Dodge Viper", "Ferrari 599", "Mercedes SLS" };

    [Header("Stat Display")]
    public Text speedLevelLabel;
    public Text accelLevelLabel;
    public Text handlingLevelLabel;
    public Text brakingLevelLabel;

    [Header("Cost Display")]
    public Text speedCostLabel;
    public Text accelCostLabel;
    public Text handlingCostLabel;
    public Text brakingCostLabel;

    [Header("Currency Display")]
    public Text coinsLabel;
    public Text gemsLabel;

    [Header("Buttons")]
    public Button speedUpgradeBtn;
    public Button accelUpgradeBtn;
    public Button handlingUpgradeBtn;
    public Button brakingUpgradeBtn;
    public Button watchAdBtn;
    public Button nextCarBtn;
    public Button prevCarBtn;

    [Header("Feedback")]
    public Text feedbackLabel;

    void Start()
    {
        // Button listeners
        if (nextCarBtn != null) nextCarBtn.onClick.AddListener(NextCar);
        if (prevCarBtn != null) prevCarBtn.onClick.AddListener(PrevCar);
        if (speedUpgradeBtn != null) speedUpgradeBtn.onClick.AddListener(() => UpgradeStat(CarUpgradeData.StatType.Speed));
        if (accelUpgradeBtn != null) accelUpgradeBtn.onClick.AddListener(() => UpgradeStat(CarUpgradeData.StatType.Acceleration));
        if (handlingUpgradeBtn != null) handlingUpgradeBtn.onClick.AddListener(() => UpgradeStat(CarUpgradeData.StatType.Handling));
        if (brakingUpgradeBtn != null) brakingUpgradeBtn.onClick.AddListener(() => UpgradeStat(CarUpgradeData.StatType.Braking));
        if (watchAdBtn != null) watchAdBtn.onClick.AddListener(WatchAdForCoins);

        RefreshUI();
    }

    // ===== CAR NAVIGATION =====

    void NextCar()
    {
        selectedCarIndex = (selectedCarIndex + 1) % CarUpgradeData.NUM_CARS;
        RefreshUI();
    }

    void PrevCar()
    {
        selectedCarIndex--;
        if (selectedCarIndex < 0) selectedCarIndex = CarUpgradeData.NUM_CARS - 1;
        RefreshUI();
    }

    // ===== UPGRADE =====

    void UpgradeStat(CarUpgradeData.StatType stat)
    {
        if (CarUpgradeData.Instance == null) return;

        int currentLevel = CarUpgradeData.Instance.GetUpgradeLevel(selectedCarIndex, stat);
        if (currentLevel >= CarUpgradeData.MAX_LEVEL)
        {
            ShowFeedback("Already at MAX level!");
            return;
        }

        // Try coins first
        if (CarUpgradeData.Instance.TryUpgradeWithCoins(selectedCarIndex, stat))
        {
            ShowFeedback(stat.ToString() + " upgraded to Level " + (currentLevel + 1) + "!");
            RefreshUI();
            return;
        }

        // Not enough coins — try gems
        if (CarUpgradeData.Instance.TryUpgradeWithGems(selectedCarIndex, stat))
        {
            ShowFeedback(stat.ToString() + " upgraded with Gems!");
            RefreshUI();
            return;
        }

        ShowFeedback("Not enough coins or gems! Watch an ad for free coins.");
    }

    // ===== REWARDED AD =====

    void WatchAdForCoins()
    {
        // This will be connected to RewardedAdController
        if (RewardedAdController.Instance != null)
        {
            RewardedAdController.Instance.ShowRewardedAd("shop_coins");
        }
        else
        {
            // Fallback for testing without ads
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnRewardedAdCoins();
                ShowFeedback("+" + GameConfig.REWARDED_AD_COINS + " Coins!");
                RefreshUI();
            }
        }
    }

    // ===== UI REFRESH =====

    void RefreshUI()
    {
        if (CarUpgradeData.Instance == null || CurrencyManager.Instance == null) return;

        // Car name
        if (carNameLabel != null)
            carNameLabel.text = carNames[selectedCarIndex];

        // Stat levels
        UpdateStatLabel(speedLevelLabel, selectedCarIndex, CarUpgradeData.StatType.Speed);
        UpdateStatLabel(accelLevelLabel, selectedCarIndex, CarUpgradeData.StatType.Acceleration);
        UpdateStatLabel(handlingLevelLabel, selectedCarIndex, CarUpgradeData.StatType.Handling);
        UpdateStatLabel(brakingLevelLabel, selectedCarIndex, CarUpgradeData.StatType.Braking);

        // Costs
        UpdateCostLabel(speedCostLabel, selectedCarIndex, CarUpgradeData.StatType.Speed);
        UpdateCostLabel(accelCostLabel, selectedCarIndex, CarUpgradeData.StatType.Acceleration);
        UpdateCostLabel(handlingCostLabel, selectedCarIndex, CarUpgradeData.StatType.Handling);
        UpdateCostLabel(brakingCostLabel, selectedCarIndex, CarUpgradeData.StatType.Braking);

        // Currency
        if (coinsLabel != null)
            coinsLabel.text = CurrencyManager.Instance.Coins.ToString("N0");
        if (gemsLabel != null)
            gemsLabel.text = CurrencyManager.Instance.Gems.ToString("N0");
    }

    void UpdateStatLabel(Text label, int car, CarUpgradeData.StatType stat)
    {
        if (label == null) return;
        int level = CarUpgradeData.Instance.GetUpgradeLevel(car, stat);
        if (level >= CarUpgradeData.MAX_LEVEL)
            label.text = "Lv." + level + " (MAX)";
        else
            label.text = "Lv." + level;
    }

    void UpdateCostLabel(Text label, int car, CarUpgradeData.StatType stat)
    {
        if (label == null) return;
        int level = CarUpgradeData.Instance.GetUpgradeLevel(car, stat);
        if (level >= CarUpgradeData.MAX_LEVEL)
        {
            label.text = "MAXED";
        }
        else
        {
            int coinCost = CarUpgradeData.Instance.GetCoinCost(level + 1);
            label.text = coinCost.ToString("N0") + " Coins";
        }
    }

    // ===== FEEDBACK =====

    void ShowFeedback(string message)
    {
        if (feedbackLabel != null)
        {
            feedbackLabel.text = message;
            CancelInvoke("ClearFeedback");
            Invoke("ClearFeedback", 3f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackLabel != null)
            feedbackLabel.text = "";
    }
}
