using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// "Underground Expansion" Phase 1: In-Engine Cinematic Director.
/// Handles seamless transitions from gameplay to scripted cinematic sequences.
/// </summary>
public class CinematicDirector : MonoBehaviour
{
    public static CinematicDirector Instance { get; private set; }

    [Header("Cinematic UI")]
    public GameObject cinematicBorders; // Black bars top and bottom
    public Text subtitleText;

    private Camera mainCamera;
    private Camera cinematicCamera;
    private bool isPlayingSequence = false;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        SetupCinematicCamera();
        SetupUI();
    }

    private void SetupCinematicCamera()
    {
        mainCamera = Camera.main;
        
        GameObject camObj = new GameObject("Cinematic_Camera");
        camObj.transform.SetParent(transform);
        cinematicCamera = camObj.AddComponent<Camera>();
        cinematicCamera.enabled = false;
        
        // Add depth of field or specific post-processing here if needed
    }

    private void SetupUI()
    {
        // Generate UI dynamically so we don't need a prefab
        GameObject canvasObj = new GameObject("CinematicDirector_UI");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Always on top

        // Black Bars
        cinematicBorders = new GameObject("BlackBars");
        cinematicBorders.transform.SetParent(canvasObj.transform);
        cinematicBorders.SetActive(false);

        // Subtitles
        GameObject subObj = new GameObject("Subtitles");
        subObj.transform.SetParent(canvasObj.transform);
        RectTransform rt = subObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.2f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        subtitleText = subObj.AddComponent<Text>();
        subtitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        subtitleText.fontSize = 36;
        subtitleText.alignment = TextAnchor.MiddleCenter;
        subtitleText.color = Color.white;
        subtitleText.text = "";
        // Add outline for readability
        Outline outline = subObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        subObj.SetActive(false);
    }

    /// <summary>
    /// Starts a predefined cinematic sequence, pausing player control.
    /// </summary>
    public void StartSequence(string sequenceName, Transform focusPoint)
    {
        if (isPlayingSequence) return;
        StartCoroutine(PlaySequenceRoutine(sequenceName, focusPoint));
    }

    private IEnumerator PlaySequenceRoutine(string sequenceName, Transform focusPoint)
    {
        Debug.Log("[CinematicDirector] Starting Sequence: " + sequenceName);
        isPlayingSequence = true;
        
        // 1. Lock Player Control
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null) player.canControl = false;

        // 2. Enable Cinematic View
        if (mainCamera != null) mainCamera.enabled = false;
        cinematicCamera.enabled = true;
        cinematicBorders.SetActive(true);
        subtitleText.gameObject.SetActive(true);

        // 3. Play Specific Sequence Logic
        switch (sequenceName)
        {
            case "DrugHandover":
                yield return StartCoroutine(Sequence_DrugHandover(focusPoint, player));
                break;
            case "Busted":
                yield return StartCoroutine(Sequence_Busted(player));
                break;
            default:
                yield return new WaitForSeconds(5f); // Fallback
                break;
        }

        // 4. Restore Control seamlessly
        EndSequence(player);
    }

    private void EndSequence(RCC_CarControllerV3 player)
    {
        isPlayingSequence = false;
        
        cinematicCamera.enabled = false;
        if (mainCamera != null) mainCamera.enabled = true;
        
        cinematicBorders.SetActive(false);
        subtitleText.gameObject.SetActive(false);
        subtitleText.text = "";

        if (player != null) player.canControl = true;
        
        Debug.Log("[CinematicDirector] Sequence Ended. Control restored.");
    }

    // --- Specific Sequences --- //

    private IEnumerator Sequence_DrugHandover(Transform focusPoint, RCC_CarControllerV3 player)
    {
        // 1. Drone shot approaching the focus point
        cinematicCamera.transform.position = focusPoint.position + new Vector3(10f, 15f, 10f);
        cinematicCamera.transform.LookAt(focusPoint);
        
        // Spawn NPC
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Placeholder until we hook up FBI/Boss FBX
        npc.transform.position = focusPoint.position;
        
        ShowSubtitle("Boss: You took your sweet time.", 2.5f);
        
        // Slowly zoom in
        float t = 0;
        while (t < 2.5f)
        {
            t += Time.deltaTime;
            cinematicCamera.transform.position = Vector3.Lerp(cinematicCamera.transform.position, focusPoint.position + new Vector3(3f, 2f, 3f), Time.deltaTime * 0.5f);
            cinematicCamera.transform.LookAt(focusPoint);
            yield return null;
        }

        ShowSubtitle("Player: Cops were onto me. The package is secure.", 3f);
        yield return new WaitForSeconds(3f);

        ShowSubtitle("Boss: Good. Try not to die on the way back.", 2f);
        yield return new WaitForSeconds(2f);

        Destroy(npc); // Clean up
    }

    private IEnumerator Sequence_Busted(RCC_CarControllerV3 player)
    {
        // Low angle dramatic shot of the car and police
        if (player != null)
        {
            cinematicCamera.transform.position = player.transform.position + player.transform.right * 3f + Vector3.up * 0.5f;
            cinematicCamera.transform.LookAt(player.transform);
        }

        ShowSubtitle("WASTED", 5f);
        subtitleText.color = Color.red;
        subtitleText.fontSize = 72;
        
        // Slow mo effect
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(4f); // Use realtime because we slowed time down
        Time.timeScale = 1.0f;
        
        // Reset sub styles
        subtitleText.color = Color.white;
        subtitleText.fontSize = 36;
    }

    private void ShowSubtitle(string text, float duration)
    {
        StartCoroutine(SubtitleRoutine(text, duration));
    }

    private IEnumerator SubtitleRoutine(string text, float duration)
    {
        subtitleText.text = text;
        
        // Phase 3: Advanced Emotional TTS with breathing
        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Emotion currentEmotion = AdvancedProceduralTTS.Emotion.Neutral;
            if (text.Contains("!") || text.Contains("sweet time")) currentEmotion = AdvancedProceduralTTS.Emotion.Angry;
            
            AdvancedProceduralTTS.Instance.Speak(text, currentEmotion);
        }

        yield return new WaitForSeconds(duration);
        if (subtitleText.text == text) subtitleText.text = ""; 
    }

    /// <summary>
    /// Phase 3: Trigger a slow-mo camera zoom.
    /// User requested 0.8x speed ("Not slow, not fast").
    /// </summary>
    public void TriggerEventCamera(Vector3 position, float duration, float timeScale = 0.8f)
    {
        StartCoroutine(EventCameraRoutine(position, duration, timeScale));
    }

    private IEnumerator EventCameraRoutine(Vector3 position, float duration, float timeScale)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
        
        cinematicCamera.enabled = true;
        mainCamera.enabled = false;
        
        // 0.8x Speed feels "Hyper Realistic"
        cinematicCamera.transform.position = position + Vector3.up * 1.5f + Vector3.right * 3f;
        cinematicCamera.transform.LookAt(position + Vector3.up * 0.5f);

        // Add subtle Camera Shake for 4D feel
        float elapsed = 0;
        Vector3 origPos = cinematicCamera.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cinematicCamera.transform.position = origPos + Random.insideUnitSphere * 0.05f;
            yield return null;
        }

        Time.timeScale = originalTimeScale;
        cinematicCamera.enabled = false;
        mainCamera.enabled = true;
    }
}
