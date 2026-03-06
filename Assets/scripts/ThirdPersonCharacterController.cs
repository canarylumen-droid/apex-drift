using UnityEngine;

/// <summary>
/// Phase 8: Third-Person Character Mobility.
/// Handles protagonist on-foot movement (walk, run, jump).
/// Seamlessly integrates with the High-Poly Protagonist FBX.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PerspectiveManager))]
[RequireComponent(typeof(CharacterSkinBinder))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    public static ThirdPersonCharacterController Instance { get; private set; }

    [Header("Realism Hook-ins")]
    public FacialAnimationController faceController;
    private PerspectiveManager perspective;
    private CharacterSkinBinder skinBinder;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        controller = GetComponent<CharacterController>();
        perspective = GetComponent<PerspectiveManager>();
        skinBinder = GetComponent<CharacterSkinBinder>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 1. Ground Check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Input Logic
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate rotation towards camera heading
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Phase 9: Trigger Footsteps based on movement
            if (FootstepSoundManager.Instance != null && isGrounded)
            {
                // Simple timer-based footstep or distance-based
                if (Time.frameCount % 20 == 0) // Roughly every few steps
                    FootstepSoundManager.Instance.PlayFootstep(transform);
            }

            // Update Animator
            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed / runSpeed);
                animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            if (animator != null) animator.SetBool("IsMoving", false);
        }

        // 3. Jump Logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger("Jump");
        }

        // 4. Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetControlled(bool canMove)
    {
        this.enabled = canMove;
        if (!canMove && animator != null) animator.SetBool("IsMoving", false);
    }
}
