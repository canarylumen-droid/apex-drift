using UnityEngine;
using System.Collections.Generic;

public enum MissionType
{
    Assassination, // Target a specific imported NPC
    StealCar,      // Steal a specific imported car
    DeliverPackage, // Go to the club or bridge
    Survival // Survive police AI / army helicopters for X minutes
}

public class DynamicMissionGenerator : MonoBehaviour
{
    public static DynamicMissionGenerator Instance { get; private set; }

    [Header("Mission UI")]
    public string currentMissionText = "Explore the city.";
    private MissionType currentMissionType;
    private GameObject currentMissionTarget;

    private float _missionTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Start generating missions after a delay once the city is populated
        InvokeRepeating(nameof(GenerateNewMission), 10f, 120f);
    }

    public void GenerateNewMission()
    {
        currentMissionType = (MissionType)Random.Range(0, 4);

        switch (currentMissionType)
        {
            case MissionType.Assassination:
                // Find a random spawned civilian
                NPCRoamingAI[] npcs = FindObjectsOfType<NPCRoamingAI>();
                if (npcs.Length > 0)
                {
                    currentMissionTarget = npcs[Random.Range(0, npcs.Length)].gameObject;
                    currentMissionText = "Mission: Eliminate the target marked in red.";
                    MarkTarget(currentMissionTarget, Color.red);
                }
                break;

            case MissionType.StealCar:
                // Find a random traffic car
                TrafficAI[] cars = FindObjectsOfType<TrafficAI>();
                if (cars.Length > 0)
                {
                    currentMissionTarget = cars[Random.Range(0, cars.Length)].gameObject;
                    currentMissionText = "Mission: Steal the marked vehicle and escape the police.";
                    MarkTarget(currentMissionTarget, Color.yellow);
                }
                break;

            case MissionType.DeliverPackage:
                currentMissionText = "Mission: Deliver the package to the Club interior.";
                break;

            case MissionType.Survival:
                currentMissionText = "Mission: Survive a 5-star police chase for 2 minutes!";
                WantedSystem ws = FindObjectOfType<WantedSystem>();
                if (ws != null) ws.AddWantedLevel(5); // Trigger high level pursuit
                break;
        }

        Debug.Log("NEW MISSION: " + currentMissionText);

        // Update the HUD (assuming CinematicHUD or PremiumHUD exists)
        if (CinematicHUD.Instance != null)
        {
            CinematicHUD.Instance.UpdateMissionText(currentMissionText);
        }
    }

    private void MarkTarget(GameObject target, Color markColor)
    {
        // Add a simple halo or marker to the target
        Light marker = target.AddComponent<Light>();
        marker.color = markColor;
        marker.range = 5f;
        marker.intensity = 5f;
        marker.type = LightType.Point;
    }
}
