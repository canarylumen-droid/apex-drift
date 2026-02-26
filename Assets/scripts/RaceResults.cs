using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Race Results Screen for Apex Drift: 3D Racing.
/// Shows after race finish: position, coins earned, buttons for double coins (ad),
/// interstitial timing, and return options.
/// Attach to the results panel in the race scene.
/// </summary>
public class RaceResults : MonoBehaviour
{
    [Header("UI Elements")]
    public Text positionLabel;
    public Text coinsEarnedLabel;
    public Text totalCoinsLabel;
    public Text raceTimeLabel;
    public Text bestTimeLabel;

    [Header("Buttons")]
    public Button doubleCoinsBtn;     // Watch ad to double reward
    public Button retryBtn;
    public Button nextRaceBtn;
    public Button mainMenuBtn;
    public Button watchAdGemsBtn;     // Watch ad for gems

    [Header("Panel")]
    public GameObject resultsPanel;

    private int finishPosition = 0;
    private int coinsEarned = 0;
    private bool hasDoubled = false;

    void Start()
    {
        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (doubleCoinsBtn != null)
            doubleCoinsBtn.onClick.AddListener(OnDoubleCoins);

        if (retryBtn != null)
            retryBtn.onClick.AddListener(OnRetry);

        if (nextRaceBtn != null)
            nextRaceBtn.onClick.AddListener(OnNextRace);

        if (mainMenuBtn != null)
            mainMenuBtn.onClick.AddListener(OnMainMenu);

        if (watchAdGemsBtn != null)
            watchAdGemsBtn.onClick.AddListener(OnWatchAdGems);

        // Listen for rewarded ad completion
        RewardedAdController.RewardGranted += OnRewardGranted;
    }

    void OnDestroy()
    {
        RewardedAdController.RewardGranted -= OnRewardGranted;
    }

    // ===== SHOW RESULTS =====

    /// <summary>
    /// Call this from obj_tracker when race ends.
    /// </summary>
    public void ShowResults(int position)
    {
        finishPosition = position;
        hasDoubled = false;

        // Award coins
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AwardRaceReward(position);
        }

        // Calculate coins earned
        switch (position)
        {
            case 1: coinsEarned = GameConfig.REWARD_1ST; break;
            case 2: coinsEarned = GameConfig.REWARD_2ND; break;
            case 3: coinsEarned = GameConfig.REWARD_3RD; break;
            case 4: coinsEarned = GameConfig.REWARD_4TH; break;
            case 5: coinsEarned = GameConfig.REWARD_5TH; break;
            default: coinsEarned = 0; break;
        }

        // Update UI
        if (positionLabel != null)
        {
            string[] suffixes = { "", "st", "nd", "rd", "th", "th" };
            positionLabel.text = position + suffixes[position] + " Place!";

            // Color based on position
            switch (position)
            {
                case 1: positionLabel.color = new Color32(0xD4, 0xAF, 0x37, 0xFF); break; // Gold
                case 2: positionLabel.color = new Color32(0xC0, 0xC0, 0xC0, 0xFF); break; // Silver
                case 3: positionLabel.color = new Color32(0xCD, 0x7F, 0x32, 0xFF); break; // Bronze
                default: positionLabel.color = Color.red; break;
            }
        }

        if (coinsEarnedLabel != null)
            coinsEarnedLabel.text = "+" + coinsEarned + " Coins";

        if (totalCoinsLabel != null && CurrencyManager.Instance != null)
            totalCoinsLabel.text = "Total: " + CurrencyManager.Instance.Coins.ToString("N0");

        // Race time
        if (raceTimeLabel != null && RaceTimer.Instance != null)
            raceTimeLabel.text = "Time: " + RaceTimer.FormatTime(RaceTimer.Instance.GetRaceTime());

        if (bestTimeLabel != null && RaceTimer.Instance != null)
            bestTimeLabel.text = "Best: " + RaceTimer.FormatTime(RaceTimer.Instance.GetBestTime());

        // Show double button only if can watch ads
        if (doubleCoinsBtn != null)
        {
            bool canWatch = RewardedAdController.Instance != null && RewardedAdController.Instance.CanWatchRewardedAd();
            doubleCoinsBtn.gameObject.SetActive(canWatch && coinsEarned > 0);
        }

        // Show results panel
        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        // Show banner ad on results screen
        if (AdManager.Instance != null)
            AdManager.Instance.ShowBanner();

        // Try to show interstitial (only shows if 5 min passed)
        if (AdManager.Instance != null)
            AdManager.Instance.ShowInterstitialIfReady();
    }

    // ===== BUTTON HANDLERS =====

    void OnDoubleCoins()
    {
        if (hasDoubled) return;

        if (RewardedAdController.Instance != null)
            RewardedAdController.Instance.ShowRewardedAd("double_coins");
    }

    void OnWatchAdGems()
    {
        if (RewardedAdController.Instance != null)
            RewardedAdController.Instance.ShowRewardedAd("free_gems");
    }

    void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnNextRace()
    {
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
        else
            SceneManager.LoadScene(0); // Back to menu
    }

    void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // ===== REWARD CALLBACK =====

    void OnRewardGranted(string rewardType)
    {
        if (rewardType == "double_coins" && !hasDoubled)
        {
            hasDoubled = true;

            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.DoubleLastReward(finishPosition);

            coinsEarned *= 2;

            if (coinsEarnedLabel != null)
                coinsEarnedLabel.text = "+" + coinsEarned + " Coins (DOUBLED!)";

            if (totalCoinsLabel != null && CurrencyManager.Instance != null)
                totalCoinsLabel.text = "Total: " + CurrencyManager.Instance.Coins.ToString("N0");

            if (doubleCoinsBtn != null)
                doubleCoinsBtn.gameObject.SetActive(false);
        }
        else if (rewardType == "free_gems")
        {
            // Refresh UI
            if (totalCoinsLabel != null && CurrencyManager.Instance != null)
                totalCoinsLabel.text = "Total: " + CurrencyManager.Instance.Coins.ToString("N0");
        }
    }
}
