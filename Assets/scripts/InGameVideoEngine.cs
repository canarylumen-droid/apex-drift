using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Phase 10: In-Game Cinema/Video Engine.
/// Plays mp4 files on designated screens (Theater, Club TV).
/// </summary>
public class InGameVideoEngine : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string[] moviePlaylist; // Paths to mp4 files in StreamingAssets
    private int currentTrack = 0;

    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        
        // Auto-play first clip if exists
        if (moviePlaylist.Length > 0) PlayMovie(0);
    }

    public void PlayMovie(int index)
    {
        if (index < 0 || index >= moviePlaylist.Length) return;
        
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, moviePlaylist[index]);
        videoPlayer.url = path;
        videoPlayer.Play();
        
        Debug.Log($"[VideoEngine] Now playing billboard movie: {moviePlaylist[index]}");
    }

    public void NextMovie()
    {
        currentTrack = (currentTrack + 1) % moviePlaylist.Length;
        PlayMovie(currentTrack);
    }
}
