using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Phase 8: NPC Roaming AI.
/// NPCs now pick random targets in the city and walk naturally.
/// </summary>
public class NPCRoamingAI : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public float waitPeriod = 3f;
    public float roamRadius = 50f;

    private bool isWaiting = false;
    private Transform player;

    private Vector3 startPosition;
    private Vector3 currentTarget;
    private bool isWandering = false;
    private Animator animator;

    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(RoamingRoutine());
    }

    public void SetWaitState(bool wait)
    {
        isWaiting = wait;
        if (isWaiting)
        {
            if (player != null)
            {
                Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.LookAt(lookTarget);
            }
            if (animator != null) animator.SetTrigger("TalkGesture");
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            if (isWaiting)
            {
                yield return null;
                continue;
            }

            // 1. Pick a random point
            Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
            currentTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            isWandering = true;
            if (animator != null) animator.SetBool("IsWalking", true);

            // 2. Walk towards target
            while (Vector3.Distance(transform.position, currentTarget) > 1.5f)
            {
                if (isWaiting) break; // Stop walking if interrupted

                Vector3 direction = (currentTarget - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
                
                if (Time.frameCount % 40 == 0 && FootstepSoundManager.Instance != null)
                    FootstepSoundManager.Instance.PlayFootstep(transform);

                yield return null;
            }

            // 3. Reach target & Wait
            isWandering = false;
            if (animator != null) animator.SetBool("IsWalking", false);
            
            float waitTime = Random.Range(waitPeriod, waitPeriod * 2f);
            float elapsed = 0;
            while (elapsed < waitTime)
            {
                if (isWaiting) break; 
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
