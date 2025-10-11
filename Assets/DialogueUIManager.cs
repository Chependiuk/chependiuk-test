using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueCanvas;
    public TMP_InputField inputField;
    public TextMeshProUGUI responseText;

    [Header("Core Managers")]
    public APIManager apiManager;

    private NPCDialogue currentNPC;

    void Start()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }

        if (apiManager == null)
        {
            apiManager = FindObjectOfType<APIManager>();
        }

        // ПІДКЛЮЧЕННЯ ENTER: Найнадійніший метод
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnInputSubmitted);
        }
    }

    // Відкриває діалог (Викликається PlayerInteraction)
    public void OpenDialogue(NPCDialogue npc)
    {
        currentNPC = npc;

        if (dialogueCanvas != null)
        {
            // Активація Canvas і його батьківського елемента
            if (dialogueCanvas.transform.parent != null && !dialogueCanvas.transform.parent.gameObject.activeSelf)
            {
                dialogueCanvas.transform.parent.gameObject.SetActive(true);
            }
            dialogueCanvas.SetActive(true);
        }

        if (inputField != null)
        {
            inputField.text = "";
            inputField.Select();
        }
        SetResponseText($"Розмова з {npc.CharacterName}. Введіть ваше запитання...");
    }

    // Обробляє відповідь від APIManager (Callback)
    public void DisplayResponse(string response)
    {
        if (dialogueCanvas != null && !dialogueCanvas.activeSelf)
        {
            dialogueCanvas.SetActive(true);
        }

        SetResponseText(response);

        if (inputField != null)
        {
            inputField.ActivateInputField();
        }
    }

    public void SetResponseText(string text)
    {
        if (responseText != null)
        {
            responseText.text = text;
        }
    }

    // Викликається при On End Edit
    private void OnInputSubmitted(string input)
    {
        if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            return;
        }

        if (currentNPC != null && apiManager != null && !string.IsNullOrEmpty(input))
        {
            apiManager.SendPromptToGemini(input, DisplayResponse);

            SetResponseText("Генерування відповіді...");
            inputField.text = "";
        }
    }

    // Закриття діалогу
    public void CloseDialogue()
    {
        currentNPC = null;
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogueCanvas != null && dialogueCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
        }
    }
}