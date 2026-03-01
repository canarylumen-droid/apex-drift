using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes pedestrians wander between random NavMesh points.
/// Creates the "alive" walking behavior for 3D humanoids.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianWander : MonoBehaviour
{
    public float wanderRadius = 20f;
    public float idleTime = 3f;

    private NavMeshAgent agent;
    private float idleTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
    }

    void Update()
    {
        if (agent == null) return;

        // When destination reached, idle then pick new one
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleTime)
            {
                SetNewDestination();
                idleTimer = 0;
            }
        }
    }

    private void SetNewDestination()
    {
        Vector3 randomDir = transform.position + Random.insideUnitSphere * wanderRadius;
        randomDir.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
