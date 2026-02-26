using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Branded Splash Screen for Apex Drift: 3D Racing.
/// Shows game logo, name, loading bar, and tips. Then loads Main Menu.
/// Create a new scene called "SplashScreen" and make it Build Index 0.
/// Move the old Main Menu to Build Index 1.
/// </summary>
public class SplashScreen : MonoBehaviour
{
    [Header("Branding")]
    public Text gameTitle;
    public Text subtitleText;
    public Text developerText;
    public Text versionText;

    [Header("Loading")]
    public Image loadingBar;
    public Text loadingText;
    public Text tipText;

    [Header("Settings")]
    public float minSplashTime = 3f;    // Minimum splash duration
    public int mainMenuSceneIndex = 1;  // Scene index of Main Menu

    // Loading tips
    private string[] tips = new string[]
    {
        "Use NITRO wisely — save it for straights!",
        "Upgrade your car in the Shop for better performance",
        "Watch ads to earn free coins and gems",
        "Daily login rewards grow with consecutive days!",
        "1st place earns 500 coins — aim for gold!",
        "Handling upgrades help on tight corners",
        "You can revive once per race by watching an ad",
        "Higher difficulty = bigger coin rewards",
        "Gems can buy premium upgrades faster",
        "Tap the boost button during long stretches for max speed"
    };

    void Start()
    {
        // Set branding text
        if (gameTitle != null)
            gameTitle.text = GameConfig.GAME_NAME;

        if (subtitleText != null)
            subtitleText.text = "Street Racing at its Finest";

        if (developerText != null)
            developerText.text = "By " + GameConfig.DEVELOPER;

        if (versionText != null)
            versionText.text = "v" + GameConfig.VERSION;

        // Random tip
        if (tipText != null)
            tipText.text = tips[Random.Range(0, tips.Length)];

        // Start loading
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        float elapsed = 0f;

        // Fake loading progress for first 2 seconds
        while (elapsed < minSplashTime * 0.6f)
        {
            elapsed += Time.deltaTime;
            float fakeProgress = elapsed / minSplashTime;
            UpdateLoadingBar(fakeProgress * 0.5f);
            yield return null;
        }

        // Start actual async scene load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneIndex);
        asyncLoad.allowSceneActivation = false;

        UpdateLoadingText("Loading game...");

        // Wait for load to complete
        while (!asyncLoad.isDone)
        {
            // asyncLoad.progress goes up to 0.9 before activation
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float displayProgress = 0.5f + (realProgress * 0.5f);
            UpdateLoadingBar(displayProgress);

            elapsed += Time.deltaTime;

            // Allow activation when loaded AND min time passed
            if (asyncLoad.progress >= 0.9f && elapsed >= minSplashTime)
            {
                UpdateLoadingBar(1f);
                UpdateLoadingText("Ready!");
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void UpdateLoadingBar(float progress)
    {
        if (loadingBar != null)
            loadingBar.fillAmount = progress;

        if (loadingText != null)
            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";
    }

    void UpdateLoadingText(string text)
    {
        if (loadingText != null)
            loadingText.text = text;
    }
}
