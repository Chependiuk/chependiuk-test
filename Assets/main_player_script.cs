using UnityEngine;
using static InteractableObject;

public class main_player_script : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public DynamicJoystick dynamicJoystick;
    public Rigidbody rb;

    [Header("Money System")]
    public float playerMoney = 0f;
    public TMPro.TextMeshProUGUI moneyText;

    [Header("Interaction Settings")]
    [Tooltip("Дистанція для повної взаємодії")]
    public float closeInteractionDistance = 2f;
    [Tooltip("Дистанція для попередження")]
    public float farInteractionDistance = 4f;
    public LayerMask interactableLayer;

    private InteractableObject currentInteractable;

    void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        UpdateMoneyUI();
    }

    private void Update()
    {
        CheckInteractables();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null &&
            GetDistanceTo(currentInteractable.transform) <= closeInteractionDistance)
        {
            InteractWithObject();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(dynamicJoystick.Horizontal, 0, dynamicJoystick.Vertical);
        if (direction.magnitude > 0.1f)
        {
            rb.AddForce(direction.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void CheckInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, farInteractionDistance, interactableLayer);
        InteractableObject nearestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            InteractableObject interactable = col.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                float distance = GetDistanceTo(col.transform);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestObject = interactable;
                }
            }
        }

        // Обробка зміни об'єкта
        if (currentInteractable != nearestObject)
        {
            if (currentInteractable != null)
                currentInteractable.SetInteractState(InteractableObject.InteractState.None);

            currentInteractable = nearestObject;
        }

        // Встановлення стану для поточного об'єкта
        if (currentInteractable != null)
        {
            float distance = GetDistanceTo(currentInteractable.transform);
            if (distance <= closeInteractionDistance)
            {
                currentInteractable.SetInteractState(InteractableObject.InteractState.Active);
            }
            else if (distance <= farInteractionDistance)
            {
                currentInteractable.SetInteractState(InteractableObject.InteractState.Near);
            }
            else
            {
                currentInteractable.SetInteractState(InteractableObject.InteractState.None);
                currentInteractable = null;
            }
        }
    }

    private float GetDistanceTo(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private void InteractWithObject()
    {
        if (currentInteractable.CompareTag("CashRegister"))
        {
            playerMoney += 100f;
            UpdateMoneyUI();
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Гроші: {playerMoney}$";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, closeInteractionDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farInteractionDistance);
    }
}