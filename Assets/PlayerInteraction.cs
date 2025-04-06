using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerMovement playermovement;
    [Header("Key Settings")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode upgradeKey = KeyCode.U;

    [Header("Interaction Distances")]
    public float defaultInteractionDistance = 3f;
    public float cashRegisterInteractionDistance = 1.5f;
    public float platformInteractionDistance = 2.5f;
    public float shopInteractionDistance = 2f;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Filters")]
    public LayerMask interactableLayer;

    private void Start()
    {
        playermovement = FindObjectOfType<PlayerMovement>();
        
    }
    private void Update()
    {
        CheckForInteractables();

        if (Input.GetKeyDown(interactKey))
        {
            HandleInteraction();
        }

        if (Input.GetKeyDown(upgradeKey))
        {
            if (playermovement != null)
            {
                playermovement.is_upgrade_anim = true;
            }
            HandleUpgrade();
        }
        else
        {
            playermovement.is_upgrade_anim = false;
        }
    }

    private void CheckForInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            float requiredDist = GetRequiredInteractionDistance(col.gameObject);

            if (dist <= requiredDist && dist < closestDist)
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        UpdateInteractionText(closest);
    }

    private float GetRequiredInteractionDistance(GameObject interactable)
    {
        if (interactable.GetComponent<CashRegister>() != null)
            return cashRegisterInteractionDistance;

        if (interactable.GetComponent<IncomePlatform>() != null)
            return platformInteractionDistance;

        if (interactable.GetComponent<PlatformShopPoint>() != null)
            return shopInteractionDistance;

        return defaultInteractionDistance;
    }

    private float GetMaxInteractionDistance()
    {
        return Mathf.Max(defaultInteractionDistance,
                       cashRegisterInteractionDistance,
                       platformInteractionDistance,
                       shopInteractionDistance);
    }

    private void UpdateInteractionText(GameObject interactable)
    {
        if (interactionText == null)
        {
            return;
        }

        if (interactable == null)
        {
            interactionText.text = "";
            return;
        }

        float dist = Vector3.Distance(transform.position, interactable.transform.position);
        float requiredDist = GetRequiredInteractionDistance(interactable);

        CashRegister cashRegister = interactable.GetComponent<CashRegister>();
        if (cashRegister != null)
        {
            cashRegister.interactKey = interactKey;
            cashRegister.upgradeKey = upgradeKey;
            interactionText.text = $"{cashRegister.GetInteractionText()}\n(Distance: {dist:F1}m)";
            return;
        }

        PlatformShopPoint shopPoint = interactable.GetComponent<PlatformShopPoint>();
        if (shopPoint != null)
        {
            interactionText.text = $"{shopPoint.GetInteractionText()}\n(Distance: {dist:F1}m)";
            return;
        }

        IncomePlatform incomePlatform = interactable.GetComponent<IncomePlatform>();
        if (incomePlatform != null)
        {
            if (dist <= requiredDist)
            {

                interactionText.text = $"Point upgrade [{upgradeKey}]\n" +
                                     $"Price: {incomePlatform.upgradeCost}$\n" +
                                     $"Current level: {incomePlatform.level}\n" +
                                     $"Current income: {incomePlatform.incomeAmount}$\n" +
                                     $"Distance: {dist:F1}m";
            }
            else
            {
                
                interactionText.text = $"Come closer to upgrade\n" +
                                     $"Required: ≤{requiredDist:F1}m\n" +
                                     $"Current: {dist:F1}m";
            }
            return;
        }

        interactionText.text = "";
    }

    private void HandleInteraction()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);
        GameObject closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in interactables)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            float requiredDist = GetRequiredInteractionDistance(col.gameObject);

            if (dist <= requiredDist && dist < closestDistance)
            {
                closestDistance = dist;
                closestInteractable = col.gameObject;
            }
        }

        if (closestInteractable != null)
        {
            CashRegister cashRegister = closestInteractable.GetComponent<CashRegister>();
            if (cashRegister != null)
            {
                cashRegister.Interact();
                return;
            }

            PlatformShopPoint shopPoint = closestInteractable.GetComponent<PlatformShopPoint>();
            if (shopPoint != null)
            {
                shopPoint.Interact();
                return;
            }
        }
    }

    private void HandleUpgrade()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);
        GameObject closestUpgradable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in interactables)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            float requiredDist = GetRequiredInteractionDistance(col.gameObject);

            if (dist <= requiredDist && dist < closestDistance)
            {
                closestDistance = dist;
                closestUpgradable = col.gameObject;
            }
        }

        if (closestUpgradable != null)
        {
            CashRegister cashRegister = closestUpgradable.GetComponent<CashRegister>();
            if (cashRegister != null)
            {
                cashRegister.TryUpgrade();
                return;
            }

            IncomePlatform incomePlatform = closestUpgradable.GetComponent<IncomePlatform>();
            if (incomePlatform != null)
            {
                incomePlatform.TryUpgrade();
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, defaultInteractionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, cashRegisterInteractionDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, platformInteractionDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, shopInteractionDistance);
    }
}