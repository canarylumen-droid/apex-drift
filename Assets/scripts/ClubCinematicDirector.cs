using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 4: Club Cinematic Director.
/// Specialized director for mature club scenes with dynamic camera angles and scripted NPC interactions.
/// </summary>
public class ClubCinematicDirector : MonoBehaviour
{
    public static ClubCinematicDirector Instance { get; private set; }

    [Header("Club Settings")]
    public Transform clubEntrance;
    public Transform danceFloorPivot;
    public GameObject stripperPoles;

    private bool isInsideClub = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void StartClubSequence()
    {
        if (isInsideClub) return;
        StartCoroutine(PlayClubSequence());
    }

    private IEnumerator PlayClubSequence()
    {
        isInsideClub = true;
        Debug.Log("[ClubDirector] Entering high-heat cinematic sequence.");

        if (CinematicDirector.Instance != null)
        {
            // Lock controls and show borders
            RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
            if (player != null) player.canControl = false;

            CinematicDirector.Instance.cinematicBorders.SetActive(true);

            // 1. Entrance Shot (Low angle pan)
            CinematicDirector.Instance.TriggerEventCamera(clubEntrance.position, 3f, 0.8f);
            if (AdvancedProceduralTTS.Instance != null)
                AdvancedProceduralTTS.Instance.Speak("Welcome to the lounge. Relax and enjoy the view.", AdvancedProceduralTTS.Emotion.Romantic);
            
            yield return new WaitForSeconds(3f);

            // 2. Dance Floor Shot (Dynamic twerking/dancing focus)
            CinematicDirector.Instance.TriggerEventCamera(danceFloorPivot.position, 5f, 0.8f);
            // In a real build, we'd trigger twerking animations on specific models here
            
            yield return new WaitForSeconds(5f);

            // 3. Relax/Smoking Segment
            if (AdvancedProceduralTTS.Instance != null)
                AdvancedProceduralTTS.Instance.Speak("Need a drink? Or something... better?", AdvancedProceduralTTS.Emotion.Romantic);
            
            yield return new WaitForSeconds(4f);

            // Return control
            CinematicDirector.Instance.cinematicBorders.SetActive(false);
            if (player != null) player.canControl = true;
            isInsideClub = false;
        }
    }
}
