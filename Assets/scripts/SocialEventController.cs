using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Phase 15: Social Event Controller.
/// Manages high-energy city events like street parties and jubilation.
/// </summary>
public class SocialEventController : MonoBehaviour
{
    public static SocialEventController Instance { get; private set; }

    [Header("Event Settings")]
    public GameObject confettiPrefab;
    public GameObject fireworkPrefab;
    public AudioClip victoryCheer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadAssets();
    }

    private void LoadAssets()
    {
        // Load sounds from Phase 8/9 Resources
        victoryCheer = Resources.Load<AudioClip>("sounds/ui_start"); // Reuse for now
    }

    /// <summary>
    /// Triggers a global jubilation event (e.g. after a big win).
    /// </summary>
    public void TriggerJubilation(Vector3 position)
    {
        Debug.Log("[Highlife] Global Jubilation Event Triggered!");
        
        // 1. Play massive cheer
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("VictoryCheer", 1.0f);

        // 2. Trigger NPC Dancing in a wide radius
        SocialNPC[] allNpcs = FindObjectsOfType<SocialNPC>();
        foreach (var npc in allNpcs)
        {
            if (Vector3.Distance(npc.transform.position, position) < 50f)
            {
                npc.SetSocialState(SocialNPC.SocialState.Dancing);
            }
        }

        // 3. Spawn Visual Flair
        StartCoroutine(SpawnCelebrationFX(position));
    }

    private IEnumerator SpawnCelebrationFX(Vector3 pos)
    {
        for (int i = 0; i < 10; i++)
        {
            // Placeholder for confetti/fireworks
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spark.transform.position = pos + Random.insideUnitSphere * 10f + Vector3.up * 5f;
            spark.transform.localScale = Vector3.one * 0.2f;
            spark.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
            
            Rigidbody rb = spark.AddComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            
            Destroy(spark, 5f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
