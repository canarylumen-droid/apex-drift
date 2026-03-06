using UnityEngine;

/// <summary>
/// Phase 10: Advanced Social NPC.
/// Replaces basic PedestrianInteraction with complex social state behaviors.
/// </summary>
public class SocialNPC : MonoBehaviour
{
    public enum SocialState { Wandering, Idle, Dancing, Protesting, Aggressive, MatureInteraction, ButtShake }

    [Header("Current State")]
    public SocialState currentState = SocialState.Wandering;
    public float behaviorChangeTimer = 0f;

    [Header("Visuals & Audio")]
    public Animator animator;
    public AudioSource voiceAudioSource;
    public AudioClip murmurSound;
    public AudioClip protestShout;
    public AudioClip angryShout;
    public AudioClip cheerSound;
    
    private SocialNPC partner; // For mature interactions (kissing, holding hands)
    private FacialAnimationController faceController;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        faceController = GetComponent<FacialAnimationController>();
        
        SetupAudio();
        
        // Phase 4: Attach Sexy AI
        gameObject.AddComponent<SexyInteractionAI>();

        // Phase 11: Auto-Bind High-Poly Skin
        if (AssetAutoBinder.Instance != null)
        {
            string modelName = (Random.value > 0.5f) ? "Club_Dancer_A" : "SWAT_Tactical";
            AssetAutoBinder.Instance.BindHighPolyModel(gameObject, modelName);
        }

        // Phase 8: Attach Roaming AI
        if (GetComponent<NPCRoamingAI>() == null) gameObject.AddComponent<NPCRoamingAI>();

        // Phase 15: Attach SWAT Tactical logic if it's high-poly SWAT
        if (gameObject.name.Contains("SWAT")) gameObject.AddComponent<SWATTacticalAI>();

        // Phase 20: Attach Fidgeting & Life logic
        gameObject.AddComponent<CharacterFidgetAI>();

