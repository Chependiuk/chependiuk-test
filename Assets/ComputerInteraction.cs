using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    public GameObject computerCanvas;

    public void OpenComputer()
    {
        if (computerCanvas == null) return;
        computerCanvas.SetActive(true);
        PlayerMovement.Instance.SetMovementState(false); // Вимикаємо рух
    }

    public void CloseComputer()
    {
        if (computerCanvas == null) return;
        computerCanvas.SetActive(false);
        PlayerMovement.Instance.SetMovementState(true); // Вмикаємо рух
    }
    // --- Методи IInteractable ---
    public void HandleInteraction(KeyCode key) { if (key == KeyCode.E) OpenComputer(); }
    public InteractionType GetActiveInteractionType(KeyCode key) { return (key == KeyCode.E) ? InteractionType.General : InteractionType.None; }
    public string GetInteractionText(KeyCode key, float dist, float requiredDist) { if (dist <= requiredDist) return "Використовувати комп'ютер [E]"; return ""; }
}