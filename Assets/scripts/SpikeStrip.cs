using UnityEngine;

/// <summary>
/// Phase 2: Advanced Police Tactics - Spike Strip.
/// Deployed by police cars. Pops tires on impact and forces the player to lose control.
/// </summary>
public class SpikeStrip : MonoBehaviour
{
    private bool isTriggered = false;

    void Start()
    {
        // Setup simple visuals (a long thin rectangle) if no mesh is attached
        if (GetComponent<MeshFilter>() == null)
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(visual.GetComponent<Collider>()); // Remove default collider
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(8f, 0.1f, 0.5f);
            
            Renderer r = visual.GetComponent<Renderer>();
            r.material.color = new Color(0.2f, 0.2f, 0.2f); // Dark metal
        }

        // Add trigger area
        BoxCollider bc = gameObject.AddComponent<BoxCollider>();
        bc.size = new Vector3(8f, 1f, 1f);
        bc.isTrigger = true;

        // Auto destroy after 30 seconds
        Destroy(gameObject, 30f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        RCC_CarControllerV3 car = other.GetComponentInParent<RCC_CarControllerV3>();
        if (car != null && car.canControl && car.GetComponent<RCC_AICarController>() == null)
        {
            TriggerSpikeHit(car);
        }
    }

    private void TriggerSpikeHit(RCC_CarControllerV3 playerCar)
    {
        isTriggered = true;
        Debug.Log("[Police Tactics] Player hit a spike strip!");

        // Phase 3: Cinematic Event Camera (Slow-Mo Tire Burst)
        if (CinematicDirector.Instance != null)
        {
            CinematicDirector.Instance.TriggerEventCamera(transform.position, 2.5f, 0.4f);
        }

        // Voice Reaction
        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.Speak("Oh shit! My tires are blown!", AdvancedProceduralTTS.Emotion.Panicked);
        }

        // Destroy Tires in RCC Engine
        playerCar.frontLeftWheelCollider.wheelCollider.brakeTorque = 5000f;
        playerCar.frontRightWheelCollider.wheelCollider.brakeTorque = 5000f;
        playerCar.rearLeftWheelCollider.wheelCollider.brakeTorque = 5000f;
        playerCar.rearRightWheelCollider.wheelCollider.brakeTorque = 5000f;
        
        // Massive slowdown
        playerCar.GetComponent<Rigidbody>().drag = 2f;

        // Make the car swerve violently
        playerCar.GetComponent<Rigidbody>().AddTorque(Vector3.up * 50000f);

        // Sound effect (Placeholder: generic bang)
        if (CinematicAudioManager.Instance != null)
        {
            CinematicAudioManager.Instance.PlayCinematicSound("SpikePop", 1.0f);
        }
    }
}
