using UnityEngine;

public enum EstadoJuego { ExploracionLibre, Flashback, Cinematica, Pausado }

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance { get; private set; }

    [Header("Referencias del jugador")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FPSCameraController cameraController;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private HUDController hud;
    [SerializeField] private GameObject postProcessVolumePause; // opcional: blur de fondo en pausa

    public EstadoJuego EstadoActual { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => CambiarEstado(EstadoJuego.ExploracionLibre);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EstadoActual == EstadoJuego.ExploracionLibre) Pausar();
            else if (EstadoActual == EstadoJuego.Pausado) Reanudar();
        }
    }

    public void CambiarEstado(EstadoJuego nuevoEstado)
    {
        EstadoActual = nuevoEstado;
        bool jugadorTieneControl = nuevoEstado == EstadoJuego.ExploracionLibre;

        playerController.enabled = jugadorTieneControl;
        cameraController.enabled = jugadorTieneControl;

        Cursor.lockState = jugadorTieneControl ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !jugadorTieneControl;

        Time.timeScale = nuevoEstado == EstadoJuego.Pausado ? 0f : 1f;

        pausePanel.SetActive(nuevoEstado == EstadoJuego.Pausado);
        if (postProcessVolumePause != null)
            postProcessVolumePause.SetActive(nuevoEstado == EstadoJuego.Pausado);

        hud.SetMisionesVisible(nuevoEstado != EstadoJuego.Cinematica);
        hud.SetItemVisible(jugadorTieneControl);
        hud.SetCrosshairForzado(nuevoEstado != EstadoJuego.ExploracionLibre);
    }

    public void Pausar() => CambiarEstado(EstadoJuego.Pausado);
    public void Reanudar() => CambiarEstado(EstadoJuego.ExploracionLibre);
    public void IniciarCinematica() => CambiarEstado(EstadoJuego.Cinematica);
    public void IniciarFlashback() => CambiarEstado(EstadoJuego.Flashback);
    public void TerminarEvento() => CambiarEstado(EstadoJuego.ExploracionLibre);

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; // importante: resetear antes de cambiar de escena
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}