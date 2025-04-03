using UnityEngine;
using TMPro;

public class CashRegister : MonoBehaviour
{
    [Header("������������")]
    public float moneyLimit = 500f;
    public float upgradeCost = 1000f;
    public float upgradeMultiplier = 1.5f;
    public float interactionDistance = 2f;
    public float incomeInterval = 10f;
    public float incomeAmount = 25f;

    [Header("����� ��������")]
    public GameObject upgradeEffect;
    public float maxEffectHeight = 3f;
    public float minEffectHeight = 0.1f;
    public float effectAnimationSpeed = 5f;



    private float currentMoney;
    private int upgradeLevel = 1;
    private GameManager gameManager;
    private float timer;
    private Vector3 originalEffectScale;
    private bool isUpgrading;
    private float upgradeAnimationTimer;

    [HideInInspector] public KeyCode interactKey;
    [HideInInspector] public KeyCode upgradeKey;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (upgradeEffect != null)
        {
            originalEffectScale = upgradeEffect.transform.localScale;
            upgradeEffect.SetActive(false);
        }

      
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= incomeInterval)
        {
            AddMoney(incomeAmount);
            timer = 0f;
        }

        UpdateEffectHeight();
        HandleUpgradeAnimation();
    }

    void UpdateEffectHeight()
    {
        if (upgradeEffect == null || !upgradeEffect.activeSelf || isUpgrading) return;

        float fillPercent = currentMoney / moneyLimit;
        float targetHeight = Mathf.Lerp(minEffectHeight, maxEffectHeight, fillPercent);

        Vector3 currentScale = upgradeEffect.transform.localScale;
        float newHeight = Mathf.Lerp(currentScale.y, originalEffectScale.y * targetHeight, Time.deltaTime * effectAnimationSpeed);

        upgradeEffect.transform.localScale = new Vector3(
            originalEffectScale.x,
            newHeight,
            originalEffectScale.z
        );
    }

    void HandleUpgradeAnimation()
    {
        if (!isUpgrading || upgradeEffect == null) return;

        upgradeAnimationTimer += Time.deltaTime;
        float pingPong = Mathf.PingPong(upgradeAnimationTimer * 10f, 1f);
        float heightMultiplier = 1f + pingPong * 0.5f; // ��������� �� 1.0 � 1.5

        upgradeEffect.transform.localScale = new Vector3(
            originalEffectScale.x,
            originalEffectScale.y * maxEffectHeight * heightMultiplier,
            originalEffectScale.z
        );

        if (upgradeAnimationTimer > 0.5f) // ��������� �������
        {
            isUpgrading = false;
            upgradeAnimationTimer = 0f;
        }
    }

    public string GetInteractionText()
    {
        return $"���� [{interactKey}]\n" +
               $"������: {currentMoney}/{moneyLimit}$ ({(currentMoney / moneyLimit) * 100:F0}%)\n" +
               $"�����: {incomeAmount}$/{incomeInterval}�\n" +
               $"������� [{upgradeKey}]: {upgradeCost}$\n" +
               $"г����: {upgradeLevel}";
    }

    public void Interact()
    {
        if (currentMoney > 0)
        {
            gameManager.AddMoney(currentMoney);
            currentMoney = 0;

            if (upgradeEffect != null)
            {
                upgradeEffect.SetActive(false);
            }

          
        }
    }

    public void TryUpgrade()
    {
        if (gameManager.TrySpendMoney(upgradeCost))
        {
            Upgrade();
        }
    }

    void Upgrade()
    {
        upgradeLevel++;
        moneyLimit *= upgradeMultiplier;
        incomeAmount *= 1.3f;
        upgradeCost *= 2f;

        if (upgradeEffect != null)
        {
            upgradeEffect.SetActive(true);
            isUpgrading = true;
            upgradeAnimationTimer = 0f;
        }

        
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        if (currentMoney > moneyLimit)
        {
            currentMoney = moneyLimit;
        }

        if (upgradeEffect != null && !upgradeEffect.activeSelf && currentMoney > 0)
        {
            upgradeEffect.SetActive(true);
        }

      
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}