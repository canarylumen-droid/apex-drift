using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Phase 17: Army Helicopter AI.
/// Handles aerial pursuit, searchlight tracking, and rappelling army units.
/// Addresses the "Army Coming Down" request.
/// </summary>
public class ArmyHelicopterAI : MonoBehaviour
{
    public Transform target; // Usually the player's car
    public Light searchlight;
    public GameObject swattPrefab;
    
    [Header("Flight Settings")]
    public float hoverHeight = 30f;
    public float followSpeed = 15f;
    public float rotationSpeed = 2f;

    private bool isRappelling = false;

    void Update()
    {
        if (target == null) 
        {
            RCC_CarControllerV3 p = FindObjectOfType<RCC_CarControllerV3>();
            if (p != null) target = p.transform;
            return;
        }

        // 1. Flight Logic
        Vector3 targetPos = target.position + Vector3.up * hoverHeight;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * rotationSpeed);

        // 2. Searchlight Logic
        if (searchlight != null)
        {
            searchlight.transform.LookAt(target.position);
            // Flicker if player is going too fast
        }

        // 3. Rappel Trigger (Chance based when close)
        if (!isRappelling && Vector3.Distance(transform.position, target.position) < 40f && Random.value > 0.999f)
        {
            StartCoroutine(RappelSequence());
        }
    }

    private System.Collections.IEnumerator RappelSequence()
    {
        isRappelling = true;
        Debug.Log("[Helicopter] Army rappelling down!");
        
        for (int i = 0; i < 3; i++)
        {
            GameObject unit = Instantiate(swattPrefab, transform.position + Vector3.down * 5f, Quaternion.identity);
            unit.AddComponent<ArmyGroundUnit>(); // Assign army behavior
            
            // In a real build, we'd animate the move down a rope here
            yield return new WaitForSeconds(1.5f);
        }
        
        yield return new WaitForSeconds(10f);
        isRappelling = false;
    }

    public void SetCinematicCamera()
    {
        // Switch to a "Chopper Cam" showing the helicopter from the ground or vice-versa
        CinematicDirector.Instance.TriggerEventCamera(transform.position, 10f, 1f);
    }
}
