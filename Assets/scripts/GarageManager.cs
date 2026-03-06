using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 14: Interactive Garage Manager.
/// Handles car rotation, switching, and customization visuals.
/// </summary>
public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    [Header("Garage Settings")]
    public Transform carPivot;
    public float rotationSpeed = 20f;
    public List<GameObject> carPrefabs = new List<GameObject>();
    
    private int currentCarIndex = 0;
    private GameObject spawnedCar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadCarPrefabs();
    }

    private void LoadCarPrefabs()
    {
        // Load the high-fidelity models we imported in Phase 8
        GameObject car1 = Resources.Load<GameObject>("Models/CarModel_1");
        if (car1 != null) carPrefabs.Add(car1);
        
        // Add existing ones if found...
    }

    void Start()
    {
        ShowCar(0);
    }

    void Update()
    {
        if (spawnedCar != null)
        {
            spawnedCar.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void ShowCar(int index)
    {
        if (carPrefabs == null || carPrefabs.Count == 0) return;
        
        if (spawnedCar != null) Destroy(spawnedCar);
        
        currentCarIndex = index;
        spawnedCar = Instantiate(carPrefabs[currentCarIndex], carPivot.position, carPivot.rotation);
        spawnedCar.transform.SetParent(carPivot);
    }

    public void NextCar()
    {
        int next = (currentCarIndex + 1) % carPrefabs.Count;
        ShowCar(next);
    }

    public void SelectCar()
    {
        Debug.Log("[Garage] Selected vehicle: " + carPrefabs[currentCarIndex].name);
        // Save selection to PlayerPrefs or PlayerData
    }
}
