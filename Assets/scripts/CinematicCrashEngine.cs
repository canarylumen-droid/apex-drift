using UnityEngine;
using System.Collections;

/// <summary>
/// Phase 9: "Rockstar" Cinematic Crash Engine.
/// Handles slow-motion impacts, camera shakes, and debris spawning for high-speed collisions.
/// </summary>
public class CinematicCrashEngine : MonoBehaviour
{
    public static CinematicCrashEngine Instance { get; private set; }

    [Header("Crash Settings")]
    public float minImpactForce = 15f;
    public float slowMoScale = 0.2f;
    public float slowMoDuration = 2.5f;
    public float cameraShakeIntensity = 0.5f;

    [Header("Debris & Effects")]
    public GameObject crashDebrisPrefab; // Populated via Resources
    public AudioClip[] crashSounds;

    private bool isSlowingDown = false;
    private float originalTimeScale = 1f;
    private GameObject playerCar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        LoadAssets();
    }

    void Update()
    {
        if (playerCar == null) FindPlayerCar();
    }

    private void FindPlayerCar()
    {
        RCC_CarControllerV3 car = FindObjectOfType<RCC_CarControllerV3>();
        if (car != null && car.canControl && car.GetComponent<RCC_AICarController>() == null)
        {
            playerCar = car.gameObject;
            // Add a dynamic collision listener if not present
            if (playerCar.GetComponent<CrashImpactListener>() == null)
            {
                playerCar.AddComponent<CrashImpactListener>();
            }
        }
    }

    private void LoadAssets()
    {
        crashSounds = new AudioClip[2];
        crashSounds[0] = Resources.Load<AudioClip>("sounds/crash_heavy");
        crashSounds[1] = Resources.Load<AudioClip>("sounds/crash_heavy"); // Placeholder for glass
    }

    /// <summary>
    /// Triggered by the car's collision detection.
    /// </summary>
    public void RegisterImpact(Collision collision, float impactForce)
    {
        if (impactForce < minImpactForce || isSlowingDown) return;

        StartCoroutine(ExecuteCinematicCrash(collision));
    }

    private IEnumerator ExecuteCinematicCrash(Collision collision)
    {
        isSlowingDown = true;
        originalTimeScale = Time.timeScale;

        // 1. Trigger Slow Motion
        Time.timeScale = slowMoScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2. Camera FX (FOV Jump)
        if (Camera.main != null)
        {
            Camera.main.fieldOfView += 15f; 
        }

        // 3. Play Sound
        if (crashSounds != null && crashSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(crashSounds[Random.Range(0, crashSounds.Length)], collision.contacts[0].point);
        }

        // 4. Spawn Debris (If we had a prefab, otherwise we use a simple cube as placeholder for now)
        SpawnDebris(collision.contacts[0].point);

        yield return new WaitForSecondsRealtime(slowMoDuration);

        // Restore
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
        if (Camera.main != null) Camera.main.fieldOfView -= 15f;
        
        isSlowingDown = false;
    }

    private void SpawnDebris(Vector3 position)
    {
        // For now, spawn 5 random small shards
        for (int i = 0; i < 5; i++)
        {
            GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debris.transform.position = position + Random.insideUnitSphere * 0.5f;
            debris.transform.localScale = Vector3.one * 0.2f;
            Rigidbody rb = debris.AddComponent<Rigidbody>();
            rb.AddExplosionForce(500f, position, 5f);
            Destroy(debris, 3f);
        }
    }
}
