using UnityEngine;

/// <summary>
/// Phase 5: Army/SWAT Unit.
/// Spawns from helicopters. Freefalls to the ground then chases the player.
/// </summary>
public class ArmyUnit : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasLanded = false;
    private Transform playerTarget;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        rb.mass = 80f;
        rb.drag = 1f; // Slow down fall slightly (parachute simulation)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animator = GetComponent<Animator>();

        FindPlayer();
    }

    private void FindPlayer()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
            {
                playerTarget = car.transform;
                break;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Untagged") || collision.gameObject.name.ToLower().Contains("road") || collision.gameObject.name.ToLower().Contains("terrain"))
        {
            hasLanded = true;
            rb.drag = 0f; // Normal movement drag
            if (animator != null) animator.SetTrigger("Landed"); // Play landing animation if available
            Debug.Log("[ArmyUnit] Landed on ground. Engaging pursuit.");
        }
    }

    void Update()
    {
        if (!hasLanded || playerTarget == null) return;

        // Ground Pursuit Logic: Run towards player car to disable it
        Vector3 dir = (playerTarget.position - transform.position).normalized;
        dir.y = 0; // Keep on horizontal plane

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        
        // Move forward
        transform.position += transform.forward * Time.deltaTime * 6f; // Run speed

        // If too close, shoot/bash!
        if (Vector3.Distance(transform.position, playerTarget.position) < 4f)
        {
            // Do damage to car, or pull player out
             Debug.Log("[ArmyUnit] Engaging player vehicle!");
        }
    }
}
