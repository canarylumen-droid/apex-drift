using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Nitro Boost System for Apex Drift: 3D Racing.
/// Press boost button for temporary speed burst with recharging bar.
/// </summary>
public class NitroBoost : MonoBehaviour
{
    public static NitroBoost Instance { get; private set; }

    [Header("UI")]
    public Image boostBar;            // Fill bar showing remaining boost
    public Text boostLabel;           // "NITRO" text (glows when ready)
    public GameObject boostEffect;    // Visual effect (particle system, screen blur, etc.)

    [Header("Input")]
    public KeyCode boostKey = KeyCode.LeftShift; // PC input
    public string boostButtonName = "Fire3";     // Controller/mobile input

    // State
    private float nitroAmount;
    private bool isBoosting = false;
    private RCC_CarControllerV3 playerCar;
    private float originalMaxSpeed = 0f;
    private bool boostReady = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nitroAmount = GameConfig.NITRO_MAX;
        UpdateUI();

        if (boostEffect != null)
            boostEffect.SetActive(false);
    }

    void Update()
    {
        // Find player car if not set
        if (playerCar == null)
        {
            FindPlayerCar();
            return;
        }

        HandleInput();
        HandleRecharge();
        UpdateUI();
    }

    void FindPlayerCar()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (RCC_CarControllerV3 car in cars)
        {
            if (car.GetComponent<RCC_AICarController>() == null && car.canControl)
            {
                playerCar = car;
                originalMaxSpeed = playerCar.maxspeed;
                break;
            }
        }
    }

    // ===== INPUT =====

    void HandleInput()
    {
        bool wantsBoost = Input.GetKey(boostKey);

        // Also check button name for mobile/controller
        if (!wantsBoost)
        {
            try { wantsBoost = Input.GetButton(boostButtonName); }
            catch { } // Button might not be configured
        }

        if (wantsBoost && nitroAmount > 0 && boostReady)
        {
            ActivateBoost();
        }
        else if (isBoosting && (!wantsBoost || nitroAmount <= 0))
        {
            DeactivateBoost();
        }
    }

    // ===== BOOST =====

    void ActivateBoost()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            if (playerCar != null)
                playerCar.maxspeed = originalMaxSpeed * GameConfig.NITRO_SPEED_MULTIPLIER;

            if (boostEffect != null)
                boostEffect.SetActive(true);
        }

        // Drain nitro
        nitroAmount -= GameConfig.NITRO_DRAIN_RATE * Time.deltaTime;
        nitroAmount = Mathf.Max(0, nitroAmount);

        if (nitroAmount <= 0)
        {
            DeactivateBoost();
            boostReady = false; // Must wait for partial recharge
        }
    }

    void DeactivateBoost()
    {
        if (isBoosting)
        {
            isBoosting = false;
            if (playerCar != null)
                playerCar.maxspeed = originalMaxSpeed;

            if (boostEffect != null)
                boostEffect.SetActive(false);
        }
    }

    // ===== RECHARGE =====

    void HandleRecharge()
    {
        if (!isBoosting && nitroAmount < GameConfig.NITRO_MAX)
        {
            nitroAmount += GameConfig.NITRO_RECHARGE_RATE * Time.deltaTime;
            nitroAmount = Mathf.Min(GameConfig.NITRO_MAX, nitroAmount);

            // Re-enable boost once recharged past 25%
            if (!boostReady && nitroAmount >= GameConfig.NITRO_MAX * 0.25f)
                boostReady = true;
        }
    }

    // ===== UI =====

    void UpdateUI()
    {
        float percent = nitroAmount / GameConfig.NITRO_MAX;

        if (boostBar != null)
        {
            boostBar.fillAmount = percent;

            // Color: Blue when full, yellow when low, red when empty
            if (percent > 0.5f)
                boostBar.color = new Color(0f, 0.5f, 1f); // Blue
            else if (percent > 0.2f)
                boostBar.color = Color.yellow;
            else
                boostBar.color = Color.red;
        }

        if (boostLabel != null)
        {
            if (isBoosting)
                boostLabel.color = new Color(1f, 0.5f, 0f); // Orange glow
            else if (boostReady)
                boostLabel.color = Color.white;
            else
                boostLabel.color = Color.gray;
        }
    }

    // ===== PUBLIC =====

    /// <summary>
    /// Mobile touch button calls this
    /// </summary>
    public void OnBoostButtonDown() { boostReady = true; }
    public void OnBoostButtonUp() { DeactivateBoost(); }

    /// <summary>
    /// Refill nitro (e.g., from pickup or rewarded ad)
    /// </summary>
    public void RefillNitro()
    {
        nitroAmount = GameConfig.NITRO_MAX;
        boostReady = true;
    }
}
