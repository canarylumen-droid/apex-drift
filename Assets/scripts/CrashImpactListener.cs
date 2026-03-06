using UnityEngine;

/// <summary>
/// Helper component added to the player car to redirect collision data to the CinematicCrashEngine.
/// </summary>
public class CrashImpactListener : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        if (CinematicCrashEngine.Instance != null)
        {
            CinematicCrashEngine.Instance.RegisterImpact(collision, impactForce);
        }
    }
}
