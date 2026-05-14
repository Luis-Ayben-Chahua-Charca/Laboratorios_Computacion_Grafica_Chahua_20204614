using Unity.VisualScripting;
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

    [Header("Machine Interaction")]
    public LayerMask machineLayer;
    public float interactRange = 1.5f;
    public LayerMask slotLayer;

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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            TryUseMachine();
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

        MachineSlot[] slots = Object.FindObjectsByType<MachineSlot>();

        foreach (var s in slots)
        {
            if (s.storedObject == grabbedObject.transform)
            {
                s.storedObject = null;
                break;
            }
        }


        // Desactivar física
        grabbedRb.bodyType = RigidbodyType2D.Kinematic;
        grabbedRb.constraints = RigidbodyConstraints2D.None; // 🔥 clave
        grabbedRb.freezeRotation = true; // solo mientras lo cargas

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

        MachineSlot slot = FindClosestSlot();

        if (slot != null && slot.storedObject == null && slot.playerInRange)
        {
            grabbedObject.transform.SetPositionAndRotation(
                slot.transform.position,
                Quaternion.identity
            );

            grabbedRb.bodyType = RigidbodyType2D.Kinematic;
            grabbedRb.constraints = RigidbodyConstraints2D.FreezeAll;

            
            Collider2D playerCol = GetComponent<Collider2D>();
            Collider2D objectCol = grabbedObject.GetComponent<Collider2D>();

            if (playerCol != null && objectCol != null)
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, false);
            }

            slot.storedObject = grabbedObject.transform;

            grabbedObject = null;
            grabbedRb = null;

            return;
        }

        // normal
        Collider2D playerCol2 = GetComponent<Collider2D>();
        Collider2D objectCol2 = grabbedObject.GetComponent<Collider2D>();

        if (playerCol2 != null && objectCol2 != null)
        {
            Physics2D.IgnoreCollision(playerCol2, objectCol2, false);
        }

        grabbedRb.bodyType = RigidbodyType2D.Dynamic;
        grabbedRb.freezeRotation = false;

        grabbedObject = null;
        grabbedRb = null;
    }

    MachineSlot FindClosestSlot()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, slotLayer);

        if (hit != null)
        {
            return hit.GetComponentInParent<MachineSlot>();
        }

        return null;
    }
    /*void ReleaseObject()
    {
        if (grabbedObject == null) return;

        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D objectCol = grabbedObject.GetComponent<Collider2D>();

        if (playerCol != null && objectCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, false);
        }

        grabbedRb.bodyType = RigidbodyType2D.Dynamic;
        grabbedRb.linearVelocity = Vector2.zero;
        grabbedRb.freezeRotation = false;

        grabbedObject = null;
        grabbedRb = null;
    }*/

    public void ForceReleaseIfHolding(GameObject obj)
    {
        if (grabbedObject == obj)
        {
            ReleaseObject();
        }
    }

    void TryUseMachine()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, machineLayer);

        if (hit != null)
        {
            IMachine machine = hit.GetComponentInParent<IMachine>();

            if (machine != null)
            {
                machine.Activate();
            }
        }
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
