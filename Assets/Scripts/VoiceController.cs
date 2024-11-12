using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour
{ 
    AndroidJavaObject activity;
    AndroidJavaObject STTplugin;
    AndroidJavaObject TTSplugin = null;
    private bool isintialised = false;

    public delegate void OnResultRecieved(string result);
    public static OnResultRecieved resultRecieved;

    private void Start()
    {
        InitPlugin();
    }

    public void InitPlugin()
    {
        Debug.Log("Initializing VoiceController plugin...");

        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                STTplugin = new AndroidJavaObject("com.example.matthew.plugin.VoiceBridge");
            }));

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                STTplugin.Call("StartPlugin");
            }));

            using (AndroidJavaClass ttsClass = new AndroidJavaClass("com.busa.ttslibrary.androidSpeechRecognition"))
            {
                if (ttsClass != null)
                {
                    TTSplugin = ttsClass.CallStatic<AndroidJavaObject>("instance");
                    if (TTSplugin != null)
                    {
                        TTSplugin.Call("setContext", activity);
                        Debug.Log("TTSplugin initialized successfully.");
                        isintialised = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to create TTSplugin instance.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to find com.busa.ttslibrary.androidSpeechRecognition class.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception during plugin initialization: " + e.Message);
        }
    }

    /// <summary>
    /// Gets called via SendMessage from the android plugin. GameObject must be called "VoiceController".
    /// </summary>
    /// <param name="recognizedText">recognizedText.</param>
    public void OnVoiceResult(string recognizedText)
    {
        Debug.Log("Voice Result: " + recognizedText);
        resultRecieved?.Invoke(recognizedText);
    }

    /// <summary>
    /// Gets called via SendMessage from the android plugin.
    /// </summary>
    /// <param name="error">Error.</param>
    public void OnErrorResult(string error)
    {
        Debug.LogError("Voice Error: " + error);
    }

    public void GetSpeech()
    {
        // Calls the function from the jar file
        if (STTplugin != null)
        {
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                STTplugin.Call("StartSpeaking");
            }));
        }
        else
        {
            Debug.LogWarning("STTplugin is not initialized.");
        }
    }

    public void TTS(string text, float pitch)
    {
        if (TTSplugin != null)
        {
            TTSplugin.Call("SpeakTTS", "US", text, pitch, 1.0f);
        }
        else
        {
            Debug.LogWarning("TTSplugin is not initialized. Cannot perform text-to-speech.");
        }
    }

    public bool isSpeaking()
    {
        if (TTSplugin == null)
        {
            Debug.LogWarning("TTSplugin is not initialized.");
            return false;
        }
        return TTSplugin.Call<bool>("isSpeaking");
    }

    public bool isIntialised()
    {
        return isintialised;
    }
}
