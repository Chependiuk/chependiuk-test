using UnityEngine;

public class main_player_script : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 10f; // Швидкість повороту
    public DynamicJoystick dynamicJoystick;
    public Rigidbody rb;

    [Header("Money System")]
    public float playerMoney = 0f;
    public float interactionDistance = 2f;
    public LayerMask interactableLayer;
    public TMPro.TextMeshProUGUI moneyText; // Посилання на TextMeshPro елемент

    void Start()
    {
        // Зафіксувати обертання по осях X та Z
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        // Перевірка натискання кнопки взаємодії (наприклад, E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
            Debug.Log("12345678");
        }
    }

    public void FixedUpdate()
    {
        // Отримуємо напрямок руху від джойстика
        Vector3 direction = new Vector3(dynamicJoystick.Horizontal, 0, dynamicJoystick.Vertical);

        // Рухаємо персонажа
        if (direction.magnitude > 0.1f) // Перевіряємо, чи джойстик активний
        {
            rb.AddForce(direction.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // Поворот персонажа в напрямку руху
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void TryInteract()
    {
        // Перевіряємо чи є об'єкт для взаємодії перед гравцем
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
        // Додаємо гроші
        float moneyToAdd = 100f; // або будь-яка інша сума
        playerMoney += moneyToAdd;

        // Оновлюємо UI
        UpdateMoneyUI();

        Debug.Log($"Ви отримали {moneyToAdd} грошей. Загалом: {playerMoney}");
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Гроші: {playerMoney}$";
        }
    }
}