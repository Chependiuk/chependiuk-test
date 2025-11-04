using UnityEngine;
using UnityEngine.AI; // Потрібно для NavMesh Agent

public class NPCBrain : MonoBehaviour
{
    [Tooltip("Як часто NPC буде приймати рішення (в секундах).")]
    public float decisionInterval = 10f;
    private float timer;

    private NavMeshAgent agent;
    private NPCActions actions; // Посилання на скрипт-виконавець

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        actions = GetComponent<NPCActions>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= decisionInterval)
        {
            MakeDecision();
            timer = 0;
        }
    }

    void MakeDecision()
    {
        // 1. Збираємо інформацію про світ
        string context = $"Ти офісний працівник. Зараз {DayNightCycle.Instance.GetCurrentTimeAsString()}. Поруч є вільні об'єкти: 'Робочий стіл', 'Кавова машина'.";
        string commandList = "Твоя відповідь має бути ОДНІЄЮ з команд: WORK, DRINK_COFFEE, WANDER.";

        string prompt = context + " " + commandList;

        // 2. Надсилаємо запит до Gemini
        APIManager.Instance.SendPromptToGemini(prompt, OnDecisionReceived);
    }

    void OnDecisionReceived(string decision)
    {
        Debug.Log($"Gemini вирішив: {decision}");

        // 4. Виконуємо команду
        // Прибираємо зайві символи, які може додати AI
        string cleanDecision = decision.Trim().ToUpper();

        switch (cleanDecision)
        {
            case "WORK":
                actions.GoToWorkstation();
                break;
            case "DRINK_COFFEE":
                actions.GoToCoffeeMachine();
                break;
            case "WANDER":
                actions.WanderAround();
                break;
            default:
                Debug.LogWarning($"AI повернуло невідому команду: {decision}. NPC буде блукати.");
                actions.WanderAround();
                break;
        }
    }
}