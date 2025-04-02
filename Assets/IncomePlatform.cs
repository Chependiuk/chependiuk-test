using UnityEngine;
using TMPro;

public class IncomePlatform : MonoBehaviour
{
    [Header("������������")]
    public int level = 1;
    public float incomeAmount = 10f;
    public float incomeInterval = 5f;
    public float upgradeCost = 0f;
    public float upgradeDistance = 3f;

    [Header("�������� ��������� �� ������")]
    public PlatformElement[] platformElements;

    [Header("³������� ��������")]
    public TextMeshPro levelText;
    public TextMeshPro incomeText;
    public GameObject upgradeEffect;

    private float timer;
    private GameManager gameManager;
    private Transform playerTransform;

    [System.Serializable]
    public class PlatformElement
    {
        public GameObject elementPrefab; // ������ ��������
        public Vector3 spawnPosition; // ������� ������� ���������
        public int appearLevel; // �� ����� ���� �'���������
        public bool persists; // �� ���������� �� ��������� �����
        [HideInInspector] public GameObject spawnedInstance; // ��������� ���������
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateUI();
        SpawnPlatformElements();
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

    public void TryUpgrade() // ����������� � PlayerInteraction �� ������
    {
        // ���������� ��������� �� ������
        if (Vector3.Distance(transform.position, playerTransform.position) > upgradeDistance)
        {
            Debug.Log("������� ����� ������ ��� ��������");
            return;
        }

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
        UpdatePlatformElements();
    }

    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = $"г���� {level}";
        }

        if (incomeText != null)
        {
            incomeText.text = $"{incomeAmount}$ / {incomeInterval}�";
        }
    }

    private void SpawnPlatformElements()
    {
        if (platformElements == null || platformElements.Length == 0) return;

        foreach (var element in platformElements)
        {
            if (element.elementPrefab != null && level >= element.appearLevel)
            {
                element.spawnedInstance = Instantiate(
                    element.elementPrefab,
                    transform.position + element.spawnPosition,
                    Quaternion.identity,
                    transform // ������ ��������� ����������� ��'�����
                );
            }
        }
    }

    private void UpdatePlatformElements()
    {
        if (platformElements == null || platformElements.Length == 0) return;

        foreach (var element in platformElements)
        {
            if (element.elementPrefab == null) continue;

            // ���� ������� �� �� ��������� � ������ ���� �����
            if (element.spawnedInstance == null && level >= element.appearLevel)
            {
                element.spawnedInstance = Instantiate(
                    element.elementPrefab,
                    transform.position + element.spawnPosition,
                    Quaternion.identity,
                    transform
                );
            }
            // ���� ������� ���� � �� ������� ���������� �� ��������� �����
            else if (element.spawnedInstance != null && !element.persists && level > element.appearLevel)
            {
                Destroy(element.spawnedInstance);
                element.spawnedInstance = null;
            }
        }
    }
}