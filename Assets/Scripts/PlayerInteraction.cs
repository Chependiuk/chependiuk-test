using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerMovement playermovement;

    [Header("Key Settings")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode upgradeKey = KeyCode.U;
    public KeyCode chatKey = KeyCode.T;

    [Header("Interaction Distances")]
    public float defaultInteractionDistance = 3f;
    public float cashRegisterInteractionDistance = 1.5f;
    public float platformInteractionDistance = 2.5f;
    public float shopInteractionDistance = 2f;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("AI Dialogue")]
    public NPCDialogue dialogueNPCReference;

    [Header("Filters")]
    public LayerMask interactableLayer;

    private GameObject closestInteractableObject = null;

    private void Start()
    {
        playermovement = FindObjectOfType<PlayerMovement>();
        if (playermovement == null)
        {
            Debug.LogWarning("[DEBUG] PlayerMovement не знайдено!");
        }
    }

    private void Update()
    {
        CheckForInteractables();

        if (Input.GetKeyDown(interactKey))
        {
            HandleInteraction(interactKey);
        }

        if (Input.GetKeyDown(chatKey))
        {
            HandleChat();
        }

        if (Input.GetKeyDown(upgradeKey))
        {
            HandleUpgrade();
        }
    }

    private void CheckForInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);
        Debug.Log($"[DEBUG] Знайдено {hitColliders.Length} об’єктів у зоні {GetMaxInteractionDistance()}m");
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            float requiredDist = GetRequiredInteractionDistance(col.gameObject);
            NPCDialogue npc = col.gameObject.GetComponent<NPCDialogue>();
            bool hasNPC = npc != null;
            bool isEnabled = hasNPC ? npc.enabled : false;
            Debug.Log($"[DEBUG] Об’єкт: {col.gameObject.name}, Відстань: {dist:F1}m, Має NPCDialogue: {hasNPC}, Увімкнений: {isEnabled}, Шар: {LayerMask.LayerToName(col.gameObject.layer)}, Активний: {col.gameObject.activeInHierarchy}");

            if (dist <= requiredDist && dist < closestDist)
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        closestInteractableObject = closest;
        UpdateInteractionText(closest);
    }

    private float GetRequiredInteractionDistance(GameObject interactable)
    {
        if (interactable == null) return defaultInteractionDistance;

        if (interactable.GetComponent<NPCDialogue>() != null)
            return defaultInteractionDistance;

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
        if (interactionText == null) return;
        if (interactable == null)
        {
            interactionText.text = "";
            return;
        }

        float dist = Vector3.Distance(transform.position, interactable.transform.position);
        float requiredDist = GetRequiredInteractionDistance(interactable);

        NPCDialogue npc = interactable.GetComponent<NPCDialogue>();
        if (npc != null)
        {
            interactionText.text = $"Поговорити з {npc.CharacterName} [{chatKey}]\n(Distance: {dist:F1}m)";
            return;
        }

        CashRegister cashRegister = interactable.GetComponent<CashRegister>();
        if (cashRegister != null)
        {
            cashRegister.interactKey = interactKey;
            cashRegister.upgradeKey = upgradeKey;
            interactionText.text = $"{cashRegister.GetInteractionText()}\n(Distance: {dist:F1}m)\nЧат [{chatKey}]";
            return;
        }

        PlatformShopPoint shopPoint = interactable.GetComponent<PlatformShopPoint>();
        if (shopPoint != null)
        {
            interactionText.text = $"{shopPoint.GetInteractionText()}\n(Distance: {dist:F1}m)\nЧат [{chatKey}]";
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
                                     $"Distance: {dist:F1}m\n" +
                                     $"Чат [{chatKey}]";
            }
            else
            {
                interactionText.text = $"Come closer to upgrade\n" +
                                     $"Required: ≤{requiredDist:F1}m\n" +
                                     $"Current: {dist:F1}m\n" +
                                     $"Чат [{chatKey}]";
            }
            return;
        }

        interactionText.text = "";
    }

    private void HandleInteraction(KeyCode key)
    {
        if (closestInteractableObject == null) return;

        float dist = Vector3.Distance(transform.position, closestInteractableObject.transform.position);
        float requiredDist = GetRequiredInteractionDistance(closestInteractableObject);

        if (dist > requiredDist) return;

        if (key == interactKey)
        {
            CashRegister cashRegister = closestInteractableObject.GetComponent<CashRegister>();
            if (cashRegister != null)
            {
                cashRegister.Interact();
                return;
            }

            PlatformShopPoint shopPoint = closestInteractableObject.GetComponent<PlatformShopPoint>();
            if (shopPoint != null)
            {
                shopPoint.Interact();
                return;
            }
        }
    }

    private void HandleChat()
    {
        if (closestInteractableObject == null) return;

        float dist = Vector3.Distance(transform.position, closestInteractableObject.transform.position);
        float requiredDist = GetRequiredInteractionDistance(closestInteractableObject);

        if (dist > requiredDist) return;

        NPCDialogue npc = closestInteractableObject.GetComponent<NPCDialogue>();
        if (npc != null)
        {
            Debug.Log($"[DEBUG] Викликаю TryActivateDialogue для {closestInteractableObject.name}");
            npc.TryActivateDialogue();
        }
        else
        {
            Debug.Log($"[DEBUG] Чат активовано з {closestInteractableObject.name}, але NPCDialogue не знайдено");
        }
    }

    private void HandleUpgrade()
    {
        if (closestInteractableObject == null) return;

        float dist = Vector3.Distance(transform.position, closestInteractableObject.transform.position);
        float requiredDist = GetRequiredInteractionDistance(closestInteractableObject);

        if (dist > requiredDist) return;

        CashRegister cashRegister = closestInteractableObject.GetComponent<CashRegister>();
        if (cashRegister != null)
        {
            cashRegister.TryUpgrade();
            return;
        }

        IncomePlatform incomePlatform = closestInteractableObject.GetComponent<IncomePlatform>();
        if (incomePlatform != null)
        {
            incomePlatform.TryUpgrade();
            return;
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