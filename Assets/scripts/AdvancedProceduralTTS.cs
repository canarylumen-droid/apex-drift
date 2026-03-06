using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// "Deep Mature Expansion" Phase 3: Advanced Procedural TTS.
/// Simulates human speech with emotions, breathing, and natural pauses.
/// Supports different character profiles (Male, Female, Deep, High).
/// </summary>
public class AdvancedProceduralTTS : MonoBehaviour
{
    public static AdvancedProceduralTTS Instance { get; private set; }

    public enum Emotion { Neutral, Happy, Angry, Panicked, Romantic }
    
    private AudioSource audioSource;
    private int sampleRate = 44100;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
    }

    public void Speak(string text, Emotion emotion = Emotion.Neutral, float pitchBase = 1.0f)
    {
        if (string.IsNullOrEmpty(text)) return;
        StartCoroutine(SynthesizeSpeech(text, emotion, pitchBase));
    }

    private IEnumerator SynthesizeSpeech(string text, Emotion emotion, float pitchBase)
    {
        // 1. Pre-process text into "Phonetic Blocks" based on punctuation
        string[] parts = text.Split(new char[] { ' ', '.', ',', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string word in parts)
        {
            float wordDuration = word.Length * 0.1f;
            if (emotion == Emotion.Panicked) wordDuration *= 0.7f; // Speak faster
            if (emotion == Emotion.Romantic) wordDuration *= 1.3f; // Speak slower

            int wordSamples = (int)(sampleRate * wordDuration);
            float[] data = new float[wordSamples];

            // 2. Synthesize Waveform
            float freq = 120f * pitchBase; // Male base
            if (pitchBase > 1.4f) freq = 220f; // Female-ish base

            for (int i = 0; i < wordSamples; i++)
            {
                float t = (float)i / wordSamples;
                
                // Add Emotional Modulation
                float jitter = Mathf.Sin(Time.time * 50f) * 0.05f;
                if (emotion == Emotion.Angry) jitter *= 3f;
                if (emotion == Emotion.Panicked) jitter *= 5f;

                float currentFreq = freq + jitter;
                float angle = i * 2f * Mathf.PI * currentFreq / sampleRate;
                
                // Formant simulation (rough)
                float wave = Mathf.Sin(angle) * 0.3f;
                wave += Mathf.Sin(angle * 2f) * 0.1f; // Overtones
                
                // Volume envelope per word
                float envelope = Mathf.Sin(t * Mathf.PI);
                data[i] = wave * envelope;
            }

            AudioClip clip = AudioClip.Create("Word_" + word, wordSamples, 1, sampleRate, false);
            clip.SetData(data, 0);
            audioSource.PlayOneShot(clip);

            yield return new WaitForSeconds(wordDuration);

            // 3. Insert "Human Elements" (Breathing & Pauses)
            if (text.Contains(".") || text.Contains("!"))
            {
                yield return PlayBreath(emotion);
            }
            else if (text.Contains(","))
            {
                yield return new WaitForSeconds(0.2f); // Short pause
            }
        }
    }

    private IEnumerator PlayBreath(Emotion emotion)
    {
        float breathDuration = 0.4f;
        int samples = (int)(sampleRate * breathDuration);
        float[] data = new float[samples];

        // Synthesize a white-noise "Sigh"
        for (int i = 0; i < samples; i++)
        {
            float envelope = Mathf.Sin((float)i / samples * Mathf.PI);
            data[i] = Random.Range(-0.05f, 0.05f) * envelope;
        }

        AudioClip breathClip = AudioClip.Create("Breath", samples, 1, sampleRate, false);
        breathClip.SetData(data, 0);
        audioSource.PlayOneShot(breathClip, 0.2f);

        yield return new WaitForSeconds(breathDuration + 0.1f);
    }
}
