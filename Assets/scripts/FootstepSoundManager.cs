using UnityEngine;

/// <summary>
/// Phase 9: High-Fidelity Audio Scape.
/// Plays realistic field-of-world sounds based on the environment.
/// </summary>
public class FootstepSoundManager : MonoBehaviour
{
    public static FootstepSoundManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip concreteStep;
    public AudioClip grassStep;
    public AudioClip metalStep;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // Full 3D
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
    }

    public void PlayFootstep(Transform origin)
    {
        // 1. Raycast down to find floor tag
        RaycastHit hit;
        if (Physics.Raycast(origin.position + Vector3.up, Vector3.down, out hit, 2f))
        {
            AudioClip clip = concreteStep; // Default

            if (hit.collider.CompareTag("Grass")) clip = grassStep;
            else if (hit.collider.CompareTag("Metal")) clip = metalStep;

            if (clip != null)
            {
                audioSource.transform.position = hit.point;
                audioSource.PlayOneShot(clip, 1.0f);
            }
        }
    }
}
