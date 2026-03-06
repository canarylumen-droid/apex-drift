using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 13: Traffic Manager.
/// Manages global traffic density and spawns AI vehicles.
/// </summary>
public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    [Header("Traffic Settings")]
    public GameObject[] trafficVehiclePrefabs;
    public int maxTrafficDensity = 20;
    public float spawnRadius = 100f;
    public float despawnRadius = 150f;

    private List<GameObject> activeVehicles = new List<GameObject>();
    private Transform playerTransform;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;
        
        MaintainTrafficDensity();
    }

    private void MaintainTrafficDensity()
    {
        // Cleanup distant vehicles
        for (int i = activeVehicles.Count - 1; i >= 0; i--)
        {
            if (activeVehicles[i] == null || Vector3.Distance(activeVehicles[i].transform.position, playerTransform.position) > despawnRadius)
            {
                Destroy(activeVehicles[i]);
                activeVehicles.RemoveAt(i);
            }
        }

        // Spawn new vehicles
        if (activeVehicles.Count < maxTrafficDensity)
        {
            SpawnTrafficVehicle();
        }
    }

    private void SpawnTrafficVehicle()
    {
        GameObject prefab = null;
        if (trafficVehiclePrefabs != null && trafficVehiclePrefabs.Length > 0)
        {
            prefab = trafficVehiclePrefabs[Random.Range(0, trafficVehiclePrefabs.Length)];
        }
        else if (RealisticModelManager.Instance != null && RealisticModelManager.Instance.carModel1Prefab != null)
        {
            prefab = RealisticModelManager.Instance.carModel1Prefab;
        }

        if (prefab == null) return;

        Vector3 spawnPos = playerTransform.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = 0;

        GameObject vehicle = Instantiate(prefab, spawnPos, Quaternion.identity);
        vehicle.AddComponent<TrafficAI>();
        activeVehicles.Add(vehicle);
    }
}
