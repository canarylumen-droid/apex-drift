using UnityEngine;

/// <summary>
/// Phase 14: Skeletal Lip-Sync.
/// Refines mouth movement based on phonetic data from the TTS.
/// Maps A, E, O, M, P phonemes to bone rigs.
/// </summary>
public class SkeletalLipSync : MonoBehaviour
{
    public Transform jawBone;
    public Transform lipUpper;
    public Transform lipLower;

    private float mouthOpenLevel = 0f;
    private float targetMouthOpen = 0f;

    public void OnPhonemeChanged(string phonemeType)
    {
        // Advanced mapping for Million Dollar Brain
        switch (phonemeType.ToUpper())
        {
            case "A": targetMouthOpen = 0.8f; break;
            case "E": targetMouthOpen = 0.5f; break;
            case "I": targetMouthOpen = 0.3f; break;
            case "O": targetMouthOpen = 1.0f; break;
            case "U": targetMouthOpen = 0.7f; break;
            case "M": case "P": case "B": targetMouthOpen = 0.1f; break;
            default: targetMouthOpen = 0.1f; break;
        }
    }

    void Update()
    {
        // Smooth transition
        mouthOpenLevel = Mathf.Lerp(mouthOpenLevel, targetMouthOpen, Time.deltaTime * 20f);
        
        if (jawBone != null)
        {
            jawBone.localRotation = Quaternion.Euler(mouthOpenLevel * 15f, 0, 0);
        }
    }
}
