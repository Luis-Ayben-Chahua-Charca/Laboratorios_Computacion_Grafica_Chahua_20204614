using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SideScrollerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        Move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space key pressed and player is grounded. Attempting to jump.");
            Jump();
        }
    }

    private void Move()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;

        if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        Debug.Log("Jumping!");
        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }
}