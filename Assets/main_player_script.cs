using UnityEngine;

public class main_player_script : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 10f; // �������� ��������
    public DynamicJoystick dynamicJoystick;
    public Rigidbody rb;

    [Header("Money System")]
    public float playerMoney = 0f;
    public float interactionDistance = 2f;
    public LayerMask interactableLayer;
    public TMPro.TextMeshProUGUI moneyText; // ��������� �� TextMeshPro �������

    void Start()
    {
        // ����������� ��������� �� ���� X �� Z
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        // �������� ���������� ������ �����䳿 (���������, E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
            Debug.Log("12345678");
        }
    }

    public void FixedUpdate()
    {
        // �������� �������� ���� �� ���������
        Vector3 direction = new Vector3(dynamicJoystick.Horizontal, 0, dynamicJoystick.Vertical);

        // ������ ���������
        if (direction.magnitude > 0.1f) // ����������, �� �������� ��������
        {
            rb.AddForce(direction.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // ������� ��������� � �������� ����
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void TryInteract()
    {
        // ���������� �� � ��'��� ��� �����䳿 ����� �������
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("CashRegister"))
            {
                Debug.Log("12345678");
                InteractWithCashRegister();
            }
        }
    }

    private void InteractWithCashRegister()
    {
        // ������ �����
        float moneyToAdd = 100f; // ��� ����-��� ���� ����
        playerMoney += moneyToAdd;

        // ��������� UI
        UpdateMoneyUI();

        Debug.Log($"�� �������� {moneyToAdd} ������. �������: {playerMoney}");
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"�����: {playerMoney}$";
        }
    }
}