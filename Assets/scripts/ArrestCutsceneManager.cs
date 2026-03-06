using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 2: Arrest Cutscene Manager.
/// Triggers massive cinematic scenes when the player is caught by the police.
/// Handles the player getting out, kneeling, hands in the air, and being handcuffed.
/// </summary>
public class ArrestCutsceneManager : MonoBehaviour
{
    public static ArrestCutsceneManager Instance { get; private set; }

    private bool isArresting = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Monitor if player is completely stopped while under 3-5 star pursuit
        if (WantedSystem.Instance != null && WantedSystem.Instance.isUnderChase && WantedSystem.Instance.wantedStars >= 3)
        {
            RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
            if (player != null && player.speed < 2f && !isArresting)
            {
                // Check if cop is nearby
                Collider[] colliders = Physics.OverlapSphere(player.transform.position, 15f);
                foreach (var col in colliders)
                {
                    if (col.GetComponent<PoliceAI>() != null)
                    {
                        TriggerArrest(player);
                        break;
                    }
                }
            }
        }
    }

    private void TriggerArrest(RCC_CarControllerV3 player)
    {
        isArresting = true;
        Debug.Log("[ArrestManager] Initiating Wasted Cinematic Sequence!");
        
        // Use the CinematicDirector to handle cameras and HUD
        if (CinematicDirector.Instance != null)
        {
            StartCoroutine(ArrestSequence(player));
        }
    }

    private IEnumerator ArrestSequence(RCC_CarControllerV3 player)
    {
        // Lock controls and start cutscene
        player.canControl = false;
        WantedSystem.Instance.isUnderChase = false;
        
        // Move camera to a dramatic ground angle pointing at the car door
        Camera cineCam = GameObject.Find("Cinematic_Camera").GetComponent<Camera>();
        if (cineCam != null)
        {
            cineCam.enabled = true;
            Camera.main.enabled = false;
            
            Vector3 camTargetPos = player.transform.position + player.transform.right * -4f + Vector3.up * 1f;
            cineCam.transform.position = camTargetPos;
            cineCam.transform.LookAt(player.transform.position + Vector3.up * 1f);
        }

        // Enable cinematic UI
        if (CinematicDirector.Instance != null)
        {
            CinematicDirector.Instance.cinematicBorders.SetActive(true);
        }

        // 1. Radio chatter
        if (ProceduralTTSManager.Instance != null) ProceduralTTSManager.Instance.Speak("Suspect apprehended! Get on the ground now!");
        yield return new WaitForSeconds(3f);

        // 2. Spawn Player Character Model outside the car
        GameObject playerModelPrefab = null;
        if (RealisticModelManager.Instance != null) playerModelPrefab = RealisticModelManager.Instance.humanIdlePrefab;
        
        if (playerModelPrefab != null)
        {
            Vector3 spawnPos = player.transform.position + player.transform.right * -1.5f;
            GameObject playerDummy = Instantiate(playerModelPrefab, spawnPos, Quaternion.LookRotation(-player.transform.right));
            
            Animator anim = playerDummy.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                // Trigger Kneel and Hands in air animation
                anim.SetTrigger("KneelHandsUp"); // Requires appropriate Mixamo Animation imported
            }

            // Spawn SWAT officer approaching
            GameObject swat = Instantiate(playerModelPrefab, spawnPos + player.transform.right * -3f, Quaternion.LookRotation(player.transform.right));
            Animator swatAnim = swat.GetComponentInChildren<Animator>();
            if (swatAnim != null) swatAnim.SetTrigger("ArrestPose");
            
            // Adjust Camera to look at the kneeling player
            cineCam.transform.LookAt(playerDummy.transform.position + Vector3.up * 1.5f);
        }
        else
        {
            Debug.LogWarning("No player 3D FBX found for cutscene. Skipping full body render.");
        }

        yield return new WaitForSeconds(2f);
        
        if (ProceduralTTSManager.Instance != null) ProceduralTTSManager.Instance.Speak("You're going away for a long time, scumbag.");

        if (CinematicDirector.Instance != null)
        {
            CinematicDirector.Instance.subtitleText.gameObject.SetActive(true);
            CinematicDirector.Instance.subtitleText.color = Color.red;
            CinematicDirector.Instance.subtitleText.fontSize = 80;
            CinematicDirector.Instance.subtitleText.text = "BUSTED";
        }

        yield return new WaitForSeconds(5f);

        // Reset Level or send to Jail (Home garage for now)
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
