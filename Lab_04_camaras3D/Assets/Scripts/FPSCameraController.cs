using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraPivot;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float upDownRange = 80f;

    private float verticalRotation = 0f;

    void Update()
    {
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal del jugador
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotación vertical del pivot
        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -upDownRange,
            upDownRange
        );

        cameraPivot.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}