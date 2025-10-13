using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlatformDialogue : MonoBehaviour
{
    [Header("Налаштування")]
    public GameObject dialogueCanvasPrefab;
    [Tooltip("Вкажіть номер портрета, який потрібно активувати (0, 1, 2...). Вкажіть -1, щоб нічого не показувати.")]
    public int portraitIndexToShow = -1;

    [Header("Генерація Імені")]
    [Tooltip("Список імен, з яких буде вибрано випадкове для цього діалогу.")]
    public List<string> characterNames;

    [Header("Налаштування AI")]
    [TextArea(3, 10)]
    public string aiPromptPrefix = "Ти — NPC-помічник...";

    [Header("Налаштування Керування")]
    public string joystickObjectName = "Dynamic Joystick";

    // Приватні посилання
    private GameObject currentDialogueInstance;
    private TextMeshProUGUI responseTextComponent;
    private TMP_InputField userInputField;
    private Button sendButton;
    private GameObject joystickObject;
    private TextMeshProUGUI nameTextUI;
    private GameObject portraitsContainerUI;

    private void Start()
    {
        joystickObject = GameObject.Find(joystickObjectName);
        if (joystickObject == null) { Debug.LogWarning($"Не вдалося знайти джойстик з назвою '{joystickObjectName}'."); }
    }

    public void ToggleDialogue()
    {
        if (currentDialogueInstance != null) CloseDialogue();
        else OpenDialogue();
    }

    private void OpenDialogue()
    {
        if (dialogueCanvasPrefab == null) return;
        if (joystickObject != null) joystickObject.SetActive(false);

        currentDialogueInstance = Instantiate(dialogueCanvasPrefab);

        // --- НОВА, РОЗУМНА ЛОГІКА ПОШУКУ ---
        // Знаходимо всі текстові поля на префабі
        TextMeshProUGUI[] allTexts = currentDialogueInstance.GetComponentsInChildren<TextMeshProUGUI>(true); // true - щоб знайти і неактивні
        foreach (var text in allTexts)
        {
            // Якщо це текст імені, зберігаємо його
            if (text.gameObject.name == "NameText")
            {
                nameTextUI = text;
            }
            // Інакше, якщо це не текст на кнопці чи в полі вводу, вважаємо його головним полем для відповідей
            else if (text.GetComponentInParent<Button>() == null && text.GetComponentInParent<TMP_InputField>() == null)
            {
                responseTextComponent = text;
            }
        }

        // Знаходимо інші елементи
        userInputField = currentDialogueInstance.GetComponentInChildren<TMP_InputField>(true);
        sendButton = currentDialogueInstance.GetComponentInChildren<Button>(true);
        Transform containerTransform = currentDialogueInstance.transform.Find("PortraitsContainer");
        if (containerTransform != null) portraitsContainerUI = containerTransform.gameObject;

        // Перевірка
        if (userInputField == null || sendButton == null || responseTextComponent == null) { Debug.LogError("Не вдалося знайти базові UI компоненти (InputField, Button, ResponseText) на префабі."); CloseDialogue(); return; }

        CameraFollow.Instance.ZoomIn();
        PrepareDialogueWindow();
    }

    private void PrepareDialogueWindow()
    {
        responseTextComponent.text = "Поставте ваше запитання...";
        userInputField.text = "";

        userInputField.Select();
        userInputField.ActivateInputField();

        sendButton.onClick.RemoveAllListeners();
        sendButton.onClick.AddListener(OnSendButtonClick);

        ActivatePortraitAndName();
    }

    private void ActivatePortraitAndName()
    {
        if (portraitsContainerUI == null) return;

        bool showPortrait = portraitIndexToShow >= 0;
        portraitsContainerUI.SetActive(showPortrait);

        if (showPortrait)
        {
            // Проходимо по дочірніх об'єктах контейнера (це наші портрети)
            for (int i = 0; i < portraitsContainerUI.transform.childCount; i++)
            {
                GameObject childObject = portraitsContainerUI.transform.GetChild(i).gameObject;
                // Ігноруємо об'єкт з іменем
                if (childObject.name == "NameText") continue;

                // Вмикаємо потрібний портрет за індексом
                childObject.SetActive(i == portraitIndexToShow);
            }

            if (nameTextUI != null)
            {
                nameTextUI.text = GetRandomName();
            }
        }
    }

    private string GetRandomName()
    {
        if (characterNames != null && characterNames.Count > 0)
        {
            return characterNames[Random.Range(0, characterNames.Count)];
        }
        return "Незнайомець";
    }

    private void CloseDialogue()
    {
        if (currentDialogueInstance == null) return;
        if (joystickObject != null) joystickObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        Destroy(currentDialogueInstance);
        currentDialogueInstance = null;
        CameraFollow.Instance.ZoomOut();
    }

    public void OnSendButtonClick()
    {
        if (string.IsNullOrWhiteSpace(userInputField.text)) { responseTextComponent.text = "Поле вводу не може бути порожнім."; return; }
        string finalPrompt = aiPromptPrefix + userInputField.text;
        responseTextComponent.text = "Аналізую ваш запит...";
        sendButton.interactable = false;
        EventSystem.current.SetSelectedGameObject(null);
        APIManager.Instance.SendPromptToGemini(finalPrompt, OnGeminiResponseReceived);
    }

    private void OnGeminiResponseReceived(string response)
    {
        if (currentDialogueInstance != null) { responseTextComponent.text = response; sendButton.interactable = true; }
    }
}