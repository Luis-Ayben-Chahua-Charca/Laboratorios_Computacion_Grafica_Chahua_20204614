using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private GameObject fpsCamera;

    [SerializeField] private GameObject tpsCamera;

    [Header("Controllers")]
    [SerializeField] private MonoBehaviour fpsController;

    [SerializeField] private MonoBehaviour tpsController;

    [Header("Input Settings")]
    [SerializeField] private KeyCode switchCameraKey = KeyCode.V;

    private bool isFirstPerson = true;

    void Start()
    {
        ActivateFPS();
    }

    void Update()
    {
        HandleCameraSwitch();
    }

    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(switchCameraKey))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                ActivateFPS();
            }
            else
            {
                ActivateTPS();
            }
        }
    }

    void ActivateFPS()
    {
        fpsCamera.SetActive(true);
        tpsCamera.SetActive(false);

        fpsController.enabled = true;
        tpsController.enabled = false;
    }

    void ActivateTPS()
    {
        fpsCamera.SetActive(false);
        tpsCamera.SetActive(true);

        fpsController.enabled = false;
        tpsController.enabled = true;
    }
}