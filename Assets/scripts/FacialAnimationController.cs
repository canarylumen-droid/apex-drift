using UnityEngine;

/// <summary>
/// Phase 10: Facial & Lip-Sync Logic.
/// Hooks into BlendShapes of the high-poly faces to move mouths during TTS.
/// Also handles additive finger/arm layering for realistic interaction.
/// </summary>
public class FacialAnimationController : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh;
    public int mouthOpenBlendShapeIndex = 0;
    
    [Header("Lip Sync")]
    public bool isSpeaking = false;
    public float mouthIntensity = 50f;

    void Update()
    {
        if (isSpeaking)
        {
            // Simple audio-amplitude-based mouth jitter or procedural oscillation
            float weight = Mathf.Abs(Mathf.Sin(Time.time * 15f)) * mouthIntensity;
            if (faceMesh != null)
                faceMesh.SetBlendShapeWeight(mouthOpenBlendShapeIndex, weight);
        }
        else
        {
            if (faceMesh != null)
                faceMesh.SetBlendShapeWeight(mouthOpenBlendShapeIndex, 0f);
        }
    }

    public void StartSpeaking(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SpeakingRoutine(duration));
    }

    private System.Collections.IEnumerator SpeakingRoutine(float duration)
    {
        isSpeaking = true;
        yield return new WaitForSeconds(duration);
        isSpeaking = false;
    }

    // Handles finger movement layering
    public void TriggerFineMotorAction(string actionName)
    {
        // This will hook into a specialized Animator layer for fingers/hands
        Debug.Log($"[Face/Hand AI] Performing: {actionName}");
        GetComponent<Animator>()?.SetTrigger(actionName);
    }
}
