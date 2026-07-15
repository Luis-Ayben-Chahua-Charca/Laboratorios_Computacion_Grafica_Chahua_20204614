using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runMultiplier = 10f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Other Inputs")]
    [SerializeField] private string verticalMoveInput = "Vertical";
    [SerializeField] private string horizontalMoveInput = "Horizontal";
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private CharacterController characterController;
    private Vector3 currentMovement = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float speedMultiplier = Input.GetKey(sprintKey) ? runMultiplier : 1f;
        float horizontalInput = Input.GetAxis(horizontalMoveInput);
        float verticalInput = Input.GetAxis(verticalMoveInput);

        float currentSpeed = walkSpeed * speedMultiplier;

        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;

        currentMovement.x = movement.x * currentSpeed;
        currentMovement.z = movement.z * currentSpeed;

        handleGravityAndJumping();

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void handleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (Input.GetKeyDown(jumpKey))
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime; // Apply gravity when in the air
        }
    }
}