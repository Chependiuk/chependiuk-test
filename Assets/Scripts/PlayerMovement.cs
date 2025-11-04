using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public DynamicJoystick dynamicJoystick;
    public Animator animator;

    [Header("Керування UI")]
    [Tooltip("Перетягніть сюди головний канвас з грошима, підказками і т.д.")]
    public GameObject mainHudCanvas;
    [Tooltip("Перетягніть сюди об'єкт Canvas, на якому знаходиться джойстик.")]
    public GraphicRaycaster joystickRaycaster;

    private Rigidbody rb;
    private bool canMove = true;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (joystickRaycaster == null)
        {
            Debug.LogError("!!! Graphic Raycaster для джойстика не призначено в інспекторі PlayerMovement !!!");
        }

        SetMovementState(true);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            HandleMovement();
        }
    }

    public void SetMovementState(bool state)
    {
        canMove = state;

        if (joystickRaycaster != null)
        {
            joystickRaycaster.enabled = state;
        }

        if (state)
        {
#if !UNITY_EDITOR
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
#endif
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!state)
        {
            if (animator != null) animator.SetBool("IsWalking", false);
            if (rb != null) rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    /// <summary>
    /// Ховає головний ігровий інтерфейс (HUD).
    /// </summary>
    public void HideMainHUD()
    {
        if (mainHudCanvas != null) mainHudCanvas.SetActive(false);
    }

    /// <summary>
    /// Показує головний ігровий інтерфейс (HUD).
    /// </summary>
    public void ShowMainHUD()
    {
        if (mainHudCanvas != null) mainHudCanvas.SetActive(true);
    }

    private void HandleMovement()
    {
        if (dynamicJoystick == null) return;

        Vector3 direction = new Vector3(dynamicJoystick.Horizontal, 0, dynamicJoystick.Vertical);
        if (direction.magnitude > 0.1f)
        {
            if (animator != null) animator.SetBool("IsWalking", true);
            rb.linearVelocity = new Vector3(direction.normalized.x * speed, rb.linearVelocity.y, direction.normalized.z * speed);
            RotatePlayer(direction);
        }
        else
        {
            if (animator != null) animator.SetBool("IsWalking", false);
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void RotatePlayer(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}