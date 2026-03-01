using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// GTA-style Mission Manager.
/// Handles high-speed delivery, police escapes, and street races.
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public enum MissionType { Delivery, Escape, Race }
    public bool isMissionActive = false;
    
    private MissionType currentMission;
    private float timer;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void StartMission(MissionType type)
    {
        if (isMissionActive) return;
        
        isMissionActive = true;
        currentMission = type;
        timer = 60f; // Default 1 min missions

        Debug.Log("[MissionManager] Started Mission: " + type);
    }

    void Update()
    {
        if (!isMissionActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            FailMission();
        }
    }

    public void CompleteMission()
    {
        isMissionActive = false;
        Debug.Log("[MissionManager] Mission Completed!");
    }

    public void FailMission()
    {
        isMissionActive = false;
        Debug.Log("[MissionManager] Mission Failed!");
    }
}
