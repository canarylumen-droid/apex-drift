using UnityEngine;

/// <summary>
/// Phase 10: Character Skin & IK Binder.
/// This connects the "mobility logic" to the "physical bones" of high-poly FBX models.
/// Handles ground-detection for feet (IK) so characters step realistically on curbs.
/// </summary>
public class CharacterSkinBinder : MonoBehaviour
{
    public Animator animator;
    
    [Header("IK Settings")]
    public float footOffset = 0.1f;
    public LayerMask groundLayer;

    private float leftFootWeight;
    private float rightFootWeight;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    // Called by Unity's Animation system for IK pass
    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // 1. Set IK Weights (Control intensity)
        leftFootWeight = animator.GetFloat("LeftFootWeight"); // Driven by animation clip
        rightFootWeight = animator.GetFloat("RightFootWeight");

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);

        // 2. Raycast to Floor for Left Foot
        RaycastHit hit;
        Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, 1.5f, groundLayer))
        {
            Vector3 footPos = hit.point;
            footPos.y += footOffset;
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }

        // 3. Raycast to Floor for Right Foot
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, 1.5f, groundLayer))
        {
            Vector3 footPos = hit.point;
            footPos.y += footOffset;
            animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }
    }

    public void BindSkinToBones(GameObject fbxBody)
    {
        Debug.Log("[SkinBinder] Mapping FBX bones to character rig.");
        // This would re-parent or link the SkinnedMeshRenderer if swapped at runtime
    }
}
