using UnityEngine;

/// <summary>
/// Phase 12: Sound Interaction Engine.
/// Dedicated manager for UI / HUD sound effects to ensure the menu feels alive.
/// </summary>
public class SoundInteractionEngine : MonoBehaviour
{
    public static SoundInteractionEngine Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip swipeSound;
    public AudioClip unlockSound;
    public AudioClip coinSound;

    private AudioSource source;

    void Awake()
    {
        if (Instance == null) Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.ignoreListenerPause = true; // Menu sounds should play even if paused
    }

    public void PlayMenuSound(string type)
    {
        AudioClip clip = null;
        float vol = 0.5f;

        switch (type.ToLower())
        {
            case "click": clip = clickSound; vol = 0.5f; break;
            case "swipe": clip = swipeSound; vol = 0.3f; break;
            case "unlock": clip = unlockSound; vol = 0.8f; break;
            case "coin": clip = coinSound; vol = 0.6f; break;
        }

        if (clip != null)
        {
            source.PlayOneShot(clip, vol);
        }
    }
}
