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

    [Header("Cameras")]
    public Camera characterCamera;
    public Camera carCamera;
    
    [Header("Jubilation Settings")]
    public GameObject playerModel; 
    public string jubilationAnimParam = "isJubilating";

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
    /// GTA-style entry/exit logic with camera transitions
    /// </summary>
    public void ToggleVehicleEntry(bool entering)
    {
        insideVehicle = entering;
        
        if (characterCamera != null) characterCamera.gameObject.SetActive(!entering);
        if (carCamera != null) carCamera.gameObject.SetActive(entering);

        Debug.Log(entering ? "[Interaction] Player entered vehicle. Switching to Car Cam." : "[Interaction] Player exited vehicle. Switching to Foot Cam.");
    }

    /// <summary>
    /// Triggers the "Drunk" state for cinematic chaos
    /// </summary>
    public void SetDrunkState(bool active)
    {
        isDrunk = active;
        // Hook for Post-Processing (Chromatic Aberration/Blur) would go here
    }

    /// <summary>
    /// Triggers the victory sequence with Animation and Audio
    /// </summary>
    public void TriggerVictory()
    {
        isRacing = false;
        
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("VictoryCheer", 1.0f);

        if (playerModel != null)
        {
            Animator anim = playerModel.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger(jubilationAnimParam);
        }

        // Slow down time for cinematic effect
        Time.timeScale = 0.5f;
        StartCoroutine(ResetTimeScale());
    }

    private IEnumerator ResetTimeScale()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1.0f;
    }
}
