using UnityEngine;
using System.Collections;

/// <summary>
/// Monitors high-impact collisions and triggers police sirens and "Wasted" states.
/// </summary>
public class PoliceChaseManager : MonoBehaviour
{
    public static PoliceChaseManager Instance { get; private set; }

    [Header("Detection Settings")]
    public float crashThreshold = 15f; // Magnitude of impact to trigger "chaos"
    public float sirenRadius = 50f;

    [Header("Audio")]
    public AudioSource sirenSource;

    private bool isChasing = false;
    private RCC_CarControllerV3 playerCar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (playerCar == null)
        {
            FindPlayerCar();
            return;
        }

        CheckForImpacts();
    }

    private void FindPlayerCar()
    {
        RCC_CarControllerV3[] cars = FindObjectsOfType<RCC_CarControllerV3>();
        foreach (var car in cars)
        {
            if (car.canControl && car.GetComponent<RCC_AICarController>() == null)
            {
                playerCar = car;
                break;
            }
        }
    }

    private void CheckForImpacts()
    {
        // RCC_CarControllerV3 doesn't expose a simple Collision callback here,
        // so we monitor rapid deceleration or hooks if we had access to the Rigidbody.
        // For now, we'll provide a public method for the Car to call.
    }

    public void ReportCollision(float impactForce)
    {
        if (impactForce > crashThreshold && !isChasing)
        {
            StartPoliceChase();
        }
    }

    private void StartPoliceChase()
    {
        isChasing = true;
        Debug.Log("[Police] High impact detected! Triggering chase...");

        if (CinematicAudioManager.Instance != null)
        {
            // Trigger the police siren sound
            CinematicAudioManager.Instance.PlayCinematicSound("PoliceSiren", 1.0f);
        }

        // Trigger visual "Police" HUD or Sirens
        StartCoroutine(ChaseCooldown());
    }

    private IEnumerator ChaseCooldown()
    {
        yield return new WaitForSeconds(15f);
        isChasing = false;
        Debug.Log("[Police] Chase ended.");
    }
}
