using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Premium Cinematic HUD for Apex Drift.
/// Replaces the placeholder code-generated UI with high-res Sprite-based elements.
/// Uses Real PNG textures for Speedometers, Maps, and Wanted Stars.
/// </summary>
public class PremiumHUD : MonoBehaviour
{
    public static PremiumHUD Instance { get; private set; }

    [Header("Textures (Assets/Textures/)")]
    public Sprite speedometerBg;
    public Sprite needleSprite;
    public Sprite wantedStarFull;
    public Sprite wantedStarEmpty;
    public Sprite mapBorder;
    public GameObject moneySprayButton; // New Phase 4 button object


    public Text warningMessageText;  // E.g., "ENGINE DAMAGE CRITICAL"
    public Text cashTotalText;
    public Text notificationText;    // Unlocks/Missions
    
    [Header("Mobile Notification Panel")]
    public GameObject mobileNotificationPanel;
    public Text mobileNotificationTitle;
    public Text mobileNotificationBody;
    
    [Header("Audio")]
    public AudioClip buttonClickSound;
    private AudioSource uiAudioSource;
    
    [Header("UI Elements")]
    private Image speedoImage;
    private RectTransform needle;
    private Image[] starImages;
    private Text speedText;
    private Text stateText;

    private RCC_CarControllerV3 playerCar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadTextures();
        SetupUIAudio();
        CreateHUDLayers();
        HookAllButtons();
    }

    private void SetupUIAudio()
    {
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.spatialBlend = 0f; // 2D sound for UI
        uiAudioSource.playOnAwake = false;
        
        // Load default click sound if not set
        if (buttonClickSound == null) buttonClickSound = Resources.Load<AudioClip>("Voices/UIClick");
    }

    private void HookAllButtons()
    {
        // Hook up sound to all buttons dynamically
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach(Button btn in allButtons)
        {
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // Phase 4: Trigger the Money Spray
    public void OnMakeItRainClicked()
    {
        PlayClickSound();
        if (EconomyExpansion.Instance != null)
        {
            EconomyExpansion.Instance.MakeItRain();
        }
    }

    private void LoadTextures()
    {
        if (speedometerBg == null) speedometerBg = Resources.Load<Sprite>("Textures/UI_Gold");
        if (wantedStarFull == null) wantedStarFull = Resources.Load<Sprite>("Textures/UI_Gold");
        if (wantedStarEmpty == null) wantedStarEmpty = Resources.Load<Sprite>("Textures/UI_Silver");
    }

    private void CreateHUDLayers()
    {
        // Canvas Setup
        GameObject canvasObj = new GameObject("PremiumHUD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // 1. SPEEDOMETER (Bottom Right)
        GameObject speedoRoot = CreateUIElement("Speedometer", canvasObj.transform);
        SetRect(speedoRoot, new Vector2(250, 250), new Vector2(1, 0), new Vector2(-150, 150));
        speedoImage = speedoRoot.AddComponent<Image>();
        speedoImage.sprite = speedometerBg; // Will be assigned in Inspector or loaded via Resources

        // Needle
        GameObject needleObj = CreateUIElement("Needle", speedoRoot.transform);
        SetRect(needleObj, new Vector2(100, 20), new Vector2(0.5f, 0.5f), Vector2.zero);
        needle = needleObj.GetComponent<RectTransform>();
        needle.AddComponent<Image>().sprite = needleSprite;

        // 2. WANTED STARS (Top Right)
        GameObject starsRoot = CreateUIElement("WantedStars", canvasObj.transform);
        SetRect(starsRoot, new Vector2(300, 60), new Vector2(1, 1), new Vector2(-160, -60));
        starImages = new Image[5];
        for (int i = 0; i < 5; i++)
        {
            GameObject star = CreateUIElement("Star_" + i, starsRoot.transform);
            SetRect(star, new Vector2(50, 50), new Vector2(0, 0.5f), new Vector2(i * 55, 0));
            starImages[i] = star.AddComponent<Image>();
            starImages[i].sprite = wantedStarEmpty;
        }

        // 3. STATE TEXT (Center Bottom)
        GameObject stateObj = CreateUIElement("StateDisplay", canvasObj.transform);
        SetRect(stateObj, new Vector2(400, 50), new Vector2(0.5f, 0), new Vector2(0, 100));
        stateText = stateObj.AddComponent<Text>();
        stateText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        stateText.fontSize = 28;
        stateText.alignment = TextAnchor.MiddleCenter;
        stateText.color = Color.yellow;

        // 4. NOTIFICATION TEXT (Center Top)
        GameObject notificationObj = CreateUIElement("NotificationDisplay", canvasObj.transform);
        SetRect(notificationObj, new Vector2(600, 80), new Vector2(0.5f, 1), new Vector2(0, -100));
        notificationText = notificationObj.AddComponent<Text>();
        notificationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        notificationText.fontSize = 32;
        notificationText.alignment = TextAnchor.MiddleCenter;
        notificationText.color = Color.white;
        notificationText.gameObject.SetActive(false); // Hidden by default
    }

    void Update()
    {
        if (playerCar == null) FindPlayerCar();
        if (playerCar == null) return;

        UpdateSpeedometer();
        UpdateWantedStars();
        UpdateStateText();
    }

    private void UpdateSpeedometer()
    {
        float speed = Mathf.Abs(playerCar.speed);
        float angle = Mathf.Lerp(180, -90, speed / 250f);
        needle.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateWantedStars()
    {
        if (WantedSystem.Instance == null) return;
        int stars = WantedSystem.Instance.wantedStars;
        for (int i = 0; i < 5; i++)
        {
            starImages[i].sprite = (i < stars) ? wantedStarFull : wantedStarEmpty;
            starImages[i].color = (i < stars) ? Color.yellow : new Color(1, 1, 1, 0.2f);
        }
    }

    private void UpdateStateText()
    {
        if (CinematicInteractionManager.Instance != null)
        {
            stateText.text = CinematicInteractionManager.Instance.currentPlayerState.ToString().ToUpper();
        }
    }

    private void FindPlayerCar()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var c in cars)
        {
            if (c.canControl && c.GetComponent<RCC_AICarController>() == null)
            {
                playerCar = c;
                break;
            }
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            // Hide after a few seconds
            StartCoroutine(HideNotificationDelayed());
        }
    }

    private System.Collections.IEnumerator HideNotificationDelayed()
    {
        yield return new WaitForSeconds(5f);
        if (notificationText != null) notificationText.gameObject.SetActive(false);
    }

    private GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.AddComponent<RectTransform>();
        return go;
    }

    private void SetRect(GameObject go, Vector2 size, Vector2 anchor, Vector2 pos)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = pos;
    }
}
