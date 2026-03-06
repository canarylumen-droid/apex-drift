using UnityEngine;

/// <summary>
/// Phase 10: Perspective Toggle.
/// Allows the player to switch between Third-Person and First-Person when on foot.
/// </summary>
public class PerspectiveManager : MonoBehaviour
{
    public Transform firstPersonTarget; // Usually head/eye level
    public Transform thirdPersonTarget; // Behind character
    public Camera playerCamera;

    private bool isFirstPerson = false;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TogglePerspective();
        }

        // Smoothly follow the active target
        Transform target = isFirstPerson ? firstPersonTarget : thirdPersonTarget;
        if (target != null)
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, target.position, Time.deltaTime * 10f);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, target.rotation, Time.deltaTime * 10f);
        }
    }

    public void TogglePerspective()
    {
        isFirstPerson = !isFirstPerson;
        Debug.Log("[Perspective] Switched to: " + (isFirstPerson ? "First Person" : "Third Person"));
    }
}
