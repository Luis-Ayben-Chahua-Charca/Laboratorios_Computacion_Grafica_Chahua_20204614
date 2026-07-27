using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraPivot;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header ("Positions Camera")]
    private float alturaOriginalY;


    private float verticalRotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alturaOriginalY = cameraPivot.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        // Rotate the player body horizontally
        playerBody.Rotate(Vector3.up * mouseX);
        // Rotate the camera vertically
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
        cameraPivot.localRotation =
           Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void SetOffsetAltura(float offset)
    {
        Vector3 pos = cameraPivot.localPosition;
        pos.y = alturaOriginalY + offset;
        cameraPivot.localPosition = pos;
    }
}