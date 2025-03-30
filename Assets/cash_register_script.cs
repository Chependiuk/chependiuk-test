using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum InteractState { None, Near, Active }

    [Header("Visual Settings")]
    public float activeBounceHeight = 0.2f;
    public float activeBounceSpeed = 5f;
    public float nearBounceHeight = 0.1f;
    public float nearBounceSpeed = 3f;

    [Header("UI References")]
    public GameObject activePrompt; // "Натисніть E"
    public GameObject nearPrompt;   // "Підійдіть ближче"

    private Vector3 originalPosition;
    private InteractState currentState;

    void Start()
    {
        originalPosition = transform.position;
        SetInteractState(InteractState.None);
    }

    void Update()
    {
        if (currentState != InteractState.None)
        {
            float height = currentState == InteractState.Active ? activeBounceHeight : nearBounceHeight;
            float speed = currentState == InteractState.Active ? activeBounceSpeed : nearBounceSpeed;

            float newY = originalPosition.y + Mathf.Sin(Time.time * speed) * height;
            transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
    }

    public void SetInteractState(InteractState state)
    {
        currentState = state;

        // Оновлення UI
        if (activePrompt != null)
            activePrompt.SetActive(state == InteractState.Active);
        if (nearPrompt != null)
            nearPrompt.SetActive(state == InteractState.Near);

        // Скидання позиції при виході
        if (state == InteractState.None)
            transform.position = originalPosition;
    }
}