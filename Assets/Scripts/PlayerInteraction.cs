using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    // --- ЗМІНЕНО: Тепер у нас лише одна клавіша для всіх дій ---
    [Header("Налаштування")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 4f;
    public TextMeshProUGUI interactionText;
    public LayerMask interactableLayer;

    private IInteractable closestInteractable;

    private void Update()
    {
        CheckForInteractables();
        HandleInput();
    }

    private void CheckForInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);
        closestInteractable = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in hitColliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestInteractable = interactable;
            }
        }
        UpdateInteractionText();
    }

    private void UpdateInteractionText()
    {
        if (interactionText == null) return;

        if (closestInteractable == null)
        {
            interactionText.text = "";
            return;
        }

        // --- ЗМІНЕНО: Отримуємо текст лише для однієї клавіші ---
        interactionText.text = closestInteractable.GetInteractionText(interactKey, 0, 0);
    }

    private void HandleInput()
    {
        if (closestInteractable == null) return;

        // --- ЗМІНЕНО: Передаємо команду лише по одній клавіші ---
        if (Input.GetKeyDown(interactKey))
        {
            closestInteractable.HandleInteraction(interactKey);
        }
    }
}