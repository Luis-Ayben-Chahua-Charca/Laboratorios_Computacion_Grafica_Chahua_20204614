using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Triggers")]
    [SerializeField] CannonController cannonController;

    [Header("UI Juego")]
    [SerializeField] GameObject canvasJuego; // HUD normal

    [Header("UI Resultado")] // fondo (imagen o panel)
    [SerializeField] Canvas textoGanaste;
    [SerializeField] Canvas textoPerdiste;

    private int secundariosDerribados = 0;
    [SerializeField] int necesariosParaGanar = 5;
    private bool juegoTerminado = false;

    [Header("Puntaje")]
    private int puntaje = 0;
    [SerializeField] TextMeshProUGUI textoPuntaje;
    [SerializeField] int puntosSecundario = 100;
    [SerializeField] int puntosPrincipal = 200;
    [SerializeField] int puntosPorDisparoRestante = 200;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Estado inicial
        textoPuntaje.gameObject.SetActive(true);
        textoGanaste.gameObject.SetActive(false);
        textoPerdiste.gameObject.SetActive(false);
        if (cannonController != null)
        {
            cannonController.onSinDisparos += GameOver;
        }
    }

    public void GameOver()
    {
        
        canvasJuego.SetActive(false);

        textoPerdiste.gameObject.SetActive(true);
        textoGanaste.gameObject.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Win()
    {
        
        canvasJuego.SetActive(false);

        int disparosRestantes = cannonController.ProyectilesDisponibles;
        puntaje += disparosRestantes * puntosPorDisparoRestante;

        textoGanaste.gameObject.SetActive(true);
        textoPerdiste.gameObject.SetActive(false);

        Time.timeScale = 0f;
    }

    public void RegistrarSecundario()
    {
        if (juegoTerminado) return;
        secundariosDerribados++;
        if (secundariosDerribados >= necesariosParaGanar)
        {
            juegoTerminado = true;
            Win();
        }
    }

    public void SumarSecundario()
    {
        puntaje += puntosSecundario;
        RegistrarSecundario();
        ActualizarPuntaje();
    }

    public void SumarPrincipal()
    {
        puntaje += puntosPrincipal;
        ActualizarPuntaje();
        Win();
    }

    public void ActualizarPuntaje()
    {
        textoPuntaje.text = "Puntos: " + puntaje;
        Debug.Log("Puntaje actualizado: " + puntaje);
    }
}