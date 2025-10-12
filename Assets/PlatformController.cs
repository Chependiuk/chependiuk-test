using UnityEngine;

// БЕЗ IInteractable
public class PlatformController : MonoBehaviour
{
    private IncomePlatform incomeComponent;
    private PlatformDialogue dialogueComponent;

    private void Awake()
    {
        incomeComponent = GetComponent<IncomePlatform>();
        dialogueComponent = GetComponent<PlatformDialogue>();
    }

    // Новий публічний метод для взаємодії
    public void Interact(KeyCode key)
    {
        if (key == KeyCode.U && incomeComponent != null)
        {
            incomeComponent.TryUpgrade();
        }
        else if (key == KeyCode.T && dialogueComponent != null)
        {
            dialogueComponent.ToggleDialogue();
        }
    }

    // Новий публічний метод для отримання тексту
    public string GetUIText()
    {
        string upgradeText = "";
        string dialogueText = "";

        if (incomeComponent != null)
        {
            upgradeText = $"Покращити [U] (Ціна: {incomeComponent.upgradeCost:F0}$)\n";
        }
        if (dialogueComponent != null)
        {
            dialogueText = "Говорити [T]\n";
        }

        return (upgradeText + dialogueText).Trim();
    }
}