using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 5: High-Stakes Interactions & Drugs.
/// Handles substance use, visual distortions, and police reporting.
/// </summary>
public class DrugSystemManager : MonoBehaviour
{
    public static DrugSystemManager Instance { get; private set; }

    [Header("Visual Effects")]
    public float distortionDuration = 20f;
    private bool isUnderInfluence = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UseHeroin()
    {
        if (isUnderInfluence) return;
        StartCoroutine(DrugEffectRoutine("Heroin", 1.5f, 0.5f));
    }

    private IEnumerator DrugEffectRoutine(string drugName, float intensity, float speedScale)
    {
        isUnderInfluence = true;
        Debug.Log($"[DrugSystem] Player used {drugName}. Shaders and Physics altering.");

        // 1. Trigger Visual Distortion (Drunk/Trippy)
        if (DrunkVisualEffects.Instance != null)
        {
            DrunkVisualEffects.Instance.ApplyDrunkEffect(distortionDuration);
        }

        // 2. TTS Reaction
        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.Speak("Everything is... moving. Oh man.", AdvancedProceduralTTS.Emotion.Neutral, 0.8f);
        }

        // 3. Citizen Report (Police Trigger)
        StartCoroutine(CitizenReportDelayed());

        yield return new WaitForSeconds(distortionDuration);
        isUnderInfluence = false;
    }

    private IEnumerator CitizenReportDelayed()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("[DrugSystem] Citizen spotted drug use. Reporting to police.");

        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.Speak("Is that guy doing drugs? I'm calling 911 right now!", AdvancedProceduralTTS.Emotion.Angry);
        }

        if (WantedSystem.Instance != null)
        {
            WantedSystem.Instance.AddWantedLevel(2); // Sudden 2-star jump
        }
    }
}
