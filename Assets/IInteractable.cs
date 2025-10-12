using UnityEngine;

// Визначаємо можливі дії
public enum InteractionType { None, Chat, Upgrade, General }

// ЦЕ ЛИШЕ ШАБЛОН (ІНТЕРФЕЙС), А НЕ СКРИПТ!
public interface IInteractable
{
    // Обробка натискання клавіші
    void HandleInteraction(KeyCode key);

    // Повертає тип взаємодії для UI
    InteractionType GetActiveInteractionType(KeyCode key);

    // Повертає текст для UI
    string GetInteractionText(KeyCode key, float dist, float requiredDist);
}