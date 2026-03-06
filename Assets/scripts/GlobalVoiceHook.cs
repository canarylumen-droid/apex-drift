using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase 20: Global Voice Hook.
/// The engine's "Voice Box." Automatically speaks any text passed to it.
/// Links to AdvancedProceduralTTS and coordinates lip-sync for characters.
/// </summary>
public class GlobalVoiceHook : MonoBehaviour
{
    public static GlobalVoiceHook Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Phase 27: Million Dollar Brain (Keyword-Response Logic)
    private Dictionary<string, string[]> brainDatabase = new Dictionary<string, string[]>()
    {
        { "HI", new string[] { "Hey there! Looking for trouble?", "Hello. Stay safe out there.", "Yo! What's up?" } },
        { "HELLO", new string[] { "Greetings, citizen.", "Hello! Nice car.", "Hey! Don't get arrested." } },
        { "POLICE", new string[] { "The cops are everywhere today.", "I heard the SWAT is moving in.", "Don't mention the police to me." } },
        { "MONEY", new string[] { "Always about the green, isn't it?", "I'm working on my million, you?", "Money talks, friend." } },
        { "HELP", new string[] { "I can't help you right now.", "Check the mission board.", "You look like you can handle yourself." } },
        { "YO", new string[] { "Sup? You ready for the drift?", "Yo! Let's get it.", "Peace, brother." } }
    };

    public void SpeakText(string text, AdvancedProceduralTTS.Emotion emotion = AdvancedProceduralTTS.Emotion.Neutral)
    {
        if (AdvancedProceduralTTS.Instance != null)
        {
            string reply = GetBrainResponse(text);
            AdvancedProceduralTTS.Instance.Speak(reply, emotion);
            Debug.Log("[GlobalVoice] Input: " + text + " | Brain Reply: " + reply);
        }
    }

    private string GetBrainResponse(string input)
    {
        string upperInput = input.ToUpper().Trim();
        foreach (var entry in brainDatabase)
        {
            if (upperInput.Contains(entry.Key))
            {
                return entry.Value[UnityEngine.Random.Range(0, entry.Value.Length)];
            }
        }
        
        // Fallback to random lines if no keyword matches
        return waitLines[UnityEngine.Random.Range(0, waitLines.Length)];
    }

    // Phase 22: Advanced Conversational Loops
    private string[] sorryLines = { "Sorry, I'm busy.", "Not now, friend.", "Come again later." };
    private string[] waitLines = { "Yes? What is it?", "I'm listening.", "Speak up." };

    public void TriggerWaitResponse()
    {
        string line = waitLines[UnityEngine.Random.Range(0, waitLines.Length)];
        SpeakText(line, AdvancedProceduralTTS.Emotion.Neutral);
    }

    public void TriggerSorryResponse()
    {
        string line = sorryLines[UnityEngine.Random.Range(0, sorryLines.Length)];
        SpeakText(line, AdvancedProceduralTTS.Emotion.Neutral);
    }

    // Call this from any UI Button to make it talk
    public void SpeakUIButton(Text uiText)
    {
        if (uiText != null) SpeakText(uiText.text);
    }
}
