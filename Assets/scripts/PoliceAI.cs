using UnityEngine;

/// <summary>
/// Phase 4: Aggressive Police AI.
/// Pursues the player car, performs PIT maneuvers, and coordinates with other units.
/// </summary>
public class PoliceAI : MonoBehaviour
{
    public float maxSpeed = 120f;
    public float acceleration = 20f;
    public float pitManeuverDistance = 5f;
    
    [Header("Phase 2 Tactics")]
    public GameObject spikeStripPrefab;
    private float lastSpikeDropTime = 0f;
    private float lastRadioCallTime = 0f;
    
    private RCC_CarControllerV3 playerCar;
    private Rigidbody rb;
    private bool isAggressive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FindPlayer();
    }

    private void FindPlayer()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
            {
                playerCar = car;
                break;
            }
        }
    }

    void FixedUpdate()
    {
        if (playerCar == null || WantedSystem.Instance == null || WantedSystem.Instance.wantedStars == 0)
        {
            // Normal traffic behavior if no stars
            return;
        }

        PursuePlayer();
    }

    private void PursuePlayer()
    {
        Vector3 targetDir = (playerCar.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerCar.transform.position);

        // Steering towards player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * 5f);

        // Acceleration
        float currentSpeed = rb.velocity.magnitude * 3.6f;
        if (currentSpeed < maxSpeed)
        {
            rb.AddForce(transform.forward * acceleration * 1000f);
        }

        // PIT Maneuver Logic
        if (distance < pitManeuverDistance && currentSpeed > 40f)
        {
            PerformPIT();
        }

        // Phase 2: Spike Strip Deployment
        if (WantedSystem.Instance.wantedStars >= 3 && distance < 30f && currentSpeed > 60f)
        {
            // Only drop if we are somewhat ahead or alongside
            float dot = Vector3.Dot(transform.forward, targetDir); // rough alignment
            if (dot < 0.5f && Time.time - lastSpikeDropTime > 20f)
            {
                DropSpikeStrip();
            }
        }

        // Phase 2: Dynamic Radio Chatter
        if (Time.time - lastRadioCallTime > 15f && Random.value > 0.6f)
        {
            lastRadioCallTime = Time.time;
            if (ProceduralTTSManager.Instance != null)
            {
                if (WantedSystem.Instance.wantedStars <= 2)
                    ProceduralTTSManager.Instance.Speak("Central, I am in pursuit. Need backup on my location.");
                else
                    ProceduralTTSManager.Instance.Speak("Suspect is hostile! Deploying spikes, send the chopper!");
            }
        }
    }

    private void DropSpikeStrip()
    {
        lastSpikeDropTime = Time.time;
        Debug.Log("[PoliceAI] Deploying Spike Strips!");
        
        // Spawn spike strip physical object behind the cop car
        GameObject spikes = new GameObject("SpikeStrip_Runtime");
        spikes.transform.position = transform.position - transform.forward * 4f;
        spikes.transform.rotation = transform.rotation;
        spikes.AddComponent<SpikeStrip>(); // Contains logical trigger and visuals
    }

    private void PerformPIT()
    {
        // Add lateral force to player's rear to spin them out
        Rigidbody playerRb = playerCar.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 contactPoint = playerCar.transform.position - playerCar.transform.forward * 2f;
            playerRb.AddForceAtPosition(transform.right * 5000f, contactPoint, ForceMode.Impulse);
            Debug.Log("[PoliceAI] PIT Maneuver Attempted!");
        }
    }
}
