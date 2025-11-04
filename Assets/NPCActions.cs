using UnityEngine;
using UnityEngine.AI;

public class NPCActions : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoToWorkstation()
    {
        Vector3 deskPosition = new Vector3(10, 0, 5);
        agent.SetDestination(deskPosition);
    }

    public void GoToCoffeeMachine()
    {
        Vector3 coffeePosition = new Vector3(-5, 0, 8);
        agent.SetDestination(coffeePosition);
    }

    public void WanderAround()
    {
        // Тепер викликаємо метод, який знаходиться всередині цього ж скрипта
        Vector3 randomPos = RandomNavSphere(transform.position, 10f, -1);
        agent.SetDestination(randomPos);
    }

    // --- ДОДАНО: Метод перенесено сюди ---
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
}