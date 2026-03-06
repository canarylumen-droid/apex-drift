using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 11: Story Mission Controller.
/// Manages high-stakes missions like Drug deals, Package deliveries, and Escapes.
/// </summary>
public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    public enum MissionStatus { NotStarted, InProgress, Success, Failed }
    
    [System.Serializable]
    public class Mission
    {
        public string title;
        public string description;
        public MissionStatus status;
        public int reward;
        public float timeLimit;
        public bool requiresWantedLevel;
    }

    public List<Mission> availableMissions = new List<Mission>();
    public Mission activeMission;

    void Awake()
    {
        if (Instance == null) Instance = this;
        InitializeDefaultMissions();
    }

    private void InitializeDefaultMissions()
    {
        availableMissions.Add(new Mission { 
            title = "The White Package", 
            description = "Deliver the narcotics to the South Side Club.", 
            reward = 5000, 
            timeLimit = 120f 
        });
        
        availableMissions.Add(new Mission { 
            title = "Police Bait", 
            description = "Lead the cops on a 3-minute chase and escape.", 
            reward = 10000, 
            requiresWantedLevel = true 
        });
    }

    public void StartMission(int index)
    {
        if (index < 0 || index >= availableMissions.Count) return;
        
        activeMission = availableMissions[index];
        activeMission.status = MissionStatus.InProgress;
        
        Debug.Log("[Mission] Started: " + activeMission.title);
        // Trigger UI Notification
        if (PremiumHUD.Instance != null) PremiumHUD.Instance.ShowNotification("MISSION STARTED: " + activeMission.title);

        // Spawn Visual Waypoint
        SpawnMissionWaypoint(activeMission);
    }

    private void SpawnMissionWaypoint(Mission mission)
    {
        // Pick a random destination or read from mission data.
        // For now, spawn it 500 meters ahead of player
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null)
        {
            Vector3 dropZone = player.transform.position + player.transform.forward * 500f;
            // Floor it to ground level roughly
            dropZone.y = 0; 

            GameObject waypoint = new GameObject("MissionWaypoint");
            waypoint.transform.position = dropZone;
            
            // Add a Trigger Collider so the car can enter it
            BoxCollider bc = waypoint.AddComponent<BoxCollider>();
            bc.size = new Vector3(20, 20, 20);
            bc.isTrigger = true;

            waypoint.AddComponent<MissionWaypointMarker>();
        }
    }

    public void CompleteMission()
    {
        if (activeMission == null) return;
        
        activeMission.status = MissionStatus.Success;
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCurrency(activeMission.reward);
        if (ReputationSystem.Instance != null) ReputationSystem.Instance.AddReputation(activeMission.reward / 10); // Rep is 10% of cash reward

        // Phase 15: Trigger Jubilation!
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null && SocialEventController.Instance != null)
        {
            SocialEventController.Instance.TriggerJubilation(player.transform.position);
        }
        
        Debug.Log("[Mission] Completed: " + activeMission.title);
        activeMission = null;
    }
}
