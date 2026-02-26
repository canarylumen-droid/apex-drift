using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dashboard display for Apex Drift: 3D Racing.
/// Shows speedometer, RPM bar, and gear indicator.
/// Reads values from RCC_CarControllerV3.
/// </summary>
public class Dashboard : MonoBehaviour
{
    [Header("Speed Display")]
    public Text speedLabel;           // Digital speed text (e.g., "180 km/h")
    public Image speedNeedle;        // Needle rotation for analog gauge
    public float maxNeedleAngle = 270f; // Max rotation degrees
    public float maxSpeed = 250f;     // Max speed for needle scale

    [Header("RPM Display")]
    public Image rpmBar;              // Fill bar for RPM
    public Text rpmLabel;             // RPM number text

    [Header("Gear Display")]
    public Text gearLabel;            // Current gear text

    [Header("Car Reference")]
    public RCC_CarControllerV3 playerCar; // Assign the player's car

    private float currentSpeed = 0f;
    private float currentRPM = 0f;
    private int currentGear = 1;

    void Update()
    {
        if (playerCar == null)
        {
            // Try to find player car dynamically
            TryFindPlayerCar();
            return;
        }

        UpdateValues();
        UpdateUI();
    }

    void TryFindPlayerCar()
    {
        // Find the car that the player is controlling (no AI component)
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (RCC_CarControllerV3 car in cars)
        {
            if (car.GetComponent<RCC_AICarController>() == null && car.canControl)
            {
                playerCar = car;
                break;
            }
        }
    }

    void UpdateValues()
    {
        // Read speed in km/h from car controller
        currentSpeed = playerCar.speed;

        // Read engine RPM
        currentRPM = playerCar.engineRPM;

        // Read current gear
        currentGear = playerCar.currentGear;
    }

    void UpdateUI()
    {
        // Speed label
        if (speedLabel != null)
            speedLabel.text = Mathf.RoundToInt(Mathf.Abs(currentSpeed)).ToString() + " km/h";

        // Speed needle rotation
        if (speedNeedle != null)
        {
            float speedPercent = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float angle = Mathf.Lerp(0, -maxNeedleAngle, speedPercent);
            speedNeedle.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        // RPM bar fill
        if (rpmBar != null)
        {
            float rpmPercent = Mathf.Clamp01(currentRPM / 8000f); // Assuming 8000 max RPM
            rpmBar.fillAmount = rpmPercent;

            // Color shifts from green to yellow to red
            if (rpmPercent < 0.6f)
                rpmBar.color = Color.Lerp(Color.green, Color.yellow, rpmPercent / 0.6f);
            else
                rpmBar.color = Color.Lerp(Color.yellow, Color.red, (rpmPercent - 0.6f) / 0.4f);
        }

        // RPM label
        if (rpmLabel != null)
            rpmLabel.text = Mathf.RoundToInt(currentRPM).ToString() + " RPM";

        // Gear label
        if (gearLabel != null)
        {
            if (currentGear == 0)
                gearLabel.text = "R"; // Reverse
            else if (currentGear == -1)
                gearLabel.text = "N"; // Neutral
            else
                gearLabel.text = currentGear.ToString();
        }
    }
}
