using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Налаштування")]
    public TextMeshProUGUI interactionText;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private void Update()
    {
        CheckForInteractables();
        HandleInteraction();
    }

    private void CheckForInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist && (col.GetComponent<PlatformShopPoint>() != null || col.GetComponent<CashRegister>() != null))
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        UpdateInteractionText(closest);
    }

    private void UpdateInteractionText(GameObject interactable)
    {
        if (interactionText == null) return;

        if (interactable != null)
        {
            // Перевірка каси
            var cashRegister = interactable.GetComponent<CashRegister>();
            if (cashRegister != null)
            {
                interactionText.text = cashRegister.GetInteractionText();
                return;
            }

            // Перевірка магазину
            var shopPoint = interactable.GetComponent<PlatformShopPoint>();
            if (shopPoint != null)
            {
                interactionText.text = shopPoint.GetInteractionText();
                return;
            }
        }

        interactionText.text = "";
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);

            foreach (Collider col in interactables)
            {
                // Пріоритет касі
                var cashRegister = col.GetComponent<CashRegister>();
                if (cashRegister != null)
                {
                    cashRegister.Interact();
                    return;
                }

                // Потім магазин
                var shopPoint = col.GetComponent<PlatformShopPoint>();
                if (shopPoint != null)
                {
                    shopPoint.Interact();
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}