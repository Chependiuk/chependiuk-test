using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Посилання на трансформ персонажа
    public Vector3 offset = new Vector3(0f, 0f, 0f); // Зміщення камери відносно персонажа
    public float smoothSpeed = 5f; // Плавність руху камери

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("CameraFollow: Player reference not set!");
            return;
        }

        // Розраховуємо цільову позицію камери
        Vector3 desiredPosition = player.position + offset;

        // Плавно переміщуємо камеру до цільової позиції
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Завжди дивимося на персонажа
        transform.LookAt(player);
    }
}