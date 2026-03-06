using UnityEngine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Phase 3: Cinema Theatre Manager.
/// Handles movie playback in the 3D world and NPC seating.
/// </summary>
public class CinemaTheatreManager : MonoBehaviour
{
    public static CinemaTheatreManager Instance { get; private set; }

    [Header("Theatre Components")]
    public VideoPlayer mainScreen;
    public GameObject theatreRoom; // Should have seats
    public MeshRenderer screenRenderer;

    private bool isPlayerInTheatre = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        SetupTheatreRuntime();
    }

    private void SetupTheatreRuntime()
    {
        // Dynamically Create a VideoPlayer if null
        if (mainScreen == null)
        {
            mainScreen = gameObject.AddComponent<VideoPlayer>();
            mainScreen.playOnAwake = false;
            mainScreen.isLooping = true;
            mainScreen.renderMode = VideoRenderMode.MaterialOverride;
            
            // Try load a movie from Resources/Movies/
            VideoClip clip = Resources.Load<VideoClip>("Movies/CinemaLoop_1");
            if (clip != null) mainScreen.clip = clip;
        }
    }

    public void EnterTheatre()
    {
        isPlayerInTheatre = true;
        Debug.Log("[Cinema] Player entered theatre. Dimming lights.");
        
        if (mainScreen != null) mainScreen.Play();
        
        // Spawn 5-10 NPCs in seats
        SpawnAudience();
    }

    private void SpawnAudience()
    {
        for (int i = 0; i < 8; i++)
        {
            // Placeholder: Use RealisticModelManager to spawn human models in a grid
            GameObject npc = new GameObject("CinemaNPC_" + i);
            npc.transform.SetParent(transform);
            // logic to place on seats...
        }
    }

    public void ExitTheatre()
    {
        isPlayerInTheatre = false;
        if (mainScreen != null) mainScreen.Pause();
    }
}
