using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using System;

public class APIManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static APIManager Instance { get; private set; }

    private const string GAS_URL = "https://script.google.com/macros/s/AKfycbyEb48eQF-gC9L4uhFNQnULgECLnGAvmJilMZkFiWj_XdwPAS6Drhpl7cndwIWTNpx6CA/exec";

    [Header("UI Reference")]
    public TextMeshProUGUI debugResponseText;

    private void Awake()
    {
        // Налаштування Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SendPromptToGemini(string prompt, Action<string> callback)
    {
        Debug.Log($"APIManager: Надсилаємо запит...");
        StartCoroutine(SendRequestCoroutine(prompt, callback));
    }

    private IEnumerator SendRequestCoroutine(string prompt, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);

        using (UnityWebRequest www = UnityWebRequest.Post(GAS_URL, form))
        {
            yield return www.SendWebRequest();

            string geminiResponse = "";

            if (www.result == UnityWebRequest.Result.Success)
            {
                geminiResponse = www.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"Помилка запиту: {www.error}.");
                geminiResponse = "Помилка: Не вдалося підключитися.";
            }

            callback?.Invoke(geminiResponse);

            if (debugResponseText != null)
            {
                debugResponseText.text = geminiResponse;
            }
        }
    }
}