using UnityEngine;

/// <summary>
/// Phase 16: Physics Master.
/// Fine-tunes the RCC_CarControllerV3 for "Premium" handling.
/// </summary>
public class PhysicsMaster : MonoBehaviour
{
    public float globalGripMultiplier = 1.2f;
    public float highSpeedStability = 0.8f;

    void Start()
    {
        TuneAllCars();
    }

    public void TuneAllCars()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            // Apply Phase 16 "Premium" Physics
            car.maxspeed = 280f; // High-end cap
            car.brakepower = 5000f;
            
            // Adjust COM for 4K Cinematic feel (less "floaty")
            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.centerOfMass = new Vector3(0, -0.5f, 0);
                rb.mass = 1600f;
            }
        }
        Debug.Log("[Phase 16] Physics Master has tuned all vehicles.");
    }
}
