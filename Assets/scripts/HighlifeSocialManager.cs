using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 10: Highlife Social Manager.
/// Manages global city "Moods" and coordinates NPC social densities.
/// </summary>
public class HighlifeSocialManager : MonoBehaviour
{
    public static HighlifeSocialManager Instance { get; private set; }

    public enum CityMood { Normal, Party, Tense, Riot }
    
    [Header("Global Mood")]
    public CityMood currentMood = CityMood.Normal;
    public float moodIntensity = 1.0f;

    [Header("Social Settings")]
    public float npcInteractionProbability = 0.3f;
    public bool allowMatureInteractions = true;

    private List<SocialZone> activeZones = new List<SocialZone>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterZone(SocialZone zone)
    {
        if (!activeZones.Contains(zone)) activeZones.Add(zone);
    }

    public void UnregisterZone(SocialZone zone)
    {
        if (activeZones.Contains(zone)) activeZones.Remove(zone);
    }

    /// <summary>
    /// Phase 12: Trigger a city-wide cinematic event (e.g. Riot).
    /// </summary>
    public void TriggerCinematicEvent(CityMood mood)
    {
        SetCityMood(mood);
        SocialNPC[] allNpcs = FindObjectsOfType<SocialNPC>();
        foreach (var npc in allNpcs)
        {
            if (mood == CityMood.Riot) npc.SetSocialState(SocialNPC.SocialState.Aggressive);
            if (mood == CityMood.Normal) npc.SetSocialState(SocialNPC.SocialState.Wandering);
        }
    }
}
