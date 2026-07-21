using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Referencias UI")]
    public GameObject pausePanel;      // arrastra aquí tu panel de pausa

    [Header("Referencias Post Processing (opcional)")]
    public GameObject postProcessVolumePause; // volumen con blur, si lo usas

    [Header("Referencias de control del jugador")]
    public PlayerController playerController;       // arrastra tu Player aquí
    public FPSCameraController cameraController;     // arrastra tu Player aquí (o donde esté el script)

    public bool IsPaused { get; private set; }

    void Awake()
    {
        // Singleton simple: solo un GameManager controla la pausa
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;          // congela física, animaciones, movimiento
        pausePanel.SetActive(true);

        if (postProcessVolumePause != null)
            postProcessVolumePause.SetActive(true);

        // Cursor visible para poder clickear el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactiva movimiento y mouse look — esto es lo que arregla
        // que la cámara siga girando con el mouse estando en pausa
        if (playerController != null) playerController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        if (postProcessVolumePause != null)
            postProcessVolumePause.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reactiva movimiento y mouse look
        if (playerController != null) playerController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
    }

    // Conecta este método al botón "Menú principal"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANTE: resetear antes de cambiar de escena
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}