using UnityEngine;
using TMPro;

// !!! ВАЖЛИВО: IncomePlatform тепер реалізує IInteractable !!!
// Вам потрібно створити окремий файл IInteractable.cs
public class IncomePlatform : MonoBehaviour, IInteractable
{
    [Header("Налаштування")]
    public int level = 1;
    public float incomeAmount = 10f;
    public float incomeInterval = 5f;
    public float upgradeCost = 0f;

    [Header("Візуальні елементи")]
    public TextMeshPro levelText;
    public TextMeshPro incomeText;
    public GameObject upgradeEffect;

    private float timer;
    private GameManager gameManager;
    private PlayerInteraction playerInteraction;

    private void Start()
    {
        // ВАЖЛИВО: Оскільки у цьому коді немає посилання на PlayerInteraction, 
        // я залишаю FindObjectOfType<PlayerInteraction>() закоментованим,
        // щоб не викликати помилки, якщо він вам не потрібен
        // playerInteraction = FindObjectOfType<PlayerInteraction>();

        // Припускаємо, що GameManager існує
        gameManager = GameManager.Instance;

        UpdateUI();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= incomeInterval)
        {
            GenerateIncome();
            timer = 0f;
        }
    }

    private void GenerateIncome()
    {
        if (gameManager != null)
        {
            gameManager.AddMoney(incomeAmount);
        }

        if (incomeText != null)
        {
            var incomePopup = Instantiate(incomeText, transform.position + Vector3.up * 2f, Quaternion.identity);
            incomePopup.text = $"+{incomeAmount}$";
            Destroy(incomePopup.gameObject, 1f);
        }
    }

    public void TryUpgrade()
    {
        if (gameManager != null && gameManager.TrySpendMoney(upgradeCost))
        {
            UpgradePlatform();
        }
    }

    private void UpgradePlatform()
    {
        level++;
        incomeAmount *= 1.5f;
        upgradeCost *= 1.8f;

        if (upgradeEffect != null)
        {
            Instantiate(upgradeEffect, transform.position, Quaternion.identity);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }

        if (incomeText != null)
        {
            incomeText.text = $"{incomeAmount}$ / {incomeInterval}s";
        }
    }

    // ======================================================
    // МЕТОДИ IINTERACTABLE (НОВА ЛОГІКА ВЗАЄМОДІЇ)
    // ======================================================

    /// <summary>
    /// Обробляє натискання клавіші гравцем (викликається PlayerInteraction)
    /// </summary>
    public void HandleInteraction(KeyCode key)
    {
        // Цей скрипт обробляє лише Upgrade, оскільки це платформа доходу
        if (key == KeyCode.U)
        {
            TryUpgrade();
        }
        // Можна додати логіку чату, якщо PlatformDialogue знаходиться на цьому ж об'єкті:
        // if (key == KeyCode.T && GetComponent<PlatformDialogue>() != null) { /* викликати чат */ }
    }

    /// <summary>
    /// Повертає пріоритетний тип взаємодії для клавіші (для UI гравця)
    /// </summary>
    public InteractionType GetActiveInteractionType(KeyCode key)
    {
        // Якщо натиснуто U, ми завжди пропонуємо апгрейд
        if (key == KeyCode.U)
        {
            return InteractionType.Upgrade;
        }
        // Якщо натиснуто T, ми перевіряємо, чи є на об'єкті компонент чату
        if (key == KeyCode.T && GetComponent<PlatformDialogue>() != null)
        {
            return InteractionType.Chat;
        }
        return InteractionType.None;
    }

    /// <summary>
    /// Повертає текст для UI гравця
    /// </summary>
    public string GetInteractionText(KeyCode key, float dist, float requiredDist)
    {
        // Текст для апгрейду
        if (key == KeyCode.U)
        {
            if (dist <= requiredDist)
            {
                return $"Point upgrade [U]\nPrice: {upgradeCost}$\n" +
                       $"Current level: {level}\n" +
                       $"Current income: {incomeAmount}$\n" +
                       $"Distance: {dist:F1}m";
            }
            return $"Come closer to upgrade\nRequired: ≤{requiredDist:F1}m\nCurrent: {dist:F1}m";
        }

        // Текст для чату (якщо об'єкт його підтримує)
        if (key == KeyCode.T && GetComponent<PlatformDialogue>() != null)
        {
            return $"Поговорити [T] (На платформі)";
        }

        return "Взаємодія";
    }
}