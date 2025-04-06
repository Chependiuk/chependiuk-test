using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;          // Об'єкт, за яким слідує камера
    public Vector3 offset = new Vector3(0f, 2f, -5f);  // Зміщення камери відносно цілі
    public float smoothSpeed = 5f;    // Плавність руху

    void LateUpdate()
    {
        if (target == null) return;  // Перевірка наявності цілі

        // Розрахунок бажаної позиції камери
        Vector3 desiredPosition = target.position + offset;

        // Плавне переміщення камери
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Застосування нової позиції
        transform.position = smoothedPosition;

        // Камера завжди дивиться на ціль
        transform.LookAt(target);
    }
}