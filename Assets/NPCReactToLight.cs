using UnityEngine;

public class NPCReactToLight : MonoBehaviour
{
    [Header("Налаштування реакції")]
    [Tooltip("Як далеко NPC 'бачить' мерехтіння світла.")]
    public float reactionRadius = 10f;

    [Header("Налаштування анімації")]
    [Tooltip("Посилання на аніматор цього NPC.")]
    public Animator npcAnimator;
    [Tooltip("Назва тригера в аніматорі, який запускає анімацію 'дивиться на лампочку'.")]
    public string lookAtLightTriggerName = "LookAtLight";

    private void OnEnable()
    {
        // Підписуємось на сигнал від будь-якої лампочки
        FlickeringLight.OnLightFlickered += HandleLightFlicker;
    }

    private void OnDisable()
    {
        // Обов'язково відписуємось, щоб уникнути помилок
        FlickeringLight.OnLightFlickered -= HandleLightFlicker;
    }

    /// <summary>
    /// Цей метод викликається автоматично, коли будь-яка лампочка моргає.
    /// </summary>
    /// <param name="lightPosition">Позиція лампочки, яка моргнула.</param>
    private void HandleLightFlicker(Vector3 lightPosition)
    {
        if (npcAnimator == null) return; // Якщо немає аніматора, нічого не робимо

        // Розраховуємо відстань від NPC до лампочки
        float distanceToLight = Vector3.Distance(transform.position, lightPosition);

        // Якщо лампочка знаходиться в межах радіуса реакції...
        if (distanceToLight <= reactionRadius)
        {
            Debug.Log(gameObject.name + " помітив мерехтіння і реагує!");

            // ...запускаємо тригер анімації
            npcAnimator.SetTrigger(lookAtLightTriggerName);
        }
    }
}