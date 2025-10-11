using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using System; // Необхідно для Action (метод зворотного виклику)

public class APIManager : MonoBehaviour
{
    // АКТУАЛЬНИЙ URL Google Apps Script вставлено тут
    private const string GAS_URL = "https://script.google.com/macros/s/AKfycbwY0uTn8jw2Z1sYjqvgBZ-fKU-Tg3s0-XtLRRV9sSn6Uy5QMEtcOydnfiBoFuXuPr9piA/exec";

    [Header("UI Reference")]
    public TextMeshProUGUI debugResponseText;

    public void SendPromptToGemini(string prompt, Action<string> callback)
    {
        Debug.Log($"APIManager викликано. Надсилаємо: {prompt.Substring(0, Mathf.Min(prompt.Length, 50))}...");
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
                Debug.LogError($"Помилка запиту: {www.error}. Перевірте URL та розгортання скрипта GAS.");
                geminiResponse = $"Помилка: Не вдалося підключитися. Перевірте GAS.";
            }

            callback?.Invoke(geminiResponse);
            Debug.Log("Відповідь Gemini: " + geminiResponse);

            if (debugResponseText != null)
            {
                debugResponseText.text = geminiResponse;
            }
        }
    }

    void Start() { }
    void Update() { }
}