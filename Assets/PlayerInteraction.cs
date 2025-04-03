using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Налаштування клавіш")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode upgradeKey = KeyCode.U;

    [Header("Відстані взаємодії")]
    public float defaultInteractionDistance = 3f; // Для звичайних об'єктів
    public float cashRegisterInteractionDistance = 1.5f; // Для каси
    public float platformInteractionDistance = 2.5f; // Для платформ
    public float shopInteractionDistance = 2f; // Для магазинів

    [Header("Інтерфейс")]
    public TextMeshProUGUI interactionText;

    [Header("Фільтри")]
    public LayerMask interactableLayer;

    private void Update()
    {
        CheckForInteractables();
        HandleInteraction();
        HandleUpgrade();
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

            if (dist < closestDist && dist <= requiredDist)
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
        float maxDistance = defaultInteractionDistance;
        maxDistance = Mathf.Max(maxDistance, cashRegisterInteractionDistance);
        maxDistance = Mathf.Max(maxDistance, platformInteractionDistance);
        maxDistance = Mathf.Max(maxDistance, shopInteractionDistance);
        return maxDistance;
    }

    private void UpdateInteractionText(GameObject interactable)
    {
        if (interactionText == null) return;

        if (interactable != null)
        {
            float dist = Vector3.Distance(transform.position, interactable.transform.position);
            float requiredDist = GetRequiredInteractionDistance(interactable);

            var cashRegister = interactable.GetComponent<CashRegister>();
            if (cashRegister != null)
            {
                cashRegister.interactKey = interactKey;
                cashRegister.upgradeKey = upgradeKey;
                interactionText.text = $"{cashRegister.GetInteractionText()}\n(Дистанція: {dist:F1}m";
                return;
            }

            var shopPoint = interactable.GetComponent<PlatformShopPoint>();
            if (shopPoint != null)
            {
                interactionText.text = $"{shopPoint.GetInteractionText()}\n(Дистанція: {dist:F1}m";
                return;
            }

            var incomePlatform = interactable.GetComponent<IncomePlatform>();
            if (incomePlatform != null)
            {
                if (dist <= requiredDist)
                {
                    interactionText.text = $"Апгрейд платформи [{upgradeKey}]\n" +
                                         $"Рівень: {incomePlatform.level}\n" +
                                         $"Дохід: {incomePlatform.incomeAmount}$\n" +
                                         $"Ціна: {incomePlatform.upgradeCost}$\n" +
                                         $"Дистанція: {dist:F1}m";
                }
                else
                {
                    interactionText.text = $"Підійдіть ближче для апгрейду\n" +
                                         $"Потрібно: ≤{requiredDist:F1}m\n" +
                                         $"Зараз: {dist:F1}m";
                }
                return;
            }
        }

        interactionText.text = "";
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);

            foreach (Collider col in interactables)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                float requiredDist = GetRequiredInteractionDistance(col.gameObject);

                if (dist > requiredDist) continue;

                var cashRegister = col.GetComponent<CashRegister>();
                if (cashRegister != null)
                {
                    cashRegister.Interact();
                    return;
                }

                var shopPoint = col.GetComponent<PlatformShopPoint>();
                if (shopPoint != null)
                {
                    shopPoint.Interact();
                    return;
                }
            }
        }
    }

    private void HandleUpgrade()
    {
        if (Input.GetKeyDown(upgradeKey))
        {
            Collider[] interactables = Physics.OverlapSphere(transform.position, GetMaxInteractionDistance(), interactableLayer);

            foreach (Collider col in interactables)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                float requiredDist = GetRequiredInteractionDistance(col.gameObject);

                if (dist > requiredDist) continue;

                var cashRegister = col.GetComponent<CashRegister>();
                if (cashRegister != null)
                {
                    cashRegister.TryUpgrade();
                    return;
                }

                var incomePlatform = col.GetComponent<IncomePlatform>();
                if (incomePlatform != null)
                {
                    incomePlatform.TryUpgrade();
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Відображення зон взаємодії
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