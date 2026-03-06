using UnityEngine;

/// <summary>
/// Phase 7: Mission Waypoint Marker.
/// Spawns a physical, glowing 3D marker (like a flare or light pillar) at mission destinations
/// to replace text-only location hints.
/// </summary>
public class MissionWaypointMarker : MonoBehaviour
{
    private Light markerLight;
    private ParticleSystem flareParticles;
    private GameObject pillarMesh;

    void Awake()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        // 1. Tall glowing pillar
        pillarMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillarMesh.transform.SetParent(transform);
        pillarMesh.transform.localPosition = new Vector3(0, 5f, 0);
        pillarMesh.transform.localScale = new Vector3(2f, 10f, 2f);
        
        Collider col = pillarMesh.GetComponent<Collider>();
        if (col != null) col.isTrigger = true; // Don't crash into it

        Renderer r = pillarMesh.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Unlit/Transparent"));
        r.material.color = new Color(0, 1f, 0.5f, 0.4f); // Neon Green

        // 2. Light Source
        markerLight = gameObject.AddComponent<Light>();
        markerLight.type = LightType.Point;
        markerLight.color = Color.green;
        markerLight.range = 30f;
        markerLight.intensity = 5f;

        // Animate up and down slightly
        StartCoroutine(FloatAnimation());
    }

    private System.Collections.IEnumerator FloatAnimation()
    {
        float startY = transform.position.y;
        while (true)
        {
            float newY = startY + Mathf.Sin(Time.time * 2f) * 1.5f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            markerLight.intensity = 5f + Mathf.Sin(Time.time * 5f) * 2f; // Pulsate light
            yield return null;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // When player drives into the marker, complete the specific stage
        if (other.CompareTag("Player") || other.GetComponent<RCC_CarControllerV3>() != null)
        {
            Debug.Log("[MissionWaypoint] Reached destination!");
            if (MissionController.Instance != null && MissionController.Instance.activeMission != null)
            {
               MissionController.Instance.CompleteMission();
            }
            Destroy(gameObject);
        }
    }
}
