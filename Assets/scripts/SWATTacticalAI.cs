using UnityEngine;

/// <summary>
/// Phase 15: SWAT Tactical AI.
/// Handles high-poly SWAT specific animations: touching ear for backup, 
/// tactical pointing, and army-style movement.
/// </summary>
public class SWATTacticalAI : MonoBehaviour
{
    public Animator animator;
    public float backupCallDistance = 30f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void CallForBackup()
    {
        Debug.Log("[SWAT] Touching ear: Calling for backup.");
        if (animator != null) animator.SetTrigger("TouchEarBackup");
        
        // Play radio static sound
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("Police_Radio_Static", 0.7f);
    }

    public void PointAtPlayer()
    {
        if (animator != null) animator.SetTrigger("PointTactical");
    }

    void Update()
    {
        // If player is far and we are pursuing, touch ear to alert others
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null && Vector3.Distance(transform.position, player.transform.position) > backupCallDistance)
        {
            if (Random.value > 0.999f) CallForBackup();
        }
    }
}
