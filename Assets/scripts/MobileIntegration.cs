using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 7: Mobile Integration.
/// Simulates native push notifications and "Fake Calls" from mission NPCs.
/// </summary>
public class MobileIntegration : MonoBehaviour
{
    public static MobileIntegration Instance { get; private set; }

    [Header("Notification Settings")]
    public GameObject pushNotificationPanel; // UI Panel
    public UnityEngine.UI.Text notificationTitle;
    public UnityEngine.UI.Text notificationBody;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// Simulates a push notification that appears on the player's screen.
    /// </summary>
    public void SendPushNotification(string title, string body)
    {
        Debug.Log($"[Mobile] Push Notification Received: {title} - {body}");
        
        if (pushNotificationPanel != null)
        {
            notificationTitle.text = title;
            notificationBody.text = body;
            pushNotificationPanel.SetActive(true);
            
            // Play a standard mobile notification sound
            if (PremiumHUD.Instance != null) PremiumHUD.Instance.PlayClickSound();
            
            StartCoroutine(AutoDismissNotification(5f));
        }
    }

    /// <summary>
    /// Simulates a phone call from a mission contact.
    /// </summary>
    public void TriggerFakeCall(string contactName, string dialogue)
    {
        Debug.Log($"[Mobile] Incoming Call from {contactName}");
        
        // Use TTS for the voice calling
        if (AdvancedProceduralTTS.Instance != null)
        {
            AdvancedProceduralTTS.Instance.Speak($"Incoming call from {contactName}. {dialogue}", AdvancedProceduralTTS.Emotion.Happy);
        }

        SendPushNotification("INCOMING CALL", contactName);
    }

    private IEnumerator AutoDismissNotification(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (pushNotificationPanel != null) pushNotificationPanel.SetActive(false);
    }
}
