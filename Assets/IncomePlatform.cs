using UnityEngine;
using TMPro;

public class IncomePlatform : MonoBehaviour
{

    [Header("Налаштування")]
    public int level = 1;
    public float incomeAmount = 10f;
    public float incomeInterval = 5f;
    public float upgradeCost = 0f;
    [Tooltip("Це значення тепер синхронізується з PlayerInteraction")]
    public float upgradeDistance = 2.5f; // Має співпадати з platformInteractionDistance

    [Header("Візуальні елементи")]
    public TextMeshPro levelText;
    public TextMeshPro incomeText;
    public GameObject upgradeEffect;

    private float timer;
    private GameManager gameManager;
    private PlayerInteraction playerInteraction;
   
    private void Start()
    {
        gameManager = GameManager.Instance;
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            upgradeDistance = playerInteraction.platformInteractionDistance;
        }
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
        gameManager.AddMoney(incomeAmount);

        if (incomeText != null)
        {
            var incomePopup = Instantiate(incomeText, transform.position + Vector3.up * 2f, Quaternion.identity);
            incomePopup.text = $"+{incomeAmount}$";
            Destroy(incomePopup.gameObject, 1f);
        }
    }

    public void TryUpgrade()
    {
        // Перевірка грошей тепер без зайвих перевірок дистанції
        if (gameManager.TrySpendMoney(upgradeCost))
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

    private void UpdateUI()
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
}