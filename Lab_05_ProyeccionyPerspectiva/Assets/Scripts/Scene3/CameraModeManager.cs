using UnityEngine;

public class CameraModeManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera sideScrollerCamera;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Controllers")]
    [SerializeField] private MonoBehaviour fpsCameraController;
    [SerializeField] private MonoBehaviour sideScrollerController;
    [SerializeField] private MonoBehaviour objectPickUpSystem;

    [Header ("Croshair")]
    [SerializeField] private GameObject crosshair;

    private bool isFirstPersonMode = true;

    private void Start()
    {
        SetFirstPersonMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleMode();
        }
    }

    private void ToggleMode()
    {
        if (isFirstPersonMode)
        {
            SetSideScrollerMode();
        }
        else
        {
            SetFirstPersonMode();
        }
    }

    private void SetFirstPersonMode()
    {
        isFirstPersonMode = true;

        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        firstPersonCamera.gameObject.SetActive(true);
        sideScrollerCamera.gameObject.SetActive(false);

        fpsCameraController.enabled = true;

        if (objectPickUpSystem != null)
            objectPickUpSystem.enabled = true;

        sideScrollerController.enabled = false;
    }

    private void SetSideScrollerMode()
    {
        isFirstPersonMode = false;

        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        firstPersonCamera.gameObject.SetActive(false);
        sideScrollerCamera.gameObject.SetActive(true);

        fpsCameraController.enabled = false;

        if (objectPickUpSystem != null)
            objectPickUpSystem.enabled = false;

        sideScrollerController.enabled = true;

        Vector3 rotation = player.eulerAngles;
        rotation.y = 180f;
        player.eulerAngles = rotation;
    }
}