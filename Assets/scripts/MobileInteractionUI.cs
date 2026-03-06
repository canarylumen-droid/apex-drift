using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Phase 12: Mobile Interaction UI.
/// Displays pop-up buttons for context-sensitive actions (Greet, Enter, Mission).
/// Replaces keyboard-only prompts for mobile-first gameplay.
/// </summary>
public class MobileInteractionUI : MonoBehaviour
{
    public static MobileInteractionUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject interactionButtonRoot;
    public Text actionText;
    public Button actionButton;
    public Image voiceIcon; // New: Glowing Voice Icon

    [Header("Interaction State")]
    private bool isHolding = false;
    private float holdTime = 0f;
    private const float REQUIRED_HOLD = 0.8f;
    private Action currentAction;

    void Awake()
    {
        if (Instance == null) Instance = this;
        CreateMobileUI();
    }

    private void CreateMobileUI()
    {
        // Auto-create UI if not assigned
        if (interactionButtonRoot == null)
        {
            GameObject canvas = GameObject.Find("PremiumHUD_Canvas");
            if (canvas == null) return;

            interactionButtonRoot = new GameObject("MobileInteractionRoot");
            interactionButtonRoot.transform.SetParent(canvas.transform, false);
            
            RectTransform rt = interactionButtonRoot.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 200);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(250, -100); // Right side pop-up

            GameObject btnObj = new GameObject("InteractionButton");
            btnObj.transform.SetParent(interactionButtonRoot.transform, false);
            actionButton = btnObj.AddComponent<Button>();
            Image img = btnObj.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Textures/UI_Gold");
            
            RectTransform btnRt = btnObj.GetComponent<RectTransform>();
            btnRt.sizeDelta = new Vector2(150, 150);

            GameObject txtObj = new GameObject("ActionText");
            txtObj.transform.SetParent(btnObj.transform, false);
            actionText = txtObj.AddComponent<Text>();
            actionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            actionText.fontSize = 30;
            actionText.alignment = TextAnchor.MiddleCenter;
            actionText.color = Color.black;
            actionText.text = "ACTION";
            
            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.sizeDelta = new Vector2(150, 50);

            actionButton.onClick.AddListener(OnButtonClick);
            interactionButtonRoot.SetActive(false);

            // Phase 16: Permanent Camera Switch Button for Mobile
            CreatePermanentCamButton(canvas.transform);
        }
    }

    private void CreatePermanentCamButton(Transform canvas)
    {
        GameObject camBtnObj = new GameObject("MobileCamButton");
        camBtnObj.transform.SetParent(canvas, false);
        Button btn = camBtnObj.AddComponent<Button>();
        Image img = camBtnObj.AddComponent<Image>();
        img.sprite = Resources.Load<Sprite>("Textures/UI_Silver");
        
        RectTransform rt = camBtnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-100, -100); // Top Right

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(camBtnObj.transform, false);
        Text t = txtObj.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = "CAM";
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.fontSize = 20;

        btn.onClick.AddListener(() => {
            CinematicCarCamera carCam = FindObjectOfType<CinematicCarCamera>();
            if (carCam != null) carCam.ToggleCamera();
            
            ThirdPersonCharacterController charCont = FindObjectOfType<ThirdPersonCharacterController>();
            if (charCont != null && charCont.GetComponent<PerspectiveManager>() != null)
                charCont.GetComponent<PerspectiveManager>().TogglePerspective();

            if (SoundInteractionEngine.Instance != null)
                SoundInteractionEngine.Instance.PlayMenuSound("swipe");
        });

        // Phase 22: Voice Interaction Icon
        CreateVoiceIcon(canvas);
    }

    private void CreateVoiceIcon(Transform canvas)
    {
        GameObject voiceObj = new GameObject("MobileVoiceIcon");
        voiceObj.transform.SetParent(canvas, false);
        voiceIcon = voiceObj.AddComponent<Image>();
        voiceIcon.sprite = Resources.Load<Sprite>("Textures/UI_Voice_Wave");
        
        RectTransform rt = voiceObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 120);
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.anchoredPosition = new Vector2(150, 150); // Bottom Left

        // Add Multi-Touch Listener for Click & Hold
        EventTrigger trigger = voiceObj.AddComponent<EventTrigger>();
        
        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { StartHold(); });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { EndHold(); });
        trigger.triggers.Add(pointerUp);
    }

    void Update()
    {
        // PC Voice Hotkey: Hold 'V'
        if (Input.GetKeyDown(KeyCode.V)) StartHold();
        if (Input.GetKeyUp(KeyCode.V)) EndHold();

        if (isHolding)
        {
            holdTime += Time.deltaTime;
            float pulse = 1f + Mathf.Sin(Time.time * 10f) * 0.1f;
            if (voiceIcon != null) voiceIcon.transform.localScale = Vector3.one * pulse;

            if (holdTime >= REQUIRED_HOLD)
            {
                TriggerVoiceCommand();
                isHolding = false; // Reset to prevent double trigger
            }
        }
    }

    private void StartHold()
    {
        isHolding = true;
        holdTime = 0f;
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("Tone_Hold", 0.3f);
    }

    private void EndHold()
    {
        isHolding = false;
        if (voiceIcon != null) voiceIcon.transform.localScale = Vector3.one;
    }

    private void TriggerVoiceCommand()
    {
        Debug.Log("[4DInteraction] Voice Command Triggered via Hold.");
        
        // Phase 27: Million Dollar Brain Integration
        string playerGreeting = "HI"; // Default greeting for now
        if (GlobalVoiceHook.Instance != null)
            GlobalVoiceHook.Instance.SpeakText(playerGreeting, AdvancedProceduralTTS.Emotion.Neutral);
        
        // Phase 22: Link to nearest NPC
        NPCRoamingAI[] npcs = FindObjectsOfType<NPCRoamingAI>();
        NPCRoamingAI nearest = null;
        float minDist = 10f; // Max interaction range

        foreach (var npc in npcs)
        {
            float dist = Vector3.Distance(transform.position, npc.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = npc;
            }
        }

        if (nearest != null)
        {
            nearest.SetWaitState(true);
            // Auto-release after 5 seconds if no further interaction
            StartCoroutine(ReleaseNPC(nearest));
        }

        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif
    }

    private System.Collections.IEnumerator ReleaseNPC(NPCRoamingAI npc)
    {
        yield return new WaitForSeconds(5f);
        if (npc != null) npc.SetWaitState(false);
    }

    public void ShowInteraction(string label, Action action)
    {
        if (interactionButtonRoot == null) return;
        
        actionText.text = label.ToUpper();
        currentAction = action;
        interactionButtonRoot.SetActive(true);
        
        // Add a "Pop" animation scale if possible
        interactionButtonRoot.transform.localScale = Vector3.one * 0.1f;
        StartCoroutine(AnimatePop());
    }

    private System.Collections.IEnumerator AnimatePop()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            interactionButtonRoot.transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, t);
            yield return null;
        }
    }

    public void HideInteraction()
    {
        if (interactionButtonRoot != null) interactionButtonRoot.SetActive(false);
        currentAction = null;
    }

    public void OnDrugButtonClick()
    {
        if (DrugSystemManager.Instance != null) DrugSystemManager.Instance.UseHeroin();
        HideInteraction();
    }

    public void OnMissionListButtonClick()
    {
        if (MissionMatrixUI.Instance != null) MissionMatrixUI.Instance.ToggleMissionList();
        HideInteraction();
    }

    private void OnButtonClick()
    {
        // Phase 10: Feedback sound
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("UI_Select", 0.7f);

        currentAction?.Invoke();
        HideInteraction();
    }
}
