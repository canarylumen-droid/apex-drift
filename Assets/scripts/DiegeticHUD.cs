using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Diegetic (World-Space) HUD that attaches to the car dashboard.
/// Displays speed and brand info in 3D space for maximum immersion.
/// </summary>
public class DiegeticHUD : MonoBehaviour
{
    public Text brandLabel;
    public Text speedLabel;
    
    private RCC_CarControllerV3 car;

    void Start()
    {
        car = GetComponentInParent<RCC_CarControllerV3>();
        if (brandLabel != null && car != null)
        {
            brandLabel.text = car.carName.ToUpper();
        }
    }

    void Update()
    {
        if (car == null || speedLabel == null) return;

        // Digital holographic speed display
        speedLabel.text = Mathf.RoundToInt(car.speed).ToString();
        
        // Dynamic color based on speed
        if (car.speed > 150)
            speedLabel.color = Color.Lerp(Color.white, Color.red, (car.speed - 150) / 100f);
        else
            speedLabel.color = Color.white;
    }
}
