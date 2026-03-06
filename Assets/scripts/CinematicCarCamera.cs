using UnityEngine;

/// <summary>
/// Phase 15: Cinematic Car Perspectives.
/// Provides multiple camera angles: Hood, Dash, Interior, and 360-Orbit.
/// Addresses the "inside my driver seat looking out" request.
/// </summary>
public class CinematicCarCamera : MonoBehaviour
{
    public Transform[] cameraPoints; // 0: Hood, 1: Dash, 2: Interior, 3: Follow
    public Camera targetCamera;
    public float smoothSpeed = 10f;

    private int currentPointIndex = 3; // Default to follow

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCamera();
        }

        if (cameraPoints.Length > 0 && cameraPoints[currentPointIndex] != null)
        {
            // Smoothly move the camera to the destination
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, cameraPoints[currentPointIndex].position, Time.deltaTime * smoothSpeed);
            targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, cameraPoints[currentPointIndex].rotation, Time.deltaTime * smoothSpeed);
        }
    }

    public void ToggleCamera()
    {
        currentPointIndex = (currentPointIndex + 1) % cameraPoints.Length;
        Debug.Log("[Camera] Switched to: " + GetCameraLabel(currentPointIndex));
    }

    string GetCameraLabel(int index)
    {
        switch (index)
        {
            case 0: return "Hood View";
            case 1: return "Dash/Speedo View";
            case 2: return "Interior / Driver Seat";
            case 3: return "Third Person Follow";
            default: return "Unknown";
        }
    }
}
