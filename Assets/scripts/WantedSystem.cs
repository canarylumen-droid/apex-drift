using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GTA-Style Wanted System for Apex Drift.
/// Tracks crimes and escalates from 1-star (minor chase) to 5-stars (Helicopters).
/// </summary>
public class WantedSystem : MonoBehaviour
{
    public static WantedSystem Instance { get; private set; }

    public enum OffenseType { Speeding, Collision, HittingPedestrian, PropertyDamage }

    [Header("Wanted Status")]
    public int wantedStars = 0;
    public float pursuitIntensity = 0f;
    public bool isUnderChase = false;

    [Header("Escalation Settings")]
    public float starCooldownTime = 20f;
    public float intensityDecay = 0.5f;

    private float lastOffenseTime;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Update()
    {
        if (isUnderChase)
        {
            // Decay intensity if out of line-of-sight (simulation)
            pursuitIntensity -= intensityDecay * Time.deltaTime;
            
            if (pursuitIntensity < 0)
            {
                pursuitIntensity = 0;
                if (Time.time - lastOffenseTime > starCooldownTime)
                {
                    LoseStar();
                }
            }
        }
    }

    /// <summary>
    /// Registers a crime and escalates wanted status.
    /// </summary>
    public void RegisterOffense(OffenseType offense)
    {
        lastOffenseTime = Time.time;
        isUnderChase = true;

        switch (offense)
        {
            case OffenseType.Speeding:
                pursuitIntensity += 10f;
                break;
            case OffenseType.Collision:
                pursuitIntensity += 25f;
                break;
            case OffenseType.PropertyDamage:
                pursuitIntensity += 30f;
                if (wantedStars < 1) wantedStars = 1;
                break;
            case OffenseType.HittingPedestrian:
                pursuitIntensity += 100f;
                wantedStars += 1;
                break;
        }

        // Cap stars
        if (wantedStars > 5) wantedStars = 5;

        // Auto-gain first star on moderate crimes
        if (pursuitIntensity > 50f && wantedStars == 0) wantedStars = 1;

        Debug.Log("[WantedSystem] Offense Registered: " + offense + " | Stars: " + wantedStars);
        
        // Trigger sirens in PoliceChaseManager
        if (PoliceChaseManager.Instance != null && wantedStars > 0)
        {
            PoliceChaseManager.Instance.OnHighImpactCollision(); // Forces sirens on
        }
    }

    private void LoseStar()
    {
        if (wantedStars > 0)
        {
            wantedStars--;
            Debug.Log("[WantedSystem] Star Lost. Count: " + wantedStars);
            
            if (wantedStars == 0)
                isUnderChase = false;
        }
    }

    public void EscapePursuit()
    {
        wantedStars = 0;
        isUnderChase = false;
        Debug.Log("[WantedSystem] Pursuit Envaded!");
    }
}
