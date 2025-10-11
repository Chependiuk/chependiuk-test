using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Character Settings")]
    public string CharacterName = "NPC";

    [Header("UI Activation")]
    [Tooltip("Призначте Canvas/Panel, який потрібно активувати при взаємодії.")]
    public GameObject dialogueCanvas;

    public void TryActivateDialogue()
    {
        Debug.Log($"[DEBUG] Початок TryActivateDialogue для {CharacterName}");
        if (dialogueCanvas != null)
        {
            bool wasActive = dialogueCanvas.activeSelf;
            dialogueCanvas.SetActive(true);
            Debug.Log($"--- Canvas {dialogueCanvas.name} змінено з {wasActive} на true для {CharacterName}! ---");
            if (!dialogueCanvas.activeInHierarchy)
            {
                Debug.LogWarning($"--- Canvas {dialogueCanvas.name} не активний у ієрархії для {CharacterName}! Перевірте батьківські об’єкти. ---");
            }
        }
        else
        {
            Debug.LogWarning($"--- dialogueCanvas не прив'язаний для {CharacterName}! ---");
        }
        Debug.Log($"[DEBUG] Кінець TryActivateDialogue для {CharacterName}");
    }
}