using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // ��������� �� ��������� ���������
    public Vector3 offset = new Vector3(0f, 0f, 0f); // ������� ������ ������� ���������
    public float smoothSpeed = 5f; // �������� ���� ������

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("CameraFollow: Player reference not set!");
            return;
        }

        // ����������� ������� ������� ������
        Vector3 desiredPosition = player.position + offset;

        // ������ ��������� ������ �� ������� �������
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // ������ �������� �� ���������
        transform.LookAt(player);
    }
}