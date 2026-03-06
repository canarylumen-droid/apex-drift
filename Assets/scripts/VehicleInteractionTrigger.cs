using UnityEngine;

/// <summary>
/// Phase 8: Vehicle Interaction Trigger.
/// Manages the seamless swap between on-foot and driving.
/// </summary>
public class VehicleInteractionTrigger : MonoBehaviour
{
    public GameObject characterPrefab; // Loaded from Resources
    private RCC_CarControllerV3 car;
    private bool isPlayerInside = true;

    void Awake()
    {
        car = GetComponent<RCC_CarControllerV3>();
    }

    void Update()
    {
        // 1. Exit Logic
        if (isPlayerInside && Input.GetKeyDown(KeyCode.F))
        {
            ExitVehicle();
        }
    }

    public void ExitVehicle()
    {
        isPlayerInside = false;
        car.canControl = false;
        
        // Disable car camera, enable player character
        if (RCC_SceneManager.Instance != null && RCC_SceneManager.Instance.activePlayerCamera != null)
            RCC_SceneManager.Instance.activePlayerCamera.gameObject.SetActive(false);

        // Spawn Character at door position
        Vector3 spawnPos = transform.position + (transform.right * -2f) + Vector3.up; 
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Models/Humans/Protagonist_HighPoly"), spawnPos, transform.rotation);
        playerObj.AddComponent<ThirdPersonCharacterController>();
        
        // Phase 11: Ensure high-poly binding
        if (AssetAutoBinder.Instance != null)
        {
            AssetAutoBinder.Instance.BindHighPolyModel(playerObj, "Protagonist_HighPoly");
        }

        // Hide car's internal driver model if any
        Debug.Log("[VehicleInteraction] Player exited vehicle.");

        // Register with Mobile UI for "Enter" action if near
        if (MobileInteractionUI.Instance != null)
        {
            MobileInteractionUI.Instance.ShowInteraction("ENTER CAR", () => EnterVehicle(playerObj));
        }
    }

    public void EnterVehicle(GameObject character)
    {
        isPlayerInside = true;
        car.canControl = true;
        
        // Destroy character, re-enable car camera
        Destroy(character);
        
        if (RCC_SceneManager.Instance != null && RCC_SceneManager.Instance.activePlayerCamera != null)
            RCC_SceneManager.Instance.activePlayerCamera.gameObject.SetActive(true);

        Debug.Log("[VehicleInteraction] Player entered vehicle.");
        if (MobileInteractionUI.Instance != null) MobileInteractionUI.Instance.HideInteraction();
    }
}
