using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates ALL visible UI elements at runtime.
/// No Inspector assignment needed — this script auto-builds the entire HUD.
/// </summary>
public class RuntimeUIBuilder : MonoBehaviour
{
    private Canvas mainCanvas;
    private Text speedText;
    private Text stateText;
    private Text carNameText;
    private Image rpmBar;

    void Start()
    {
        BuildCanvas();
        BuildSpeedometer();
        BuildStateDisplay();
        BuildCarNamePopup();
        BuildRPMBar();
    }

    void Update()
    {
        UpdateHUD();
    }

    // ───────────────────────── CANVAS ─────────────────────────
    private void BuildCanvas()
    {
        GameObject canvasGO = new GameObject("RuntimeHUD_Canvas");
        mainCanvas = canvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;

        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        DontDestroyOnLoad(canvasGO);
    }

    // ───────────────────────── SPEED ─────────────────────────
    private void BuildSpeedometer()
    {
        GameObject speedGO = new GameObject("SpeedText");
        speedGO.transform.SetParent(mainCanvas.transform, false);

        speedText = speedGO.AddComponent<Text>();
        speedText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        speedText.fontSize = 64;
        speedText.color = Color.white;
        speedText.alignment = TextAnchor.LowerRight;
        speedText.text = "0";

        // Outline for visibility
        Outline outline = speedGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        RectTransform rt = speedGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(-30, 20);
        rt.sizeDelta = new Vector2(300, 100);
    }

    // ───────────────────────── STATE ─────────────────────────
    private void BuildStateDisplay()
    {
        GameObject stateGO = new GameObject("StateText");
        stateGO.transform.SetParent(mainCanvas.transform, false);

        stateText = stateGO.AddComponent<Text>();
        stateText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        stateText.fontSize = 28;
        stateText.color = Color.yellow;
        stateText.alignment = TextAnchor.UpperLeft;
        stateText.text = "CRUISING";

        Outline outline = stateGO.AddComponent<Outline>();
        outline.effectColor = Color.black;

        RectTransform rt = stateGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(400, 50);
    }

    // ───────────────────────── CAR NAME ─────────────────────────
    private void BuildCarNamePopup()
    {
        GameObject nameGO = new GameObject("CarNameText");
        nameGO.transform.SetParent(mainCanvas.transform, false);

        carNameText = nameGO.AddComponent<Text>();
        carNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        carNameText.fontSize = 36;
        carNameText.color = Color.white;
        carNameText.alignment = TextAnchor.UpperCenter;
        carNameText.text = "";

        Outline outline = nameGO.AddComponent<Outline>();
        outline.effectColor = Color.black;

        RectTransform rt = nameGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -60);
        rt.sizeDelta = new Vector2(600, 50);
    }

    // ───────────────────────── RPM BAR ─────────────────────────
    private void BuildRPMBar()
    {
        // Background
        GameObject bgGO = new GameObject("RPMBar_BG");
        bgGO.transform.SetParent(mainCanvas.transform, false);
        Image bg = bgGO.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        RectTransform bgRt = bgGO.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(1, 0);
        bgRt.anchorMax = new Vector2(1, 0);
        bgRt.pivot = new Vector2(1, 0);
        bgRt.anchoredPosition = new Vector2(-30, 130);
        bgRt.sizeDelta = new Vector2(300, 20);

        // Fill bar
        GameObject fillGO = new GameObject("RPMBar_Fill");
        fillGO.transform.SetParent(bgGO.transform, false);
        rpmBar = fillGO.AddComponent<Image>();
        rpmBar.color = Color.green;
        rpmBar.type = Image.Type.Filled;
        rpmBar.fillMethod = Image.FillMethod.Horizontal;

        RectTransform fillRt = fillGO.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.sizeDelta = Vector2.zero;
    }

    // ───────────────────────── UPDATE ─────────────────────────
    private void UpdateHUD()
    {
        RCC_CarControllerV3 car = FindPlayerCar();
        if (car == null) return;

        // Speed
        float speed = Mathf.Abs(car.speed);
        if (speedText != null)
            speedText.text = Mathf.RoundToInt(speed) + "\n<size=24>KM/H</size>";

        // RPM bar
        if (rpmBar != null)
        {
            float rpmPct = Mathf.Clamp01(car.engineRPM / 8000f);
            rpmBar.fillAmount = rpmPct;

            if (rpmPct < 0.6f)
                rpmBar.color = Color.Lerp(Color.green, Color.yellow, rpmPct / 0.6f);
            else
                rpmBar.color = Color.Lerp(Color.yellow, Color.red, (rpmPct - 0.6f) / 0.4f);
        }

        // State
        if (stateText != null)
        {
            if (CinematicInteractionManager.Instance != null)
            {
                if (CinematicInteractionManager.Instance.isDrunk)
                {
                    stateText.text = "⚠ DRUNK";
                    stateText.color = Color.red;
                }
                else if (speed > 120)
                {
                    stateText.text = "🔥 BLAZING";
                    stateText.color = new Color(1f, 0.5f, 0f);
                }
                else if (speed > 60)
                {
                    stateText.text = "🏁 RACING";
                    stateText.color = Color.yellow;
                }
                else
                {
                    stateText.text = "🚗 CRUISING";
                    stateText.color = Color.white;
                }
            }
        }

        // Car name
        if (carNameText != null && string.IsNullOrEmpty(carNameText.text))
        {
            carNameText.text = car.gameObject.name.ToUpper();
        }
    }

    private RCC_CarControllerV3 FindPlayerCar()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
                return car;
        }
        return null;
    }
}
