using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Phase 6: Mission Matrix Manager.
/// Manages the massive 50-mission story progression and transitions.
/// </summary>
public class MissionMatrixManager : MonoBehaviour
{
    public static MissionMatrixManager Instance { get; private set; }

    [Header("Mission Database")]
    public List<MissionData> allMissions = new List<MissionData>();
    public int currentMissionIndex = 0;
    public int currentStepIndex = 0;

    private bool stepInProgress = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void StartStoryMatrix()
    {
        if (allMissions.Count == 0) return;
        LoadMission(currentMissionIndex);
    }

    private void LoadMission(int index)
    {
        if (index >= allMissions.Count)
        {
            Debug.Log("[MissionMatrix] All Story Missions Completed!");
            return;
        }

        currentMissionIndex = index;
        currentStepIndex = 0;
        ExecuteCurrentStep();
    }

    private void ExecuteCurrentStep()
    {
        if (stepInProgress) return;
        
        MissionData mission = allMissions[currentMissionIndex];
        if (currentStepIndex >= mission.steps.Count)
        {
            CompleteCurrentMission();
            return;
        }

        MissionStep step = mission.steps[currentStepIndex];
        Debug.Log($"[MissionMatrix] Step: {step.stepName} of Mission: {mission.title}");

        // 1. Voice the Step Dialogue automatically
        if (!string.IsNullOrEmpty(step.dialogueText) && AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.Speak(step.dialogueText, step.dialogueEmotion);
        }

        // 2. Add Map Icon for targets
        if (step.type == MissionStep.StepType.DriveTo && MissionIconManager.Instance != null)
        {
            // Create a dummy target object at targetPosition
            GameObject marker = new GameObject("StepMarker");
            marker.transform.position = step.targetPosition;
            MissionIconManager.Instance.TrackTarget(marker.transform, Resources.Load<Sprite>("Textures/UI_Gold"));
        }

        // 3. Situational Transition (Slow-mo)
        if (step.type == MissionStep.StepType.Cutscene && CinematicDirector.Instance != null)
        {
            CinematicDirector.Instance.TriggerEventCamera(step.targetPosition, 5f, step.slowmoSpeed);
        }
    }

    public void OnStepObjectiveReached()
    {
        Debug.Log("[MissionMatrix] Objective Reached!");
        
        // Remove tracking of previous target
        MissionStep step = allMissions[currentMissionIndex].steps[currentStepIndex];
        // logic to remove icon...

        currentStepIndex++;
        ExecuteCurrentStep();
    }

    void Update()
    {
        // Simple proximity check for 'DriveTo' steps
        if (allMissions.Count > 0 && currentMissionIndex < allMissions.Count)
        {
            MissionData mission = allMissions[currentMissionIndex];
            if (currentStepIndex < mission.steps.Count)
            {
                MissionStep step = mission.steps[currentStepIndex];
                if (step.type == MissionStep.StepType.DriveTo)
                {
                    RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
                    if (player != null && Vector3.Distance(player.transform.position, step.targetPosition) < 10f)
                    {
                        OnStepObjectiveReached();
                    }
                }
            }
        }
    }

    private void CompleteCurrentMission()
    {
        MissionData mission = allMissions[currentMissionIndex];
        Debug.Log($"[MissionMatrix] {mission.title} COMPLETE!");
        
        if (EconomyExpansion.Instance != null) 
            Debug.Log($"Reward: ${mission.rewardMoney}");

        currentMissionIndex++;
        
        // Unlock next mission in list if exists
        if (currentMissionIndex < allMissions.Count)
        {
            allMissions[currentMissionIndex].isUnlocked = true;
            PlayerPrefs.SetInt($"Mission_{allMissions[currentMissionIndex].missionID}_Unlocked", 1);
        }

        mission.isCompleted = true;
        PlayerPrefs.SetInt($"Mission_{mission.missionID}_Completed", 1);
        PlayerPrefs.Save();

        // Show Save notification
        if (ApexSaveSystem.Instance != null) ApexSaveSystem.Instance.SaveGame();
        
        LoadMission(currentMissionIndex);
    }

    public bool IsMissionUnlocked(string missionID)
    {
        return PlayerPrefs.GetInt($"Mission_{missionID}_Unlocked", 0) == 1;
    }
}
