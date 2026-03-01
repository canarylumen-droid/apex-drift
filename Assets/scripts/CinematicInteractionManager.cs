using UnityEngine;
using System.Collections;

/// <summary>
/// Handles GTA-style car entering/exiting and player states like 'Drunk'.
/// </summary>
public class CinematicInteractionManager : MonoBehaviour
{
    public static CinematicInteractionManager Instance { get; private set; }

    [Header("Player States")]
    public bool isDrunk = false;
    public bool isRacing = false;
    public bool insideVehicle = true;

    [Header("Jubilation Settings")]
    public GameObject playerModel; // Assign a humanoid model if available
    public AudioClip victoryCheer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// GTA-style entry/exit logic placeholder
    /// </summary>
    public void ToggleVehicleEntry(bool entering)
    {
        insideVehicle = entering;
        Debug.Log(entering ? "[Interaction] Player entered vehicle." : "[Interaction] Player exited vehicle.");
        
        // Logic to switch between Humanoid Controller and RCC Car Controller would go here
    }

    /// <summary>
    /// Triggers the "Drunk" state for cinematic chaos
    /// </summary>
    public void SetDrunkState(bool active)
    {
        isDrunk = active;
        Debug.Log(active ? "[Interaction] Player is DRUNK. Handling will swerve!" : "[Interaction] Player is sober.");
    }

    /// <summary>
    /// Triggers the victory sequence
    /// </summary>
    public void TriggerVictory()
    {
        Debug.Log("[Interaction] RACE WON! Jubilating...");
        
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("VictoryCheer", 1.0f);

        // Logic to play 'Jubilate' animation on the playerModel
    }
}
