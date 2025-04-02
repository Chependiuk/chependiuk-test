using UnityEngine;
using System.Collections;
using TMPro;

public class CashRegister : MonoBehaviour
{
    [Header("Налаштування")]
    public float refillSpeed = 20f; // Гроші/секунду для автоматичного поповнення
    public float maxMoneyCapacity = 500f;

    [Header("Візуалізація")]
    public ParticleSystem moneyEffect;
    public AudioClip moneySound;
    public Transform moneyVisual;
    public TextMeshProUGUI fillStatusText;

    private AudioSource audioSource;
    private Vector3 originalMoneyScale;
    private float currentMoney;
    private bool isRefilling;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        originalMoneyScale = moneyVisual.localScale;
        currentMoney = maxMoneyCapacity; // Починаємо з повної каси
        UpdateVisuals();
    }

    private void Update()
    {
        // Автоматичне поповнення
        if (currentMoney < maxMoneyCapacity && !isRefilling)
        {
            StartCoroutine(RefillCoroutine());
        }
    }

    public string GetInteractionText()
    {
        float fillPercent = (currentMoney / maxMoneyCapacity) * 100f;
        return $"Забрати всі {currentMoney:F0}$ [E]\nЗаповнення: {fillPercent:F0}%";
    }

    public void Interact()
    {
        if (currentMoney <= 0) return;

        float amountToGive = currentMoney;
        GameManager.Instance.AddMoney(amountToGive);
        currentMoney = 0f;

        PlayEffects();
        UpdateVisuals();
    }

    private IEnumerator RefillCoroutine()
    {
        isRefilling = true;

        while (currentMoney < maxMoneyCapacity)
        {
            float refillAmount = refillSpeed * Time.deltaTime;
            currentMoney = Mathf.Min(currentMoney + refillAmount, maxMoneyCapacity);
            UpdateVisuals();
            yield return null;
        }

        isRefilling = false;
    }

    private void UpdateVisuals()
    {
        // Оновлення 3D моделі
        float fillRatio = currentMoney / maxMoneyCapacity;

        // Параметри для більш виразного зростання
        float minHeight = originalMoneyScale.y * 0f;  // Мінімальна висота (10% від оригіналу)
        float maxHeight = originalMoneyScale.y * 30f;    // Максимальна висота (500% від оригіналу)

        // Додаємо нелінійне зростання для більшого ефекту
        float animatedFillRatio = Mathf.Pow(fillRatio, 0.7f); // Експоненційне зростання

        moneyVisual.localScale = new Vector3(
            originalMoneyScale.x,
            Mathf.Lerp(minHeight, maxHeight, animatedFillRatio),
            originalMoneyScale.z
        );

        // Оновлення тексту
        float fillPercent = fillRatio * 100f;
        fillStatusText.text = $"Каса: {fillPercent:F0}%";
        fillStatusText.color = Color.Lerp(Color.red, Color.green, fillRatio);
    }

    private void PlayEffects()
    {
        if (moneyEffect != null)
        {
            Instantiate(moneyEffect, transform.position, Quaternion.identity);
        }

        if (moneySound != null)
        {
            audioSource.PlayOneShot(moneySound);
        }
    }

    // Викликайте при покупках для поповнення каси
    public void AddMoneyToRegister(float amount)
    {
        currentMoney = Mathf.Min(currentMoney + amount, maxMoneyCapacity);
        UpdateVisuals();
    }
}