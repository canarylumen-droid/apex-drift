using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase 7: Game Settings App.
/// UI menu to control Volume, Audio preferences, and Voice Toggles.
/// </summary>
public class GameSettingsApp : MonoBehaviour
{
    public static GameSettingsApp Instance { get; private set; }

    [Header("UI Controls")]
    public Slider masterVolumeSlider;
    public Toggle voiceToggle;
    public Toggle sfxToggle;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        Debug.Log($"[Settings] Master Volume: {value}");
    }

    public void OnVoiceToggle(bool enabled)
    {
        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.gameObject.SetActive(enabled);
        }
        Debug.Log($"[Settings] Voice Enabled: {enabled}");
    }

    public void OnSFXToggle(bool enabled)
    {
        // Logic to mute UI/Engine sounds
        if (CinematicAudioManager.Instance != null)
        {
            CinematicAudioManager.Instance.ambientVolume = enabled ? 0.5f : 0f;
        }
        Debug.Log($"[Settings] SFX Enabled: {enabled}");
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
        if (ApexSaveSystem.Instance != null) ApexSaveSystem.Instance.SaveGame();
    }
}
