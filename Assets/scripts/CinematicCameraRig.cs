using UnityEngine;

/// <summary>
/// Professional cinematic camera rig that replaces the basic camera.cs.
/// Adjusts angle and distance based on speed, environment, and state.
/// Shows the car face and adjusts for weather/drunk effects.
/// </summary>
public class CinematicCameraRig : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // The car to follow

    [Header("Camera Positions")]
    public float defaultDistance = 6f;
    public float defaultHeight = 2.5f;
    public float highSpeedDistance = 8f;
    public float highSpeedHeight = 3.5f;
    public float speedThreshold = 100f;

    [Header("Smoothing")]
    public float followSpeed = 5f;
    public float rotationSpeed = 3f;
    public float lookAheadMultiplier = 0.5f;

    [Header("Drunk Camera")]
    public float drunkSwayAmount = 0.3f;
    public float drunkSwaySpeed = 1.5f;

    private RCC_CarControllerV3 carController;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null) FindTarget();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        float speed = 0;
        if (carController != null) speed = Mathf.Abs(carController.speed);

        // Determine distance/height based on speed
        float t = Mathf.Clamp01(speed / speedThreshold);
        float dist = Mathf.Lerp(defaultDistance, highSpeedDistance, t);
        float height = Mathf.Lerp(defaultHeight, highSpeedHeight, t);

        // Calculate desired position behind the car
        Vector3 desiredPos = target.position
            - target.forward * dist
            + Vector3.up * height;

        // Add drunk sway if active
        if (CinematicInteractionManager.Instance != null && CinematicInteractionManager.Instance.isDrunk)
        {
            float swayX = Mathf.Sin(Time.time * drunkSwaySpeed) * drunkSwayAmount;
            float swayY = Mathf.Cos(Time.time * drunkSwaySpeed * 0.7f) * drunkSwayAmount * 0.5f;
            desiredPos += new Vector3(swayX, swayY, 0);
        }

        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, 1f / followSpeed);

        // Look at the car with a slight look-ahead
        Vector3 lookTarget = target.position + target.forward * speed * lookAheadMultiplier * 0.01f + Vector3.up * 1f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    private void FindTarget()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
            {
                target = car.transform;
                carController = car;
                break;
            }
        }
    }
}
