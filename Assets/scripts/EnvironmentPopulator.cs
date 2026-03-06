using UnityEngine;

/// <summary>
/// Phase 10: Environmental Populator.
/// Randomly spawns city props (benches, streetlights) along the road to make the world look real.
/// </summary>
public class EnvironmentPopulator : MonoBehaviour
{
    public GameObject streetlightPrefab;
    public GameObject benchPrefab;
    public float spacing = 20f;

    public void PopulateStreet(Vector3 start, Vector3 end)
    {
        float dist = Vector3.Distance(start, end);
        int count = Mathf.FloorToInt(dist / spacing);
        
        Vector3 direction = (end - start).normalized;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = start + direction * (i * spacing);
            
            // Side of road offset
            Vector3 sideOffset = Vector3.Cross(direction, Vector3.up) * 5f;

            // Spawn Streetlight
            if (streetlightPrefab != null) Instantiate(streetlightPrefab, pos + sideOffset, Quaternion.identity);
            
            // Spawn Bench randomly
            if (benchPrefab != null && Random.value > 0.6f)
                Instantiate(benchPrefab, pos + sideOffset + direction * 2f, Quaternion.LookRotation(-sideOffset));
        }
    }
}
