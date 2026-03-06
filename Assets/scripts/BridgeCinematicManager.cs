using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 18: Bridge Cinematic Manager.
/// Handles the "Epic Bridge Scene" with destruction and jumps.
/// Coordinates multiple camera angles and world-state changes.
/// </summary>
public class BridgeCinematicManager : MonoBehaviour
{
    public Transform bridgeRampPoint;
    public GameObject bridgeGapVisual;
    public Transform cinematicCamPosition;

    private bool sequenceTriggered = false;

    public void TriggerBridgeJump()
    {
        if (sequenceTriggered) return;
        StartCoroutine(ExecuteBridgeSequence());
    }

    private IEnumerator ExecuteBridgeSequence()
    {
        sequenceTriggered = true;
        Debug.Log("[Bridge] CINEMATIC SEQUENCE START!");

        // 1. Cinematic Camera Cut
        if (CinematicDirector.Instance != null)
        {
            CinematicDirector.Instance.TriggerEventCamera(cinematicCamPosition.position, 8f, 0.4f); // Slow-mo jump
        }

        // 2. Play Bridge Collapse Sound
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("Bridge_Collapse", 1.2f);

        // 3. Slow motion for the jump
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(5f);

        // 4. Return to normal
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        
        Debug.Log("[Bridge] Sequence Complete. Player escaped!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerBridgeJump();
        }
    }
}
