using UnityEngine;

/// <summary>
/// Cinematic Gore System for Apex Drift.
/// Spawns blood-red particles and plays "Squish" sounds on high-impact NPC collisions.
/// </summary>
public class CinematicGore : MonoBehaviour
{
    public static CinematicGore Instance { get; private set; }

    [Header("Gore Settings")]
    public float thresholdVelocity = 40f;
    public AudioClip[] squishSounds;
    public Color bloodColor = new Color(0.6f, 0, 0, 0.8f);

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Called when a car hits a pedestrian.
    /// </summary>
    public void TriggerGore(Vector3 position, float velocity)
    {
        if (velocity < thresholdVelocity) return;

        // Spawn blood particles using primitives
        SpawnBloodParticles(position, velocity);

        // Play sound
        if (CinematicAudioManager.Instance != null && squishSounds != null && squishSounds.Length > 0)
        {
            CinematicAudioManager.Instance.PlayEffect(squishSounds[Random.Range(0, squishSounds.Length)], position);
        }

        // Screen shake for impact
        if (Camera.main != null)
        {
            // Simple shake simulation
        }
    }

    private void SpawnBloodParticles(Vector3 pos, float velocity)
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject blood = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            blood.name = "BloodDrop";
            blood.transform.position = pos + Random.insideUnitSphere * 0.5f;
            blood.transform.localScale = Vector3.one * Random.Range(0.05f, 0.15f);
            
            Renderer r = blood.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Standard"));
            r.material.color = bloodColor;
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", bloodColor * 0.5f);

            Rigidbody rb = blood.AddComponent<Rigidbody>();
            rb.velocity = Random.onUnitSphere * (velocity * 0.2f);
            rb.mass = 0.1f;

            Destroy(blood, 1.5f);
        }
    }
}
