using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlatformDialogue : MonoBehaviour, IInteractable
{
    [Header("Налаштування")]
    public GameObject dialogueCanvasPrefab;
    public int portraitIndexToShow = -1;

    [Header("Генерація Імені")]
    public List<string> characterNames;

    [Header("Налаштування AI")]
    [TextArea(3, 10)]
    public string aiPromptPrefix = "Ти — NPC-помічник. Дай коротку відповідь на запитання гравця.";

    private GameObject currentDialogueInstance;
    private TextMeshProUGUI responseTextComponent;
    private TMP_InputField userInputField;
    private Button sendButton;
    private TextMeshProUGUI nameTextUI;
    private GameObject portraitsContainerUI;
    private string generatedName;

    /// <summary>
    /// Публічний метод, щоб інші скрипти могли перевірити, чи відкритий діалог.
    /// </summary>
    public bool IsDialogueOpen()
    {
        return currentDialogueInstance != null;
    }

    // --- Секція IInteractable ---

    public void HandleInteraction(KeyCode key)
    {
        // 'E' або 'T' тільки відкривають діалог, якщо він закритий
        if ((key == KeyCode.T || key == KeyCode.E) && !IsDialogueOpen())
        {
            OpenDialogue();
        }
    }

    public InteractionType GetActiveInteractionType(KeyCode key)
    {
        // Показуємо підказку, тільки якщо діалог ще не відкритий
        if (!IsDialogueOpen() && (key == KeyCode.T || key == KeyCode.E))
        {
            return InteractionType.Chat;
        }
        return InteractionType.None;
    }

    public string GetInteractionText(KeyCode key, float dist, float requiredDist)
    {
        if (!IsDialogueOpen() && (key == KeyCode.T || key == KeyCode.E))
        {
            return "Говорити [T]";
        }
        return "";
    }

    // --- Основна логіка ---

    public void ToggleDialogue()
    {
        if (currentDialogueInstance != null) CloseDialogue();
        else OpenDialogue();
    }

    private void OpenDialogue()
    {
        if (dialogueCanvasPrefab == null) return;
        if (PlayerMovement.Instance != null) { PlayerMovement.Instance.SetMovementState(false); PlayerMovement.Instance.HideMainHUD(); }
        if (CameraFollow.Instance != null) CameraFollow.Instance.ZoomIn();

        currentDialogueInstance = Instantiate(dialogueCanvasPrefab);
        FindUIReferences();

        Button closeButton = currentDialogueInstance.transform.Find("CloseButton")?.GetComponent<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseDialogue);
        }
        else
        {
            Debug.LogWarning("На префабі діалогу не знайдено кнопку з назвою 'CloseButton'!");
        }

        if (userInputField == null || sendButton == null || responseTextComponent == null)
        {
            Debug.LogError("Не вдалося знайти базові UI компоненти на префабі.");
            CloseDialogue();
            return;
        }
        PrepareDialogueWindow();
    }

    public void CloseDialogue()
    {
        if (currentDialogueInstance == null) return;
        if (PlayerMovement.Instance != null) { PlayerMovement.Instance.SetMovementState(true); PlayerMovement.Instance.ShowMainHUD(); }
        if (CameraFollow.Instance != null) CameraFollow.Instance.ZoomOut();

        Destroy(currentDialogueInstance);
        currentDialogueInstance = null;
    }

    private void FindUIReferences()
    {
        if (currentDialogueInstance == null) return;
        userInputField = currentDialogueInstance.GetComponentInChildren<TMP_InputField>(true);
        sendButton = currentDialogueInstance.GetComponentInChildren<Button>(true);
        Transform containerTransform = currentDialogueInstance.transform.Find("PortraitsContainer");
        if (containerTransform != null)
        {
            portraitsContainerUI = containerTransform.gameObject;
            Transform nameTextTransform = portraitsContainerUI.transform.Find("NameText");
            if (nameTextTransform != null) nameTextUI = nameTextTransform.GetComponent<TextMeshProUGUI>();
        }
        foreach (var text in currentDialogueInstance.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text.GetComponentInParent<Button>() == null && text.GetComponentInParent<TMP_InputField>() == null && (nameTextUI == null || text != nameTextUI))
            {
                responseTextComponent = text;
                break;
            }
        }
    }

    private void PrepareDialogueWindow()
    {
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
            for (int i = 0; i < portraitsContainerUI.transform.childCount; i++)
            {
                GameObject childObject = portraitsContainerUI.transform.GetChild(i).gameObject;
                if (childObject.name == "NameText") continue;
                childObject.SetActive(i == portraitIndexToShow);
            }
            if (nameTextUI != null)
            {
                if (string.IsNullOrEmpty(generatedName)) generatedName = GetRandomName();
                nameTextUI.text = generatedName;
            }
        }
    }

    private string GetRandomName()
    {
        if (characterNames != null && characterNames.Count > 0) return characterNames[Random.Range(0, characterNames.Count)];
        return "Незнайомець";
    }

    public void OnSendButtonClick()
    {
        if (userInputField == null) return;
        string userInput = userInputField.text;
        if (string.IsNullOrWhiteSpace(userInput))
        {
            responseTextComponent.text = "Поле вводу не може бути порожнім.";
            return;
        }

        if (userInput.ToLower().Contains("прокачай"))
        {
            IncomePlatform platform = GetComponentInParent<IncomePlatform>();
            if (platform != null)
            {
                platform.TryUpgrade();
                responseTextComponent.text = $"Рівень об'єкта підвищено до {platform.level}!";
                userInputField.text = "";
                userInputField.ActivateInputField();
                return;
            }
            else
            {
                responseTextComponent.text = "Тут нічого покращувати.";
                userInputField.text = "";
                userInputField.ActivateInputField();
                return;
            }
        }

        string finalPrompt = aiPromptPrefix + userInput;
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
            if (userInputField != null) userInputField.ActivateInputField();
        }
    }
}