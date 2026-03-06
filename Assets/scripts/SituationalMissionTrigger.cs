using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 5: Situational Mission Trigger.
/// Starts missions "right there" in the world when entering a zone.
/// Seamlessly transitions to a slow-mo cutscene before returning to gameplay.
/// </summary>
public class SituationalMissionTrigger : MonoBehaviour
{
    public string missionID;
    public string missionDialogue = "We have a job to do. Get in the car.";
    public float cinematicSlowMo = 0.8f;
    
    private bool missionStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (missionStarted) return;

        RCC_CarControllerV3 car = other.GetComponentInParent<RCC_CarControllerV3>();
        if (car != null && car.canControl && car.GetComponent<RCC_AICarController>() == null)
        {
            BeginSituationalMission();
        }
    }

    private void BeginSituationalMission()
    {
        missionStarted = true;
        Debug.Log($"[SituationalMission] Starting mission: {missionID} right here.");

        if (CinematicDirector.Instance != null)
        {
            // 1. Slow down and focus camera
            CinematicDirector.Instance.TriggerEventCamera(transform.position, 4f, cinematicSlowMo);
            
            // 2. Play Dialogue
            if (AdvancedProceduralTTS.Instance != null)
            {
                AdvancedProceduralTTS.Instance.Speak(missionDialogue, AdvancedProceduralTTS.Emotion.Neutral);
            }

            // 3. Register with MissionManager
            if (MissionManager.Instance != null)
            {
                // In a real build, we'd pass mission metadata here
                MissionManager.Instance.StartMission(missionID);
            }
        }
    }
}
