using UnityEngine;

/// <summary>
/// Phase 20: Character Fidget AI.
/// Adds micro-movements to NPCs: finger twitching, phone usage, and limb shifting.
/// </summary>
public class CharacterFidgetAI : MonoBehaviour
{
    private Animator animator;
    private float fidgetTimer = 0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        fidgetTimer += Time.deltaTime;
        
        if (fidgetTimer > 5f)
        {
            TriggerRandomFidget();
            fidgetTimer = 0f;
        }
    }

    private void TriggerRandomFidget()
    {
        if (animator == null) return;

        float r = Random.value;
        if (r > 0.8f) animator.SetTrigger("CheckPhone");
        else if (r > 0.6f) animator.SetTrigger("HandWiggle");
        else if (r > 0.4f) animator.SetTrigger("ShiftWeight");
        
        // Phase 20: Procedural finger movement logic would go here
        // (Usually handled via Bone rotations in a late update)
    }
}
