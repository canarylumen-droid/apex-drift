using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase 21: Gangster Menu Manager.
/// Provides a high-end "Racing Gangster" UI experience.
/// Clean, cinematic, and responsive. Includes click sounds and vocal support.
/// </summary>
public class GangsterMenuManager : MonoBehaviour
{
    public static GangsterMenuManager Instance { get; private set; }

    [Header("Menu Elements")]
    public GameObject mainPanel;
    public Text titleText;
    public Image backgroundImage;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        SetupGangsterTheme();
    }

    private void SetupGangsterTheme()
    {
        if (titleText != null)
        {
            titleText.text = "APEX DRIFT: GOLIATH";
            titleText.color = new Color(1f, 0.84f, 0f); // Gold
            titleText.fontSize = 60;
        }

        // Apply dark luxury background if missing
        if (backgroundImage != null && backgroundImage.sprite == null)
        {
            backgroundImage.color = new Color(0.05f, 0.05f, 0.05f, 0.95f); // Sleek charcoal
        }

        Debug.Log("[GangsterUI] Menu Theme Applied: Luxury/Racing style.");
    }

    public void OnButtonClick(string label)
    {
        // Phase 12 & 20: Sound and Voice
        if (SoundInteractionEngine.Instance != null)
            SoundInteractionEngine.Instance.PlayMenuSound("click");

        if (GlobalVoiceHook.Instance != null)
            GlobalVoiceHook.Instance.SpeakText("Opening " + label, AdvancedProceduralTTS.Emotion.Neutral);
            
        Debug.Log("[GangsterUI] User clicked: " + label);
    }
}
