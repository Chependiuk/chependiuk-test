using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private PlatformShopPoint currentInteractable;

    private void Update()
    {
        FindNearestInteractable();
        HandleInteractionInput();
    }

    private void FindNearestInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance, interactableLayer);
        PlatformShopPoint nearest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            var interactable = col.GetComponent<PlatformShopPoint>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearest = interactable;
                }
            }
        }

        UpdateInteractionUI(nearest);
        currentInteractable = nearest;
    }

    private void UpdateInteractionUI(PlatformShopPoint interactable)
    {
        if (interactionText == null) return;

        interactionText.text = interactable != null ? interactable.GetInteractionText() : "";
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
            interactionText.text = "";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}