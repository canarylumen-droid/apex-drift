using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase 12: Luxury HUD Manager.
/// Provides a "Wowed" UI experience with 3D icons, particle bursts for coins,
/// and smooth background art transitions for missions.
/// </summary>
public class LuxuryHUDManager : MonoBehaviour
{
    public static LuxuryHUDManager Instance { get; private set; }

    [Header("UI Overlays")]
    public GameObject coinBurstPrefab;
    public GameObject gemBurstPrefab;
    public Image missionArtFull;
    public Text unlockText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OnMissionUnlocked(string title, Sprite art)
    {
        if (unlockText != null)
        {
            unlockText.text = "MISSION UNLOCKED: " + title;
            unlockText.gameObject.SetActive(true);
            if (art != null) missionArtFull.sprite = art;
            
            // Trigger Fade
            StartCoroutine(FadeOutUnlock(3f));
        }

        // Phase 12 Sound
        if (SoundInteractionEngine.Instance != null)
            SoundInteractionEngine.Instance.PlayMenuSound("Mission_Unlock");
    }

    private System.Collections.IEnumerator FadeOutUnlock(float delay)
    {
        yield return new WaitForSeconds(delay);
        unlockText.gameObject.SetActive(false);
    }

    public void TriggerCoinBurst(Vector3 screenPos)
    {
        if (coinBurstPrefab != null)
        {
            GameObject burst = Instantiate(coinBurstPrefab, screenPos, Quaternion.identity);
            Destroy(burst, 1.5f);
        }
    }
}
