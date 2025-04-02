using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Налаштування клавіш")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode upgradeKey = KeyCode.U;

    [Header("Відстані взаємодії")]
    public float defaultInteractionDistance = 3f;  // Для магазинів, каси тощо
    public float platformUpgradeMultiplier = 1f;   // Множник відстані для платформ

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

            if (dist < closestDist && dist <= requiredDist &&
                (col.GetComponent<PlatformShopPoint>() != null ||
                 col.GetComponent<CashRegister>() != null ||
                 col.GetComponent<IncomePlatform>() != null))
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        UpdateInteractionText(closest);
    }

    private float GetMaxInteractionDistance()
    {
        // Повертає найбільшу з усіх можливих дистанцій
        return Mathf.Max(defaultInteractionDistance, defaultInteractionDistance * platformUpgradeMultiplier);
    }

    private float GetRequiredInteractionDistance(GameObject interactable)
    {
        var incomePlatform = interactable.GetComponent<IncomePlatform>();
        if (incomePlatform != null)
        {
            // Використовуємо множник для платформ
            return incomePlatform.upgradeDistance * platformUpgradeMultiplier;
        }

        // Для інших об'єктів - стандартна дистанція
        return defaultInteractionDistance;
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
                interactionText.text = $"{cashRegister.GetInteractionText()}\n(Дистанція: {dist:F1}m)";
                return;
            }

            var shopPoint = interactable.GetComponent<PlatformShopPoint>();
            if (shopPoint != null)
            {
                interactionText.text = $"{shopPoint.GetInteractionText()}\n(Дистанція: {dist:F1}m)";
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
                var incomePlatform = col.GetComponent<IncomePlatform>();
                if (incomePlatform != null)
                {
                    float dist = Vector3.Distance(transform.position, incomePlatform.transform.position);
                    float requiredDist = incomePlatform.upgradeDistance * platformUpgradeMultiplier;

                    if (dist <= requiredDist)
                    {
                        incomePlatform.TryUpgrade();
                    }
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetMaxInteractionDistance());
    }
}