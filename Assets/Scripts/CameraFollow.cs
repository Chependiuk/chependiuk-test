using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Ціль для камери")]
    public Transform target;

    [Header("Налаштування відстані")]
    [Tooltip("Стандартна відстань камери від гравця.")]
    // Нові значення за замовчуванням
    public Vector3 defaultOffset = new Vector3(0f, 3f, -2f);

    [Tooltip("Відстань камери від гравця під час діалогу (ближче).")]
    // Нові значення за замовчуванням
    public Vector3 zoomedOffset = new Vector3(-1f, 1f, 1.2f);

    // Нове значення за замовчуванням
    public float smoothSpeed = 2f;

    private Vector3 currentTargetOffset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentTargetOffset = defaultOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + currentTargetOffset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }

    public void ZoomIn()
    {
        currentTargetOffset = zoomedOffset;
    }

    public void ZoomOut()
    {
        currentTargetOffset = defaultOffset;
    }
}