        // Randomize initial behavior if not in a zone
        if (Random.value > 0.8f) SetSocialState(SocialState.Idle);
        else SetSocialState(SocialState.Wandering);
    }

    private void SetupAudio()
    {
        voiceAudioSource = gameObject.AddComponent<AudioSource>();
        voiceAudioSource.spatialBlend = 1.0f; // Full 3D sound
        voiceAudioSource.minDistance = 2f;
        voiceAudioSource.maxDistance = 20f;
        voiceAudioSource.loop = true; // Loop ambient sounds
        
        // Try load default voices if unassigned
        if (murmurSound == null) murmurSound = Resources.Load<AudioClip>("Voices/CrowdMurmur");
        if (protestShout == null) protestShout = Resources.Load<AudioClip>("Voices/ProtestShout");
        if (angryShout == null) angryShout = Resources.Load<AudioClip>("Voices/AngryShout");
        if (cheerSound == null) cheerSound = Resources.Load<AudioClip>("Voices/CheerSounds");
    }

    public void SetSocialState(SocialState state)
    {
        currentState = state;
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Reset all
        animator.SetBool("isDancing", false);
        animator.SetBool("isProtesting", false);
        animator.SetBool("isInteracting", false);

        switch (currentState)
        {
            case SocialState.Dancing:
            case SocialState.ButtShake:
                animator.SetBool("isDancing", true);
                if (currentState == SocialState.ButtShake) animator.SetTrigger("ButtShake");
                PlayVoice(cheerSound, true, 0.6f);
                break;
            case SocialState.Protesting:
                animator.SetBool("isProtesting", true);
                PlayVoice(protestShout, true, 1.0f);
                break;
            case SocialState.MatureInteraction:
                animator.SetBool("isInteracting", true);
                PlayVoice(murmurSound, true, 0.4f);
                break;
            case SocialState.Aggressive:
                PlayVoice(angryShout, false, 1.0f);
                break;
            case SocialState.Idle:
            case SocialState.Wandering:
                PlayVoice(murmurSound, true, 0.3f);
                break;
        }
    }

    private void PlayVoice(AudioClip clip, bool loop, float volume)
    {
        if (voiceAudioSource == null || clip == null) return;
        
        if (voiceAudioSource.clip != clip || !voiceAudioSource.isPlaying)
        {
            voiceAudioSource.clip = clip;
            voiceAudioSource.loop = loop;
            voiceAudioSource.volume = volume;
            voiceAudioSource.Play();
        }
    }

    void Update()
    {
        // Occasional state switching for realism
        behaviorChangeTimer += Time.deltaTime;
        if (behaviorChangeTimer > 30f && currentState == SocialState.Idle)
        {
            SetSocialState(SocialState.Wandering);
            behaviorChangeTimer = 0f;
        }
        
        CheckPlayerProximity();
        HandleInput();
    }

    private void CheckPlayerProximity()
    {
        RCC_CarControllerV3 player = FindObjectOfType<RCC_CarControllerV3>();
        if (player != null && player.canControl && player.GetComponent<RCC_AICarController>() == null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < 5f && player.speed < 10f)
            {
                // Trigger Mobile UI Prompt
                if (MobileInteractionUI.Instance != null && !MobileInteractionUI.Instance.interactionButtonRoot.activeSelf)
                {
                    string label = (currentState == SocialState.MatureInteraction) ? "INTERACT" : "GREET";
                    MobileInteractionUI.Instance.ShowInteraction(label, InteractWithPlayer);
                    
                    // Phase 4: Add Smoke option if idle
                    if (currentState == SocialState.Idle)
                    {
                        // In a real UI, this would be a secondary button
                        Debug.Log("[SocialNPC] Player can now Smoke here.");
                    }
                }
            }
            else if (dist > 7f)
            {
                if (MobileInteractionUI.Instance != null && MobileInteractionUI.Instance.interactionButtonRoot.activeSelf)
                    MobileInteractionUI.Instance.HideInteraction();
            }

            // Phase 12: Fuck-you reaction if player honks or drives recklessly
            if (dist < 10f && player.speed > 80f && currentState != SocialState.Aggressive)
            {
                SetSocialState(SocialState.Aggressive);
            }
        }
    }

    public void InteractWithPlayer()
    {
        Debug.Log("[Highlife] NPC Interactive reaction triggered.");
        
        if (currentState == SocialState.Aggressive)
        {
            animator.SetTrigger("FuckYouHandSign"); // Requires Phase 8/12 Animation
            PlayVoice(angryShout, false, 1.0f);
        }
        else if (currentState == SocialState.MatureInteraction)
        {
             animator.SetTrigger("SexyInteraction");
             // Randomly play kissing sounds if close enough
             if (Random.value > 0.5f && CinematicAudioManager.Instance != null)
                CinematicAudioManager.Instance.PlayCinematicSound("Kissing", 1.0f);
        }
        else 
        {
            animator.SetTrigger("Wave");
            PlayVoice(cheerSound, false, 0.8f);
            
            // Phase 20: Vocal Greet
            if (GlobalVoiceHook.Instance != null)
                GlobalVoiceHook.Instance.SpeakText("Hey! Nice car!", AdvancedProceduralTTS.Emotion.Cheer);
        }
    }

    // Phase 4: Smoke interaction that triggers police
    public void StartSmoking()
    {
        Debug.Log("[SocialNPC] Player is smoking. Triggering citizen report.");
        animator.SetTrigger("SmokeAnim");
        
        if (WantedSystem.Instance != null)
        {
            WantedSystem.Instance.AddWantedLevel(1);
            if (AdvancedProceduralTTS.Instance != null)
                AdvancedProceduralTTS.Instance.Speak("Hey! You can't do that here! I'm calling the cops!", AdvancedProceduralTTS.Emotion.Angry);
        }
    }
}
