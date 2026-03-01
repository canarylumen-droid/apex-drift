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
    public AudioClip[] talkClips;   // Greet, Talk, Gossip
    public AudioClip[] shoutClips;  // Warnings, Speed Shouts
    public AudioClip[] reactiveClips; // Reactions to "Illegal" driving
    
    private AudioSource voiceSource;
    private float lastShoutTime;
    private bool playerNear = false;

    void Start()
    {
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.spatialBlend = 1.0f; 
        voiceSource.minDistance = 2f;
        voiceSource.maxDistance = 15f;

        // "Human have shadow" - Force casting
        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null)
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.receiveShadows = true;
        }
    }

    void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        
        foreach (var car in cars)
        {
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

        // 1. SHOUT if fast
        if (car.speed > 50f)
        {
            PlayRandomClip(shoutClips);
            lastShoutTime = Time.time;
        }
        // 2. COMPLAIN/TALK if slightly reckless
        else if (car.speed > 25f)
        {
            PlayRandomClip(reactiveClips);
            lastShoutTime = Time.time;
        }
        // 3. GREET if slow
        else if (car.speed < 10f)
        {
            PlayRandomClip(talkClips);
            lastShoutTime = Time.time;
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
