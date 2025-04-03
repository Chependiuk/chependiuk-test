using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCPatrol : MonoBehaviour
{
    private Transform[] waypoints;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minDistance = 0.2f;
    [SerializeField] private float avoidanceForce = 2f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private float randomOffset = 0.5f;
    [SerializeField] private bool randomWaypointOrder = true; // Додано опцію випадкового порядку

    private int currentWaypointIndex = 0;
    private List<int> availableWaypointIndices = new List<int>();
    private Vector3 moveDirection;
    private bool isWaiting = false;
    private Animator animator;
    private Vector3 targetPosition;

    void Start()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        waypoints = new Transform[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints found with tag 'Waypoint' in the scene!");
        }

        // Ініціалізуємо список доступних індексів вейпоінтів
        availableWaypointIndices = Enumerable.Range(0, waypoints.Length).ToList();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on NPC!");
        }

        SetNewTargetPosition();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!isWaiting)
        {
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            Vector3 avoidanceDirection = AvoidOtherNPCs();
            moveDirection = (directionToTarget + avoidanceDirection).normalized;

            if (animator != null) animator.SetBool("IsWalking", true);

            float step = speed * Time.deltaTime * 0.25f;
            transform.position = Vector3.MoveTowards(transform.position,
                transform.position + moveDirection, step);

            if (moveDirection != Vector3.zero)
            {
                Vector3 flattenedDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
                if (flattenedDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(flattenedDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
                }
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= minDistance)
            {
                StartCoroutine(WaitAtWaypoint());
            }
        }
        else
        {
            Vector3 avoidanceDirection = AvoidOtherNPCs();
            if (avoidanceDirection != Vector3.zero)
            {
                float step = speed * Time.deltaTime * 0.25f;
                transform.position = Vector3.MoveTowards(transform.position,
                    transform.position + avoidanceDirection, step);
            }
        }
    }

    private Vector3 AvoidOtherNPCs()
    {
        Vector3 avoidance = Vector3.zero;
        Collider[] nearbyNPCs = Physics.OverlapSphere(transform.position, detectionRadius, npcLayer);

        foreach (Collider npc in nearbyNPCs)
        {
            if (npc.transform != transform)
            {
                Vector3 directionToNPC = (transform.position - npc.transform.position).normalized;
                float distanceToNPC = Vector3.Distance(transform.position, npc.transform.position);

                float avoidanceStrength = Mathf.Clamp01(1 - (distanceToNPC / detectionRadius)) * avoidanceForce;
                avoidance += directionToNPC * avoidanceStrength;
            }
        }

        return avoidance;
    }

    private void SetNewTargetPosition()
    {
        if (randomWaypointOrder)
        {
            // Якщо всі вейпоінти вичерпано, починаємо новий цикл
            if (availableWaypointIndices.Count == 0)
            {
                availableWaypointIndices = Enumerable.Range(0, waypoints.Length).ToList();

                // Видаляємо поточний вейпоінт, щоб не йти до нього знову
                if (availableWaypointIndices.Contains(currentWaypointIndex))
                {
                    availableWaypointIndices.Remove(currentWaypointIndex);
                }
            }

            // Вибираємо випадковий вейпоінт з доступних
            int randomIndex = Random.Range(0, availableWaypointIndices.Count);
            currentWaypointIndex = availableWaypointIndices[randomIndex];
            availableWaypointIndices.RemoveAt(randomIndex);
        }
        else
        {
            // Стара логіка - послідовний обхід
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 randomCircle = Random.insideUnitCircle * randomOffset;
        targetPosition = targetWaypoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        if (animator != null) animator.SetBool("IsWalking", false);

        float randomWait = waitTime + Random.Range(-0.5f, 0.5f);
        yield return new WaitForSeconds(randomWait);

        isWaiting = false;
        SetNewTargetPosition();
    }

    void OnCollisionEnter(Collision collision)
    {
        bool isWaypoint = false;
        foreach (Transform waypoint in waypoints)
        {
            if (collision.transform == waypoint)
            {
                isWaypoint = true;
                break;
            }
        }

        if (!isWaypoint)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            moveDirection = Vector3.Reflect(moveDirection, collisionNormal).normalized * avoidanceForce;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        bool isWaypoint = false;
        foreach (Transform waypoint in waypoints)
        {
            if (collision.transform == waypoint)
            {
                isWaypoint = true;
                break;
            }
        }

        if (!isWaypoint)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            moveDirection = Vector3.Reflect(moveDirection, collisionNormal).normalized * avoidanceForce;
        }
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
                }
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPosition, 0.1f);
    }
}