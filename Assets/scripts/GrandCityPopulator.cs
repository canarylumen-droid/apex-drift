using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Grand City Populator
/// Wires up the 5000+ imported open-source/super-realistic FBXs into the game.
/// Spawns civilians, police, helicopters, traffic, and props (like spike strips) 
/// across the massive city map and even in the main menu!
/// </summary>
public class GrandCityPopulator : MonoBehaviour
{
    public static GrandCityPopulator Instance { get; private set; }

    [Header("Imported Prefabs (Assign from Import_Queue)")]
    public GameObject[] citizenPrefabs;
    public GameObject[] clubNPCsPrefabs; // e.g. smoking, dancing, panties
    public GameObject[] trafficCars;
    public GameObject[] policeCars;
    public GameObject helicopterPrefab;
    public GameObject spikeStripPrefab;
    
    [Header("Spawning Limits")]
    public int maxCityCivilians = 5000;
    public int maxTraffic = 500;
    public float cityRadius = 5000f; // Massive open world

    [Header("Locations")]
    public Transform clubInteriorLocation;
    public Transform mainBridgeLocation;
    public Transform playerHomeLocation;
    public Transform mainMenuLocation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across menu and gameplay
    }

    private void Start()
    {
        // 1. Populate Main Menu immediately if we are in the menu scene
        PopulateMainMenu();

        // 2. Start bulk spawning for the actual city if we are in gameplay
        if (FindObjectOfType<RCC_CarControllerV3>() != null)
        {
            GenerateCityLife();
            GenerateTraffic();
            PopulateClubInterior();
            SetupBridgeAndHome();
            SpawnPoliceAndHelicopters();
        }
    }

    private void PopulateMainMenu()
    {
        if (mainMenuLocation == null) return;
        
        Debug.Log("Populating Main Menu with imported humans and cars...");
        for (int i = 0; i < 15; i++)
        {
            Vector3 spawnPos = mainMenuLocation.position + Random.insideUnitSphere * 10f;
            spawnPos.y = mainMenuLocation.position.y;
            if (citizenPrefabs != null && citizenPrefabs.Length > 0)
            {
                Instantiate(citizenPrefabs[Random.Range(0, citizenPrefabs.Length)], spawnPos, Quaternion.identity);
            }
        }
    }

    private void GenerateCityLife()
    {
        Debug.Log($"Spawning {maxCityCivilians} dynamic civilians across the city...");
        // Spawns civilians procedurally
        for (int i = 0; i < maxCityCivilians; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-cityRadius, cityRadius), 0, Random.Range(-cityRadius, cityRadius));
            if (citizenPrefabs != null && citizenPrefabs.Length > 0)
            {
                GameObject npc = Instantiate(citizenPrefabs[Random.Range(0, citizenPrefabs.Length)], spawnPos, Quaternion.identity);
                // Add roaming AI
                if(npc.GetComponent<NPCRoamingAI>() == null)
                    npc.AddComponent<NPCRoamingAI>();
            }
        }
    }

    private void GenerateTraffic()
    {
        for (int i = 0; i < maxTraffic; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-cityRadius, cityRadius), 0, Random.Range(-cityRadius, cityRadius));
            if (trafficCars != null && trafficCars.Length > 0)
            {
                GameObject car = Instantiate(trafficCars[Random.Range(0, trafficCars.Length)], spawnPos, Quaternion.identity);
                if (car.GetComponent<TrafficAI>() == null)
                    car.AddComponent<TrafficAI>();
            }
        }
    }

    private void PopulateClubInterior()
    {
        if (clubInteriorLocation == null) return;

        Debug.Log("Populating the Club interior with specific NPCs (dancing, smoking, etc)...");
        for (int i = 0; i < 50; i++) // 50 people in the club
        {
            Vector3 spawnPos = clubInteriorLocation.position + Random.insideUnitSphere * 20f;
            spawnPos.y = clubInteriorLocation.position.y;
            
            if (clubNPCsPrefabs != null && clubNPCsPrefabs.Length > 0)
            {
                GameObject clubber = Instantiate(clubNPCsPrefabs[Random.Range(0, clubNPCsPrefabs.Length)], spawnPos, Quaternion.identity);
                // Wire up animations
                Animator anim = clubber.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetBool("IsSmoking", Random.value > 0.5f);
                    anim.SetBool("IsDancing", Random.value > 0.3f);
                }
            }
        }
    }

    private void SetupBridgeAndHome()
    {
        // Spawns traffic/missions near the bridge and player's home
        if (mainBridgeLocation != null)
        {
            // Spawn some spike strips for police chases?
            Instantiate(spikeStripPrefab, mainBridgeLocation.position + Vector3.forward * 50f, Quaternion.identity);
        }
    }

    private void SpawnPoliceAndHelicopters()
    {
        // Spawns the hovering army/police helicopters and police cars patrolling
        if (helicopterPrefab != null)
        {
            Instantiate(helicopterPrefab, new Vector3(0, 100, 0), Quaternion.identity);
        }
    }
}
