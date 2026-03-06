using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 13: Lane Waypoint.
/// Defines the nodes for traffic paths. AI vehicles follow these in sequence.
/// </summary>
public class LaneWaypoint : MonoBehaviour
{
    public LaneWaypoint nextWaypoint;
    public List<LaneWaypoint> branches; // For intersections
    
    [Header("Lane Settings")]
    public float laneWidth = 4f;
    public bool isIntersection = false;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);
        
        if (nextWaypoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, nextWaypoint.transform.position);
        }

        if (branches != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var branch in branches)
            {
                if (branch != null) Gizmos.DrawLine(transform.position, branch.transform.position);
            }
        }
    }
}
