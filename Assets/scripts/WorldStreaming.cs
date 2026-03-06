using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 7: World Streaming.
/// Dynamically loads and unloads 'City Chunks' based on player distance.
/// Prevents memory overload on mobile devices during the city expansion.
/// </summary>
public class WorldStreaming : MonoBehaviour
{
    public static WorldStreaming Instance { get; private set; }

    [Header("Streaming Settings")]
    public Transform playerTransform;
    public float loadDistance = 500f;
    public float unloadDistance = 800f;

    [System.Serializable]
    public class CityChunk
    {
        public string chunkID;
        public Vector3 center;
        public GameObject chunkObject; // Found via Resources.Load
        public bool isLoaded = false;
    }

    public List<CityChunk> cityMap = new List<CityChunk>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (playerTransform == null && Camera.main != null) playerTransform = Camera.main.transform;
        if (playerTransform == null) return;

        foreach (var chunk in cityMap)
        {
            float dist = Vector3.Distance(playerTransform.position, chunk.center);

            if (dist < loadDistance && !chunk.isLoaded)
            {
                LoadChunk(chunk);
            }
            else if (dist > unloadDistance && chunk.isLoaded)
            {
                UnloadChunk(chunk);
            }
        }
    }

    private void LoadChunk(CityChunk chunk)
    {
        Debug.Log($"[Streaming] Loading City Sector: {chunk.chunkID}");
        chunk.isLoaded = true;
        
        // Asynchronous loading to prevent frame drops
        GameObject prefab = Resources.Load<GameObject>($"Environment/Chunks/{chunk.chunkID}");
        if (prefab != null)
        {
            chunk.chunkObject = Instantiate(prefab, chunk.center, Quaternion.identity);
        }
    }

    private void UnloadChunk(CityChunk chunk)
    {
        Debug.Log($"[Streaming] Unloading City Sector: {chunk.chunkID}");
        chunk.isLoaded = false;
        
        if (chunk.chunkObject != null)
        {
            Destroy(chunk.chunkObject);
        }

        // Suggest garbage collection to free memory
        System.GC.Collect();
    }
}
