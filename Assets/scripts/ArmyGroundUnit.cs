using UnityEngine;

/// <summary>
/// Phase 17: Army Ground Unit.
/// Specialized aggressive AI for SWAT/Army characters rappelling from helicopters.
/// </summary>
public class ArmyGroundUnit : MonoBehaviour
{
    private SocialNPC social;
    private Transform player;

    void Start()
    {
        social = GetComponent<SocialNPC>();
        if (social == null) social = gameObject.AddComponent<SocialNPC>();
        
        social.SetSocialState(SocialNPC.SocialState.Aggressive);
        
        RCC_CarControllerV3 p = FindObjectOfType<RCC_CarControllerV3>();
        if (p != null) player = p.transform;

        // Play heavy weapon cocking sound
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("Weapon_Cocking", 1f);
    }

    void Update()
    {
        if (player == null) return;

        // Pursue player more aggressively than standard police
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > 5f)
        {
            // Run towards player
            if (social.animator != null) social.animator.SetFloat("Speed", 1.0f);
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 6f);
            transform.LookAt(player.position);
        }
        else
        {
            // Close combat / Point gun
            if (social.animator != null) social.animator.SetTrigger("PointTactical");
        }
    }
}
