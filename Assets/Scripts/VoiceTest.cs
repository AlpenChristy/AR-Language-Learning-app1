using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VoiceController))]
public class VoiceTest : MonoBehaviour
{
    // public Text uiText;
    public string myResponse;

    private VoiceController voiceController;

    void Start()
    {
        // Initialize the VoiceController instance in Start
        voiceController = GetComponent<VoiceController>();
        InitPlugin(); // Ensure plugin initialization
    }

    public void GetSpeech()
    {
        if (voiceController == null)
        {
            Debug.LogError("VoiceController is not initialized.");
            return;
        }

        myResponse = null;
        voiceController.GetSpeech();
    }

    public void InitPlugin() // Corrected method name
    {
        if (voiceController == null)
        {
            Debug.LogError("VoiceController component is missing.");
            return;
        }

        myResponse = null;
        voiceController.InitPlugin();
    }

    public IEnumerator TTS(string text)
    {
        if (voiceController == null)
        {
            Debug.LogError("VoiceController is not initialized.");
            yield break;
        }

        voiceController.TTS(text, 0.4f);
    }

    void OnEnable()
    {
        VoiceController.resultRecieved += OnVoiceResult;
    }

    void OnDisable()
    {
        VoiceController.resultRecieved -= OnVoiceResult;
    }

    void OnVoiceResult(string text)
    {
        myResponse = text;
    }

    public bool isSpeaking()
    {
        return voiceController != null && voiceController.isSpeaking();
    }

    public bool isIntialised()
    {
        return voiceController != null && voiceController.isIntialised();
    }
}
