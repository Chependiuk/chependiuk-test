using UnityEngine;

public class NPCAutoPatrool : MonoBehaviour
{
    public Transform[] waypoints;    // ����� ����� ��������
    public float speed = 2f;         // �������� ���� NPC
    public float minDistance = 0.2f; // ̳������� ������� �� ����� ��� �����������
    public float avoidanceDistance = 2f; // ��������� ��� ��������� ��������
    public float raycastDistance = 3f;   // ������� ������� ��� ��������� ��������

    private int currentWaypointIndex = 0; // �������� ������ ����� ��������

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 directionToTarget = (targetWaypoint.position - transform.position).normalized;

        // �������� ��������
        Vector3 avoidanceDirection = AvoidObstacles(directionToTarget);

        // ��������� �������� ����
        Vector3 moveDirection = avoidanceDirection.magnitude > 0 ? avoidanceDirection : directionToTarget;

        // ��� NPC
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            transform.position + moveDirection, step);

        // ������� NPC
        if (moveDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        }

        // ������� �� �������� �����
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance <= minDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    Vector3 AvoidObstacles(Vector3 desiredDirection)
    {
        RaycastHit hit;
        Vector3 avoidance = Vector3.zero;

        // �������� ����� ����� NPC
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            // ����������, �� �� �� waypoint
            bool isWaypoint = false;
            foreach (Transform waypoint in waypoints)
            {
                if (hit.transform == waypoint)
                {
                    isWaypoint = true;
                    break;
                }
            }

            if (!isWaypoint && hit.distance < avoidanceDistance)
            {
                avoidance = Vector3.Cross(Vector3.up, hit.normal).normalized;
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            }
        }

        // �������� ������ (45 ������� ������ � ��������)
        Quaternion leftRay = Quaternion.Euler(0, -45, 0);
        Quaternion rightRay = Quaternion.Euler(0, 45, 0);

        if (Physics.Raycast(transform.position, leftRay * transform.forward, out hit, raycastDistance))
        {
            bool isWaypoint = false;
            foreach (Transform waypoint in waypoints)
            {
                if (hit.transform == waypoint)
                {
                    isWaypoint = true;
                    break;
                }
            }

            if (!isWaypoint && hit.distance < avoidanceDistance)
            {
                avoidance += Vector3.Cross(Vector3.up, hit.normal).normalized;
                Debug.DrawRay(transform.position, leftRay * transform.forward * hit.distance, Color.red);
            }
        }

        if (Physics.Raycast(transform.position, rightRay * transform.forward, out hit, raycastDistance))
        {
            bool isWaypoint = false;
            foreach (Transform waypoint in waypoints)
            {
                if (hit.transform == waypoint)
                {
                    isWaypoint = true;
                    break;
                }
            }

            if (!isWaypoint && hit.distance < avoidanceDistance)
            {
                avoidance += Vector3.Cross(Vector3.up, hit.normal).normalized;
                Debug.DrawRay(transform.position, rightRay * transform.forward * hit.distance, Color.red);
            }
        }

        return avoidance;
    }

    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                    if (i > 0)
                    {
                        Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
                    }
                }
            }
            if (waypoints.Length > 1)
            {
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            }
        }
    }
}