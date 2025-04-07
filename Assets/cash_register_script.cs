using UnityEngine;
using TMPro;

public class InteractableObject : MonoBehaviour
{
    public enum InteractState { None, Near, Active }

    [Header("Visual Settings")]
    public float activeBounceHeight = 0.2f;
    public float activeBounceSpeed = 5f;
    public float nearBounceHeight = 0.1f;
    public float nearBounceSpeed = 3f;

    [Header("UI References")]
    public GameObject activePrompt; // "�������� E"
    public GameObject nearPrompt;   // "ϳ����� ������"
    public TextMeshPro moneyText;   // 3D �����

    [Header("Money Settings")]
    public float maxMoney = 1000f;
    public float moneyAccumulationRate = 50f;
    public float collectionCooldown = 5f;

    [Header("Money Cube")]
    public GameObject moneyCube;    // ��� ���������� ��� � �����
    public float minHeight = 0.1f; // ̳������� ������ ����
    public float maxHeight = 2f;    // ����������� ������

    private Vector3 originalPosition;
    private Vector3 originalCubePosition;
    private InteractState currentState;
    private float currentMoney = 0f;
    private float timeSinceLastCollection = 0f;
    private bool isCollecting = false;

    void Start()
    {
        originalPosition = transform.position;

        // �������� �������� �������
        if (moneyCube != null)
        {
            originalCubePosition = moneyCube.transform.position;
            moneyCube.transform.localScale = new Vector3(
                moneyCube.transform.localScale.x,
                minHeight,
                moneyCube.transform.localScale.z
            );
        }

        SetInteractState(InteractState.None);
        UpdateMoneyDisplay();
    }

    void Update()
    {
        // ϳ����������� ��'����
        if (currentState != InteractState.None)
        {
            float height = currentState == InteractState.Active ? activeBounceHeight : nearBounceHeight;
            float speed = currentState == InteractState.Active ? activeBounceSpeed : nearBounceSpeed;

            float newY = originalPosition.y + Mathf.Sin(Time.time * speed) * height;
            transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }

        // ����������� ������
        if (!isCollecting && currentMoney < maxMoney)
        {
            currentMoney += moneyAccumulationRate * Time.deltaTime;
            currentMoney = Mathf.Min(currentMoney, maxMoney);
            UpdateMoneyDisplay();
            UpdateCubeHeight();
        }
        else if (isCollecting)
        {
            timeSinceLastCollection += Time.deltaTime;
            if (timeSinceLastCollection >= collectionCooldown)
            {
                isCollecting = false;
                timeSinceLastCollection = 0f;
                UpdateMoneyDisplay();
            }
        }
    }

    void UpdateCubeHeight()
    {
        if (moneyCube == null) return;

        float progress = currentMoney / maxMoney;
        float newHeight = Mathf.Lerp(minHeight, maxHeight, progress);

        // ������� ����� ������ ����
        moneyCube.transform.localScale = new Vector3(
            moneyCube.transform.localScale.x,
            newHeight,
            moneyCube.transform.localScale.z
        );

        // �������� �������, ��� ��� ������� �����
        moneyCube.transform.position = new Vector3(
            originalCubePosition.x,
            originalCubePosition.y + newHeight / 2,
            originalCubePosition.z
        );
    }

    void UpdateMoneyDisplay()
    {
        if (moneyText != null)
            moneyText.text = $"{currentMoney:F0}/{maxMoney}";
    }

    public void SetInteractState(InteractState state)
    {
        currentState = state;

        if (activePrompt != null)
            activePrompt.SetActive(state == InteractState.Active);
        if (nearPrompt != null)
            nearPrompt.SetActive(state == InteractState.Near);

        if (state == InteractState.None)
            transform.position = originalPosition;

        if (state == InteractState.Active && Input.GetKeyDown(KeyCode.E))
            CollectMoney();
    }

    void CollectMoney()
    {
        if (currentMoney <= 0 || isCollecting) return;

        Debug.Log($"ǳ����� {currentMoney} ������!");
        currentMoney = 0f;
        isCollecting = true;
        timeSinceLastCollection = 0f;

        // ������� ���
        if (moneyCube != null)
        {
            moneyCube.transform.localScale = new Vector3(
                moneyCube.transform.localScale.x,
                minHeight,
                moneyCube.transform.localScale.z
            );
            moneyCube.transform.position = originalCubePosition;
        }

        if (moneyText != null)
        {
            moneyText.text = "ǳ�����!";
            Invoke("UpdateMoneyDisplay", 1f);
        }
    }
}