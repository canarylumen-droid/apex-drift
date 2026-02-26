using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Daily Reward System for Apex Drift: 3D Racing.
/// Grants coins and gems once per day on login.
/// Shows a popup panel when reward is available.
/// </summary>
public class DailyReward : MonoBehaviour
{
    [Header("UI")]
    public GameObject rewardPanel;      // Panel that shows "Daily Reward!"
    public Text rewardText;            // Shows what they earned
    public Button claimButton;         // Tap to claim
    public Text streakLabel;           // Shows consecutive days

    private int consecutiveDays = 0;

    void Start()
    {
        if (claimButton != null)
            claimButton.onClick.AddListener(ClaimReward);

        CheckDailyReward();
    }

    void CheckDailyReward()
    {
        string lastClaim = PlayerPrefs.GetString("daily_last_claim", "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (lastClaim == today)
        {
            // Already claimed today
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
            return;
        }

        // Check streak
        if (!string.IsNullOrEmpty(lastClaim))
        {
            DateTime lastDate;
            if (DateTime.TryParse(lastClaim, out lastDate))
            {
                double daysDiff = (DateTime.Now - lastDate).TotalDays;
                if (daysDiff <= 1.5) // Within ~36 hours = streak continues
                    consecutiveDays = PlayerPrefs.GetInt("daily_streak", 0) + 1;
                else
                    consecutiveDays = 1; // Streak broken
            }
            else
            {
                consecutiveDays = 1;
            }
        }
        else
        {
            consecutiveDays = 1;
        }

        // Show reward panel
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
            int bonusCoins = GameConfig.DAILY_COINS * Mathf.Min(consecutiveDays, 7); // Max 7x multiplier
            int bonusGems = GameConfig.DAILY_GEMS * Mathf.Min(consecutiveDays, 7);

            if (rewardText != null)
                rewardText.text = "Day " + consecutiveDays + " Reward!\n\n" +
                                  bonusCoins + " Coins\n" +
                                  bonusGems + " Gems";

            if (streakLabel != null)
                streakLabel.text = consecutiveDays + " day streak!";
        }
    }

    void ClaimReward()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        int bonusCoins = GameConfig.DAILY_COINS * Mathf.Min(consecutiveDays, 7);
        int bonusGems = GameConfig.DAILY_GEMS * Mathf.Min(consecutiveDays, 7);

        // Grant rewards
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(bonusCoins);
            CurrencyManager.Instance.AddGems(bonusGems);
        }

        // Save claim date and streak
        PlayerPrefs.SetString("daily_last_claim", today);
        PlayerPrefs.SetInt("daily_streak", consecutiveDays);
        PlayerPrefs.Save();

        // Hide panel
        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        Debug.Log("[DailyReward] Claimed: " + bonusCoins + " coins, " + bonusGems + " gems (Day " + consecutiveDays + ")");
    }
}
