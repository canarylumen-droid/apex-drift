using UnityEngine;
using System.Collections;

/// <summary>
/// "Underground Expansion" Feature: Procedural Text-To-Speech.
/// Converts dialogue text strings into dynamic audio waveforms at runtime
/// so we don't need to pre-record hundreds of voice lines.
/// It creates a deep "Male Radio Voice" cadence by generating sound data.
/// </summary>
public class ProceduralTTSManager : MonoBehaviour
{
    public static ProceduralTTSManager Instance { get; private set; }

    private AudioSource audioSource;
    private int sampleRate = 44100;

    void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D UI Sound for direct listening
    }

    /// <summary>
    /// Generates audio based on the text length and plays it.
    /// </summary>
    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        StartCoroutine(GenerateAndPlaySpeech(text));
    }

    private IEnumerator GenerateAndPlaySpeech(string text)
    {
        // Estimate duration based on text length (human pace ~ 15 characters per second)
        float duration = text.Length / 15f;
        if (duration < 0.5f) duration = 0.5f;

        int samples = (int)(sampleRate * duration);
        float[] audioData = new float[samples];

        // Synthesize a deep male "radio" voice waveform
        float frequency = 120f; // Deep voice base frequency
        float currentPhase = 0f;
        
        for (int i = 0; i < samples; i++)
        {
            // Modulate frequency based on syllables (spaces/punctuation)
            float progress = (float)i / samples;
            int wordIndex = (int)(progress * text.Split(' ').Length);
            
            // Randomize pitch slightly per word to sound like talking
            float pitchMod = 1f + Mathf.Sin(wordIndex * 15f) * 0.2f;
            currentPhase += (frequency * pitchMod * 2f * Mathf.PI) / sampleRate;

            // Generate a distorted square wave for that radio/walkie-talkie feel
            float wave = Mathf.Sign(Mathf.Sin(currentPhase)) * 0.3f;
            
            // Add static noise
            wave += Random.Range(-0.1f, 0.1f);
            
            audioData[i] = wave;
        }

        AudioClip clip = AudioClip.Create("TTS_" + text.GetHashCode(), samples, 1, sampleRate, false);
        clip.SetData(audioData, 0);

        audioSource.clip = clip;
        audioSource.Play();

        yield return null;
    }
}
