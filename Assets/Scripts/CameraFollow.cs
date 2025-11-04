using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Загальні налаштування")]
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Режими слідування за гравцем")]
    [Tooltip("Стандартна відстань камери від гравця.")]
    public Vector3 playerOffset = new Vector3(0f, 3f, -2f);
    [Tooltip("Відстань камери від гравця під час діалогу.")]
    public Vector3 zoomedOffset = new Vector3(-1f, 1.5f, -2.5f); // Повернули це поле

    [Header("Режим будівництва")]
    [Tooltip("Позиція камери відносно об'єкта в режимі будівництва.")]
    public Vector3 buildModeOffset = new Vector3(0f, 8f, -6f);
    [Tooltip("Фіксований кут нахилу камери в режимі будівництва.")]
    public Vector3 buildModeRotation = new Vector3(55f, 0f, 0f);

    // Внутрішні змінні
    private Transform originalTarget;
    private bool isInBuildMode = false;
    private Vector3 currentTargetOffset; // Повернули цю змінну

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        currentTargetOffset = playerOffset; // Встановлюємо стандартний офсет на старті
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition;
        Quaternion desiredRotation;

        if (isInBuildMode)
        {
            // Логіка для режиму будівництва
            desiredPosition = target.position + buildModeOffset;
            desiredRotation = Quaternion.Euler(buildModeRotation);
        }
        else
        {
            // Логіка для слідування за гравцем (стандартна або наближена)
            desiredPosition = target.position + currentTargetOffset; // Використовуємо поточний офсет
            desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }

    // Методи для режиму будівництва
    public void SetTemporaryTarget(Transform tempTarget)
    {
        if (originalTarget == null) originalTarget = target;
        target = tempTarget;
        isInBuildMode = true;
    }

    public void ClearTemporaryTarget()
    {
        if (originalTarget != null)
        {
            target = originalTarget;
            originalTarget = null;
        }
        isInBuildMode = false;
        currentTargetOffset = playerOffset; // Завжди повертаємось до стандартного офсету
    }

    // --- ПОВЕРНУЛИ МЕТОДИ ДЛЯ ЗУМУ В ДІАЛОГАХ ---

    /// <summary>
    /// Наближує камеру для діалогу.
    /// </summary>
    public void ZoomIn()
    {
        // Дозволяємо наближення тільки якщо ми НЕ в режимі будівництва
        if (!isInBuildMode)
        {
            currentTargetOffset = zoomedOffset;
        }
    }

    /// <summary>
    /// Віддаляє камеру після діалогу.
    /// </summary>
    public void ZoomOut()
    {
        currentTargetOffset = playerOffset;
    }
}