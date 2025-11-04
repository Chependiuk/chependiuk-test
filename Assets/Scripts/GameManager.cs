using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float currentMoney = 1000f; // Початкова кількість грошей
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    /// <summary>
    /// Намагається списати гроші. Повертає true, якщо успішно, і false, якщо ні.
    /// </summary>
    public bool TrySpendMoney(float amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            Debug.Log($"Витрачено {amount}$. Залишилось: {currentMoney}$");
            return true;
        }
        else
        {
            Debug.LogWarning($"Недостатньо грошей! Потрібно: {amount}$, є: {currentMoney}$");
            return false;
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {currentMoney:F0}$";
        }
    }
}