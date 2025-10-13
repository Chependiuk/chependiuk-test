using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlatformDialogue : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Перетягніть сюди ПРЕФАБ вашого канвасу з вікна Project.")]
    public GameObject dialogueCanvasPrefab;

    [Tooltip("Контекст, який додається до запиту гравця для AI.")]
    [TextArea(3, 10)]
    public string aiPromptPrefix = "Ти — NPC-помічник у грі. Гравець поставив тобі запитання. Дай коротку, корисну відповідь у стилі гри. Ось запитання гравця: ";

    [Header("Налаштування Керування")]
    [Tooltip("Вкажіть точну назву об'єкта джойстика у вашій сцені.")]
    public string joystickObjectName = "Dynamic Joystick";

    private GameObject currentDialogueInstance;
    private TextMeshProUGUI responseTextComponent;
    private TMP_InputField userInputField;
    private Button sendButton;
    private GameObject joystickObject;

    private void Start()
    {
        joystickObject = GameObject.Find(joystickObjectName);
        if (joystickObject == null)
        {
            Debug.LogWarning($"Не вдалося знайти об'єкт джойстика з назвою '{joystickObjectName}'. Перевірте назву.");
        }
    }

    public void ToggleDialogue()
    {
        if (currentDialogueInstance != null)
        {
            CloseDialogue();
        }
        else
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        if (dialogueCanvasPrefab == null)
        {
            Debug.LogError("Префаб канвасу не призначено в інспекторі!");
            return;
        }

        if (joystickObject != null)
        {
            joystickObject.SetActive(false);
        }

        currentDialogueInstance = Instantiate(dialogueCanvasPrefab);
        CameraFollow.Instance.ZoomIn();

        userInputField = currentDialogueInstance.GetComponentInChildren<TMP_InputField>();
        sendButton = currentDialogueInstance.GetComponentInChildren<Button>();

        // --- ВИПРАВЛЕНО ТУТ ---
        // Використовуємо правильну назву класу: TextMeshProUGUI
        foreach (var text in currentDialogueInstance.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (text.GetComponentInParent<TMP_InputField>() == null && text.GetComponentInParent<Button>() == null)
            {
                responseTextComponent = text;
                break;
            }
        }

        if (userInputField == null || sendButton == null || responseTextComponent == null)
        {
            Debug.LogError("Не вдалося знайти всі UI компоненти (InputField, Button, Text) на префабі канвасу. Перевірте їх наявність.");
            CloseDialogue();
            return;
        }

        PrepareDialogueWindow();
    }

    private void CloseDialogue()
    {
        if (currentDialogueInstance == null) return;

        if (joystickObject != null)
        {
            joystickObject.SetActive(true);
        }

        EventSystem.current.SetSelectedGameObject(null);
        Destroy(currentDialogueInstance);
        currentDialogueInstance = null;
        CameraFollow.Instance.ZoomOut();
    }

    private void PrepareDialogueWindow()
    {
        responseTextComponent.text = "Поставте ваше запитання...";
        userInputField.text = "";

        userInputField.Select();
        userInputField.ActivateInputField();

        sendButton.onClick.RemoveAllListeners();
        sendButton.onClick.AddListener(OnSendButtonClick);
    }

    public void OnSendButtonClick()
    {
        if (string.IsNullOrWhiteSpace(userInputField.text))
        {
            responseTextComponent.text = "Поле вводу не може бути порожнім.";
            return;
        }

        string finalPrompt = aiPromptPrefix + userInputField.text;
        responseTextComponent.text = "Аналізую ваш запит...";
        sendButton.interactable = false;

        EventSystem.current.SetSelectedGameObject(null);
        APIManager.Instance.SendPromptToGemini(finalPrompt, OnGeminiResponseReceived);
    }

    private void OnGeminiResponseReceived(string response)
    {
        if (currentDialogueInstance != null)
        {
            responseTextComponent.text = response;
            sendButton.interactable = true;
        }
    }
}