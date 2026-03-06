using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 6: Mission Data. 
/// ScriptableObject to define complex missions without hardcoding.
/// </summary>
[CreateAssetMenu(fileName = "NewMission", menuName = "ApexDrift/Mission")]
public class MissionData : ScriptableObject
{
    public string missionID;
    public string title;
    [TextArea] public string description;
    public int rewardMoney;
    public int rewardReputation;

    [Header("Progression")]
    public bool isUnlocked = false;
    public bool isCompleted = false;
    public Sprite missionArt;

    public List<MissionStep> steps = new List<MissionStep>();
}

[System.Serializable]
public class MissionStep
{
    public enum StepType { DriveTo, Cutscene, Chase, Interaction, Objective }
    
    public string stepName;
    public StepType type;
    public Vector3 targetPosition;
    public string dialogueText;
    public AdvancedProceduralTTS.Emotion dialogueEmotion;
    public float timeLimit = -1f; // -1 for no limit
    
    [Header("Cinematic Settings")]
    public string cutsceneID;
    public float slowmoSpeed = 0.8f;
}
