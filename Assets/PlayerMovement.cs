using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public DynamicJoystick dynamicJoystick;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
            RotatePlayer(direction);
        }
    }

    private void RotatePlayer(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}