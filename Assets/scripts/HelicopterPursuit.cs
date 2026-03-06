using UnityEngine;

/// <summary>
/// Phase 4: Helicopter Pursuit AI.
/// Tracks player from the air and shines a spotlight.
/// </summary>
public class HelicopterPursuit : MonoBehaviour
{
    public float followHeight = 30f;
    public float followDistance = 20f;
    public Light spotlight;
    
    [Header("Army Features")]
    public AudioClip armyRadioChatter;
    public GameObject armyUnitPrefab; // Human preset
    private AudioSource audioSource;
    private float lastDropTime = 0f;

    private RCC_CarControllerV3 playerCar;

    void Start()
    {
        FindPlayer();
        SetupAudio();
    }

    private void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        audioSource.volume = 0.8f;
        
        // Load army chatter sound if available, otherwise reuse police siren or placeholder
        armyRadioChatter = Resources.Load<AudioClip>("Voices/PoliceRadio"); // Placeholder path
        if (armyRadioChatter != null)
        {
            audioSource.clip = armyRadioChatter;
            audioSource.Play();
        }
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

    void Update()
    {
        if (playerCar == null || WantedSystem.Instance == null || WantedSystem.Instance.wantedStars < 5)
        {
            gameObject.SetActive(false);
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 targetPos = playerCar.transform.position + Vector3.up * followHeight - playerCar.transform.forward * followDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
        
        // Point spotlight at player
        if (spotlight != null)
        {
            spotlight.transform.LookAt(playerCar.transform.position);
        }

        // Rotate helicopter slightly towards player
        transform.LookAt(playerCar.transform.position);

        // Army Jumping Logic
        float distanceToPlayerXY = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(playerCar.transform.position.x, playerCar.transform.position.z));
        if (distanceToPlayerXY < 15f && Time.time - lastDropTime > 10f)
        {
            DropArmyUnit();
        }
    }

    private void DropArmyUnit()
    {
        lastDropTime = Time.time;
        Debug.Log("[Helicopter] Dropping Army Unit!");
        
        // Use realistic model or placeholder
        GameObject prefab = armyUnitPrefab;
        if (prefab == null && RealisticModelManager.Instance != null && RealisticModelManager.Instance.humanIdlePrefab != null)
        {
            prefab = RealisticModelManager.Instance.humanIdlePrefab;
        }

        if (prefab != null)
        {
            Vector3 dropPos = transform.position - Vector3.up * 5f;
            GameObject soldier = Instantiate(prefab, dropPos, Quaternion.identity);
            
            // Add Army AI script
            soldier.AddComponent<ArmyUnit>();
        }
    }
}
