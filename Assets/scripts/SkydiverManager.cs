using UnityEngine;

/// <summary>
/// Controls random skydiving events to make the world look interactive and "video gamey".
/// </summary>
public class SkydiverManager : MonoBehaviour
{
    public GameObject skydiverPrefab;
    public Transform[] spawnPoints;
    public float minSpawnInterval = 20f;
    public float maxSpawnInterval = 60f;

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnSkydiver();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnSkydiver()
    {
        if (skydiverPrefab == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject skydiver = Instantiate(skydiverPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Add some random movement/drift
        Rigidbody rb = skydiver.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.down * 5f, ForceMode.Acceleration);
        }

        Destroy(skydiver, 30f); // Clean up after a while
        Debug.Log("[SkyEvents] Skydiver deployed for cinematic effect.");
    }
}
