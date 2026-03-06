using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 4: Mature Sandbox II - Sexy Interaction AI.
/// Handles contextual female NPC reactions to the player (Biting lips, blowing kisses).
/// Reactions are more intense if the player is in a Supercar.
/// </summary>
public class SexyInteractionAI : MonoBehaviour
{
    [Header("NPC Settings")]
    public float detectionRange = 10f;
    public float interactionCooldown = 15f;
    
    [Header("Animations")]
    public string lipBiteTrigger = "LipBite";
    public string blowKissTrigger = "BlowKiss";
    public string waveTrigger = "SexyWave";

    private Animator animator;
    private float lastInteractionTime;
    private SocialNPC socialNPC;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        socialNPC = GetComponent<SocialNPC>();
        lastInteractionTime = -interactionCooldown;
    }

    void Update()
    {
        if (Time.time - lastInteractionTime < interactionCooldown) return;

        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange)
        {
            // Check if player is in a 'Supercar' (Reputation System can tell us this or speed/model check)
            bool isCoolCar = player.speed > 30f || player.GetComponent<DynamicEngineAudio>()?.currentProfile == DynamicEngineAudio.CarProfile.Supercar;

            if (isCoolCar)
            {
                LookAtPlayer(player.transform);
                TriggerSexyReaction();
            }
        }
    }

    private void LookAtPlayer(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void TriggerSexyReaction()
    {
        lastInteractionTime = Time.time;
        
        float r = Random.value;
        if (r < 0.33f)
        {
            animator.SetTrigger(lipBiteTrigger);
            if (AdvancedProceduralTTS.Instance != null)
                AdvancedProceduralTTS.Instance.Speak("Nice ride...", AdvancedProceduralTTS.Emotion.Romantic, 1.3f);
        }
        else if (r < 0.66f)
        {
            animator.SetTrigger(blowKissTrigger);
            if (AdvancedProceduralTTS.Instance != null)
                AdvancedProceduralTTS.Instance.Speak("Hey handsome!", AdvancedProceduralTTS.Emotion.Happy, 1.4f);
        }
        else
        {
            animator.SetTrigger(waveTrigger);
        }

        Debug.Log("[SexyAI] NPC reacting to player's presence.");
    }
}
