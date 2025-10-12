using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Key Settings")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode upgradeKey = KeyCode.U;
    public KeyCode chatKey = KeyCode.T;

    [Header("Interaction Distances")]
    public float interactionDistance = 3f; // Тепер одна загальна дистанція

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Filters")]
    public LayerMask interactableLayer;

    private GameObject closestInteractableObject = null;

    private void Update()
    {
        CheckForInteractables();
        HandleInput();
    }

    private void CheckForInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                // Головна зміна: перевіряємо, чи є на об'єкті ХОЧА Б ОДИН з потрібних нам скриптів
                if (col.GetComponent<PlatformController>() != null || col.GetComponent<CashRegister>() != null)
                {
                    closestDist = dist;
                    closest = col.gameObject;
                }
            }
        }

        closestInteractableObject = closest;
        UpdateInteractionText();
    }

    private void UpdateInteractionText()
    {
        if (interactionText == null) return;

        if (closestInteractableObject == null)
        {
            interactionText.text = "";
            return;
        }

        // --- ЛОГІКА ДЛЯ КОЖНОГО ТИПУ ОБ'ЄКТІВ ---

        PlatformController platform = closestInteractableObject.GetComponent<PlatformController>();
        if (platform != null)
        {
            interactionText.text = platform.GetUIText();
            return;
        }

        CashRegister cashRegister = closestInteractableObject.GetComponent<CashRegister>();
        if (cashRegister != null)
        {
            // Припускаємо, що у CashRegister теж є метод GetUIText()
            // interactionText.text = cashRegister.GetUIText();
            interactionText.text = "Використати касу [E]";
            return;
        }

        // Якщо додасте новий тип, доведеться писати нову перевірку тут...
    }

    private void HandleInput()
    {
        if (closestInteractableObject == null) return;

        // --- ОБРОБКА НАТИСКАНЬ ДЛЯ КОЖНОГО ТИПУ ОБ'ЄКТІВ ---

        PlatformController platform = closestInteractableObject.GetComponent<PlatformController>();
        if (platform != null)
        {
            if (Input.GetKeyDown(upgradeKey)) platform.Interact(upgradeKey);
            if (Input.GetKeyDown(chatKey)) platform.Interact(chatKey);
            return; // Виходимо, щоб не обробляти інші типи
        }

        CashRegister cashRegister = closestInteractableObject.GetComponent<CashRegister>();
        if (cashRegister != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                // Припускаємо, що у CashRegister є метод Interact()
                // cashRegister.Interact();
            }
            return;
        }

        // Якщо додасте новий тип, доведеться писати нову обробку клавіш тут...
    }
}