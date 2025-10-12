using UnityEngine;
using TMPro;

public class PlatformDialogue : MonoBehaviour
{
    [Header("Налаштування Префабу")]
    public GameObject dialogueCanvasPrefab;

    [Header("Налаштування діалогу")]
    [TextArea(3, 5)]
    public string dialogueMessage = "Це платформа приносить стабільний дохід...";

    private GameObject currentDialogueInstance;

    public void ToggleDialogue()
    {
        if (currentDialogueInstance != null)
        {
            // Знищуємо вікно, коли воно вже відкрите
            Destroy(currentDialogueInstance);
            currentDialogueInstance = null;

            // --- ДОДАНО: Кажемо камері віддалитися ---
            CameraFollow.Instance.ZoomOut();
        }
        else
        {
            if (dialogueCanvasPrefab == null)
            {
                Debug.LogError("Префаб для діалогу не призначено!", this.gameObject);
                return;
            }

            // Створюємо вікно
            currentDialogueInstance = Instantiate(dialogueCanvasPrefab);

            // --- ДОДАНО: Кажемо камері наблизитися ---
            CameraFollow.Instance.ZoomIn();

            // Оновлюємо текст
            TextMeshProUGUI textComponent = currentDialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = dialogueMessage;
            }
        }
    }

    // Додамо невелике покращення: якщо об'єкт платформи знищується, 
    // а діалог був відкритий, камера повернеться в стандартний стан.
    private void OnDestroy()
    {
        if (currentDialogueInstance != null)
        {
            Destroy(currentDialogueInstance);
            CameraFollow.Instance.ZoomOut();
        }
    }
}