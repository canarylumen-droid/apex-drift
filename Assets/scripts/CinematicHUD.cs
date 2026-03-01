using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A sleek, cinematic HUD to display car brand and state in the GTA style.
/// </summary>
public class CinematicHUD : MonoBehaviour
{
    public static CinematicHUD Instance { get; private set; }

    [Header("UI Elements")]
    public Text carNameText;
    public Text statusText; // Shows "DRUNK", "RACING", etc.
    public CanvasGroup hudCanvasGroup;

    [Header("Animation")]
    public float fadeSpeed = 2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (hudCanvasGroup != null) hudCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        UpdateStatusDisplay();
    }

    private void UpdateStatusDisplay()
    {
        if (CinematicInteractionManager.Instance == null) return;

        bool showHUD = CinematicInteractionManager.Instance.insideVehicle;
        
        // Fade HUD in/out
        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = Mathf.MoveTowards(hudCanvasGroup.alpha, showHUD ? 1f : 0f, Time.deltaTime * fadeSpeed);
        }

        if (showHUD)
        {
            if (statusText != null)
            {
                if (CinematicInteractionManager.Instance.isDrunk)
                {
                    statusText.text = "STATE: DRUNK";
                    statusText.color = Color.red;
                }
                else if (CinematicInteractionManager.Instance.isRacing)
                {
                    statusText.text = "STATE: RACING";
                    statusText.color = Color.yellow;
                }
                else
                {
                    statusText.text = "STATE: CRUISING";
                    statusText.color = Color.white;
                }
            }
        }
    }

    /// <summary>
    /// Displays the car name when entering a vehicle
    /// </summary>
    public void NotifyEntry(string name)
    {
        if (carNameText != null)
        {
            carNameText.text = name.ToUpper();
            // Trigger an animation or pop-up here
        }
    }
}
