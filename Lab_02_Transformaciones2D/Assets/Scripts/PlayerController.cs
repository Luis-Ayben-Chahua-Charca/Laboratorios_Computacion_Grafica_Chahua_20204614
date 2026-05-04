using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ladder Detection")]
    public Transform ladderCheck;
    public float ladderCheckRadius = 0.3f;
    public LayerMask ladderLayer;

    [Header("Ladder")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;

    [Header("Grab System")]
    public float grabRange = 1.2f;
    public LayerMask grabbableLayer;
    public Transform grabPoint; // punto delante del jugador

    private GameObject grabbedObject;
    private Rigidbody2D grabbedRb;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private float verticalInput;  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabbedObject == null)
                TryGrab();
            else
                ReleaseObject();
        }
    }

    void FixedUpdate()
    {
        isClimbing = Physics2D.OverlapCircle(ladderCheck.position, ladderCheckRadius, ladderLayer);
        CheckGround();

        if (isClimbing)
        {
            rb.gravityScale = 0f;

            // Cambiamos el 0 por moveInput para que puedas moverte de lado o despegarte
            rb.linearVelocity = new Vector2(moveInput * moveSpeed * 0.5f, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 3f;
            // Movimiento normal
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (grabbedObject != null)
        {
            grabbedObject.transform.position = grabPoint.position;
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void TryGrab()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, grabRange, grabbableLayer);

        if (hit == null) return; // evita el error

        grabbedObject = hit.gameObject;
        grabbedRb = grabbedObject.GetComponent<Rigidbody2D>();

        if (grabbedRb == null) return; // seguridad extra

        // Desactivar física
        grabbedRb.linearVelocity = Vector2.zero;
        grabbedRb.bodyType = RigidbodyType2D.Kinematic;
        grabbedRb.freezeRotation = true;

        // IGNORAR COLISIÓN
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D objectCol = grabbedObject.GetComponent<Collider2D>();

        if (playerCol != null && objectCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, true);
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject == null) return;

        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D objectCol = grabbedObject.GetComponent<Collider2D>();

        if (playerCol != null && objectCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, false);
        }

        grabbedRb.bodyType = RigidbodyType2D.Dynamic;
        grabbedRb.freezeRotation = false;

        grabbedObject = null;
        grabbedRb = null;
    }

    // =========================
    // DEBUG VISUAL
    // =========================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ladderCheck.position, ladderCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabRange);
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
