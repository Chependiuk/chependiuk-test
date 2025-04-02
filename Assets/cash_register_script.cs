using UnityEngine;
using System.Collections;
using TMPro;

public class CashRegister : MonoBehaviour
{
    [Header("������������")]
    public float refillSpeed = 20f; // �����/������� ��� ������������� ����������
    public float maxMoneyCapacity = 500f;

    [Header("³���������")]
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
        currentMoney = maxMoneyCapacity; // �������� � ����� ����
        UpdateVisuals();
    }

    private void Update()
    {
        // ����������� ����������
        if (currentMoney < maxMoneyCapacity && !isRefilling)
        {
            StartCoroutine(RefillCoroutine());
        }
    }

    public string GetInteractionText()
    {
        float fillPercent = (currentMoney / maxMoneyCapacity) * 100f;
        return $"������� �� {currentMoney:F0}$ [E]\n����������: {fillPercent:F0}%";
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
        // ��������� 3D �����
        float fillRatio = currentMoney / maxMoneyCapacity;

        // ��������� ��� ���� ��������� ���������
        float minHeight = originalMoneyScale.y * 0f;  // ̳������� ������ (10% �� ��������)
        float maxHeight = originalMoneyScale.y * 30f;    // ����������� ������ (500% �� ��������)

        // ������ ������� ��������� ��� ������� ������
        float animatedFillRatio = Mathf.Pow(fillRatio, 0.7f); // ������������� ���������

        moneyVisual.localScale = new Vector3(
            originalMoneyScale.x,
            Mathf.Lerp(minHeight, maxHeight, animatedFillRatio),
            originalMoneyScale.z
        );

        // ��������� ������
        float fillPercent = fillRatio * 100f;
        fillStatusText.text = $"����: {fillPercent:F0}%";
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

    // ���������� ��� �������� ��� ���������� ����
    public void AddMoneyToRegister(float amount)
    {
        currentMoney = Mathf.Min(currentMoney + amount, maxMoneyCapacity);
        UpdateVisuals();
    }
}