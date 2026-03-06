using UnityEngine;

/// <summary>
/// Phase 13: Intelligent Traffic AI.
/// Handles lane following, collision avoidance, and speed control.
/// </summary>
public class TrafficAI : MonoBehaviour
{
    public float targetSpeed = 40f;
    public float brakeDistance = 10f;
    public float detectionRadius = 15f;
    
    [Header("Path Following")]
    public LaneWaypoint currentWaypoint;
    public float steeringSensitivity = 5f;
    
    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        rb.mass = 1500f;
        rb.drag = 0.5f;
        rb.angularDrag = 5f;

        if (currentWaypoint == null) FindNearestWaypoint();
    }

    private void FindNearestWaypoint()
    {
        LaneWaypoint[] allWaypoints = FindObjectsOfType<LaneWaypoint>();
        float minDist = float.MaxValue;
        foreach (var wp in allWaypoints)
        {
            float d = Vector3.Distance(transform.position, wp.transform.position);
            if (d < minDist)
            {
                minDist = d;
                currentWaypoint = wp;
            }
        }
    }

    void FixedUpdate()
    {
        if (currentWaypoint == null) return;

        HandleMovement();
        HandleSteering();
        AvoidObstacles();
        CheckWaypointProgress();
    }

    private void HandleMovement()
    {
        rb.AddForce(transform.forward * targetSpeed * 5f);
        if (rb.velocity.magnitude > targetSpeed / 3.6f)
        {
            rb.velocity = rb.velocity.normalized * (targetSpeed / 3.6f);
        }
    }

    private void HandleSteering()
    {
        Vector3 targetDir = (currentWaypoint.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.right, targetDir);
        rb.AddTorque(Vector3.up * dot * steeringSensitivity * 100f);
    }

    private void CheckWaypointProgress()
    {
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < 5f)
        {
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0 && Random.value > 0.5f)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            }
            else
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
            }
        }
    }

    private void AvoidObstacles()
    {
        RaycastHit hit;
        // Front raycast
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, detectionRadius))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.GetComponent<TrafficAI>() != null)
            {
                // Brake!
                rb.velocity *= 0.9f;
                Debug.Log("[TrafficAI] Braking for obstacle: " + hit.collider.name);
            }
        }
    }
}
