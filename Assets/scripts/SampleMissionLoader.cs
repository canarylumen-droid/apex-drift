using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 6: Sample Mission Loader.
/// Hardcodes the first "Master Mission" into the Matrix as a demo.
/// Drive -> Club -> Sexy Interaction -> Police Ambush.
/// </summary>
public class SampleMissionLoader : MonoBehaviour
{
    void Start()
    {
        if (MissionMatrixManager.Instance == null) return;

        MissionData mission1 = ScriptableObject.CreateInstance<MissionData>();
        mission1.missionID = "STORY_01";
        mission1.title = "The Neon Handshake";
        mission1.rewardMoney = 5000;

        // Step 1: Drive to Club
        mission1.steps.Add(new MissionStep {
            stepName = "Drive to the Club",
            type = MissionStep.StepType.DriveTo,
            targetPosition = new Vector3(100, 0, 100), // Club Entrance
            dialogueText = "Meeting the contact at the Neon Lounge. Drive smooth, don't attract attention.",
            dialogueEmotion = AdvancedProceduralTTS.Emotion.Neutral
        });

        // Step 2: Club Interior (Cutscene)
        mission1.steps.Add(new MissionStep {
            stepName = "Meet the Contact",
            type = MissionStep.StepType.Cutscene,
            targetPosition = new Vector3(100.5f, 1f, 100.5f),
            dialogueText = "She's waiting in the back. Watch those hips, kid.",
            dialogueEmotion = AdvancedProceduralTTS.Emotion.Romantic,
            slowmoSpeed = 0.8f
        });

        // Step 3: Ambush (Police)
        mission1.steps.Add(new MissionStep {
            stepName = "Escape the Cops",
            type = MissionStep.StepType.Chase,
            dialogueText = "Cops! It's a setup! Get the car and get out of here now!",
            dialogueEmotion = AdvancedProceduralTTS.Emotion.Panicked
        });

        MissionMatrixManager.Instance.allMissions.Add(mission1);
        Debug.Log("[SampleMission] Registered 'The Neon Handshake' into the Matrix.");
    }
}
