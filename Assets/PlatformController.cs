using UnityEngine;

public class PlatformController : MonoBehaviour, IInteractable
{
    private IncomePlatform incomeComponent;
    private PlatformDialogue dialogueComponent;

    private void Awake()
    {
        incomeComponent = GetComponent<IncomePlatform>();
        dialogueComponent = GetComponent<PlatformDialogue>();
    }

    public void HandleInteraction(KeyCode key)
    {
        // 'E' тепер тільки намагається відкрити діалог, якщо він ще не відкритий.
        if (key == KeyCode.E && dialogueComponent != null && !dialogueComponent.IsDialogueOpen())
        {
            dialogueComponent.ToggleDialogue();
        }

        // 'U' працює як і раніше, для покращення.
        if (key == KeyCode.U && incomeComponent != null)
        {
            incomeComponent.TryUpgrade();
        }
    }

    public InteractionType GetActiveInteractionType(KeyCode key)
    {
        // Показуємо підказку для діалогу, тільки якщо він ще не відкритий.
        if (key == KeyCode.E && dialogueComponent != null && !dialogueComponent.IsDialogueOpen())
        {
            return InteractionType.General;
        }

        if (key == KeyCode.U && incomeComponent != null)
        {
            return InteractionType.Upgrade;
        }

        return InteractionType.None;
    }

    public string GetInteractionText(KeyCode key, float dist, float requiredDist)
    {
        // Збираємо підказку з усіх можливих дій.
        // PlayerInteraction сам збере цей текст до купи.
        if (key == KeyCode.U && incomeComponent != null)
        {
            return $"Покращити [U] (Ціна: {incomeComponent.upgradeCost:F0}$)";
        }

        if (key == KeyCode.E && dialogueComponent != null && !dialogueComponent.IsDialogueOpen())
        {
            return "Говорити [E]";
        }

        return "";
    }
}