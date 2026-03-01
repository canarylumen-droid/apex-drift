using UnityEngine;

/// <summary>
/// Simulates the physical and visual effects of drunk driving.
/// Hooks into the car's Rigidbody to create swaying torque.
/// </summary>
public class DrunkDrivingSystem : MonoBehaviour
{
    private RCC_CarControllerV3 carController;
    private Rigidbody rb;

    [Header("Sway settings")]
    public float swayIntensity = 2500f;
    public float swayFrequency = 0.5f;

    void Start()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (CinematicInteractionManager.Instance == null || !CinematicInteractionManager.Instance.isDrunk)
            return;

        if (carController == null || rb == null) return;

        // Only sway if moving
        if (carController.speed > 5)
        {
            float sway = Mathf.Sin(Time.time * swayFrequency) * swayIntensity;
            rb.AddTorque(Vector3.up * sway);
            
            // Random steering "glitches"
            if (Random.value > 0.98f) 
            {
                // Simulate steering slip
            }
        }
    }
}
