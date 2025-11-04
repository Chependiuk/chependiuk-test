using UnityEngine;
using TMPro;

// Зверніть увагу: ", IInteractable" видалено
public class IncomePlatform : MonoBehaviour
{
    [Header("Налаштування")]
    public int level = 1;
    public float incomeAmount = 10f;
    public float incomeInterval = 5f;
    public float upgradeCost = 50f;

    [Header("Візуальні елементи")]
    public TextMeshPro levelText;
    public GameObject upgradeEffect;

    private float timer;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(incomeAmount);
        }
    }

    // Цей метод тепер викликається з PlatformController
    public void TryUpgrade()
    {
        if (GameManager.Instance != null && GameManager.Instance.TrySpendMoney(upgradeCost))
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

        if (levelText != null)
        {
            levelText.text = $"Рівень {level}";
        }
    }
}