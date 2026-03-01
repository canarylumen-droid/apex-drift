using UnityEngine;

/// <summary>
/// Makes any object destructible upon impact.
/// Objects fly apart and play "crash" sounds when hit by high-speed cars.
/// Attach this to Fences, Poles, Crates, and Barrels.
/// </summary>
public class DestructibleProp : MonoBehaviour
{
    public float breakForce = 15f;
    public AudioClip breakSound;
    public GameObject breakEffect;
    public bool isRespawnable = true;
    public float respawnTime = 30f;

    private Vector3 initialPos;
    private Quaternion initialRot;
    private Rigidbody rb;
    private bool isBroken = false;

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 20f;
            rb.drag = 0.5f;
            rb.isKinematic = true; // Stay in place until hit
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        // Check if hit by a car at high speed
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<RCC_CarControllerV3>() != null)
        {
            float impactVelocity = collision.relativeVelocity.magnitude;
            
            if (impactVelocity > breakForce)
            {
                TriggerBreak(collision.relativeVelocity);
            }
        }
    }

    private void TriggerBreak(Vector3 force)
    {
        isBroken = true;
        rb.isKinematic = false;
        rb.AddForce(force * 2f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * force.magnitude, ForceMode.Impulse);

        // Play sound via CinematicAudioManager
        if (CinematicAudioManager.Instance != null && breakSound != null)
        {
            CinematicAudioManager.Instance.PlayEffect(breakSound, transform.position);
        }

        // Spawn debris effect
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Raise Wanted level if hitting city property
        if (WantedSystem.Instance != null)
        {
            WantedSystem.Instance.RegisterOffense(WantedSystem.OffenseType.PropertyDamage);
        }

        if (isRespawnable)
        {
            Invoke("Respawn", respawnTime);
        }
    }

    private void Respawn()
    {
        rb.isKinematic = true;
        transform.position = initialPos;
        transform.rotation = initialRot;
        isBroken = false;
    }
}
