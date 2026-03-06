using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Phase 13: Billboard Video Manager.
/// Manages large-scale video output for city billboards and the theater.
/// Synchronizes movie playback across multiple 3D screens.
/// </summary>
public class BillboardVideoManager : MonoBehaviour
{
    public static BillboardVideoManager Instance { get; private set; }

    [Header("City Screens")]
    public VideoPlayer[] cityBillboards;
    public VideoPlayer theaterScreen;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayCityCommercials(string videoUrl)
    {
        foreach (var billboard in cityBillboards)
        {
            if (billboard != null)
            {
                billboard.url = videoUrl;
                billboard.Play();
            }
        }
        Debug.Log("[Billboard] Playing city-wide broadcast: " + videoUrl);
    }

    public void PlayTheaterMovie(string videoUrl)
    {
        if (theaterScreen != null)
        {
            theaterScreen.url = videoUrl;
            theaterScreen.Play();
            
            // Phase 13: Mute ambient city sound near theater
            if (AmbientAudioDirector.Instance != null)
                AmbientAudioDirector.Instance.SetAmbientVolume(0.2f);
        }
    }
}
