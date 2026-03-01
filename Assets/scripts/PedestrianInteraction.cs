using UnityEngine;

/// <summary>
/// Handles pedestrian AI interactions, including shouting and talking when cars are near.
/// </summary>
public class PedestrianInteraction : MonoBehaviour
{
    [Header("Interaction Radius")]
    public float detectionRadius = 8f;
    public float shoutCooldown = 5f;
    
    [Header("Audio Clips")]
    public AudioClip[] talkClips;
    public AudioClip[] shoutClips;
    
    private AudioSource voiceSource;
    private float lastShoutTime;
    private bool playerNear = false;

    void Start()
    {
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.spatialBlend = 1.0f; // 3D spatial audio
        voiceSource.minDistance = 2f;
        voiceSource.maxDistance = 15f;
    }

    void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        // Find the player car (using RCC controller as anchor)
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        
        foreach (var car in cars)
        {
            // Only interact with the controlled player car
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
            {
                float distance = Vector3.Distance(transform.position, car.transform.position);
                
                if (distance < detectionRadius)
                {
                    if (!playerNear)
                    {
                        playerNear = true;
                        ReactToPlayer(car);
                    }
                }
                else
                {
                    playerNear = false;
                }
            }
        }
    }

    private void ReactToPlayer(RCC_CarControllerV3 car)
    {
        if (Time.time - lastShoutTime < shoutCooldown) return;

        // If car is moving fast, SHOUT. If slow, TALK.
        if (car.speed > 40f)
        {
            PlayRandomClip(shoutClips);
            lastShoutTime = Time.time;
            Debug.Log("[Pedestrian] SHOUTING at car speed: " + car.speed);
        }
        else if (car.speed < 10f)
        {
            PlayRandomClip(talkClips);
            lastShoutTime = Time.time;
            Debug.Log("[Pedestrian] Talking/Greeting player.");
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0 && voiceSource != null)
        {
            voiceSource.clip = clips[Random.Range(0, clips.Length)];
            voiceSource.Play();
        }
    }
}
