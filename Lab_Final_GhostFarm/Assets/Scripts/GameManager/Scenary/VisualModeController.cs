using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class VisualModeController : MonoBehaviour
{
    public static VisualModeController Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private SkyboxController skyboxController;
    [SerializeField] private Light solDireccional;
    [SerializeField] private Volume volumeRealista;
    [SerializeField] private Volume volumeMemoria;

    [Header("Ajuste de luz en recuerdo")]
    [SerializeField] private Color colorLuzMemoria = new Color(1f, 0.78f, 0.65f); // cálido, dorado-rosado
    [SerializeField] private Vector3 rotacionSolMemoria = new Vector3(10f, 40f, 0f); // sol bajo, casi horizonte
    [SerializeField] private float duracionBlend = 2f;

    [Header("Sensación de escala")]
    [SerializeField] private FPSCameraController cameraController;
    [SerializeField] private ManoJugador manoJugador;
    [SerializeField] private float bajarAlturaEnMemoria = 0.4f;

    [Header("Fog")]
    [SerializeField] private Color fogColorRealista;
    [SerializeField] private Color fogColorMemoria;
    [SerializeField] private float fogDensityRealista = 0.02f;
    [SerializeField] private float fogDensityMemoria = 0.01f;

    private IDualStyle[] objetosDuales;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        objetosDuales = GetDualStyles();

        // FIX: antes acá se fotografiaba una sola vez el color/rotación de la
        // luz ("colorLuzOriginal"/"rotacionSolOriginal") y se usaba como el
        // "realista" fijo para todo el juego, sin importar la hora del día.
        // Ahora esos valores viven en SkyboxController (por CondicionCielo),
        // así que no hay nada que fotografiar acá.
    }

    // FIX: el juego debe arrancar explícitamente en modo realista, sin depender
    // de en qué posición hayan quedado los sliders de Weight de los Volumes en
    // el Inspector. instantaneo=true evita que se vea un blend de 2s al cargar.
    // Para cuando este Start() corre, SkyboxController.Awake() ya aplicó la
    // condición inicial (Awake de todos los objetos corre antes que
    // cualquier Start), así que ColorLuzActual/RotacionLuzActual ya son
    // correctos (ej. los de Mañana del Día 1).
    void Start() => SalirModoMemoria(instantaneo: true);

    private IDualStyle[] GetDualStyles()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include)
            .OfType<IDualStyle>()
            .ToArray();
    }

    public void EntrarModoMemoria(bool instantaneo = false)
    {
        if (skyboxController == null) Debug.LogError("Falta asignar skyboxController", this);
        if (solDireccional == null) Debug.LogError("Falta asignar solDireccional", this);
        if (volumeRealista == null) Debug.LogError("Falta asignar volumeRealista", this);
        if (volumeMemoria == null) Debug.LogError("Falta asignar volumeMemoria", this);
        if (cameraController == null) Debug.LogError("Falta asignar cameraController", this);
        if (manoJugador == null) Debug.LogError("Falta asignar manoJugador", this);

        skyboxController.EntrarModoMemoria();
        foreach (var obj in objetosDuales) obj.SetModo(true);

        if (instantaneo) AplicarLuzInstantanea(true);
        else StartCoroutine(BlendLuz(true));
    }

    public void SalirModoMemoria(bool instantaneo = false)
    {
        if (skyboxController == null) Debug.LogError("Falta asignar skyboxController", this);
        if (solDireccional == null) Debug.LogError("Falta asignar solDireccional", this);
        if (volumeRealista == null) Debug.LogError("Falta asignar volumeRealista", this);
        if (volumeMemoria == null) Debug.LogError("Falta asignar volumeMemoria", this);
        if (cameraController == null) Debug.LogError("Falta asignar cameraController", this);
        if (manoJugador == null) Debug.LogError("Falta asignar manoJugador", this);

        foreach (var obj in objetosDuales) obj.SetModo(false);

        // FIX: antes nada revertía el skybox después de un flashback —
        // EntrarModoMemoria cambiaba el material, pero SalirModoMemoria nunca
        // lo restauraba. Ahora vuelve al skybox de la condición del día
        // (Mañana/Tarde/Noche) que estuviera activa antes de entrar a memoria.
        skyboxController.RestaurarUltimaCondicion();

        if (instantaneo) AplicarLuzInstantanea(false);
        else StartCoroutine(BlendLuz(false));
    }

    private void AplicarLuzInstantanea(bool memoria)
    {
        solDireccional.color = memoria ? colorLuzMemoria : skyboxController.ColorLuzActual;
        solDireccional.transform.rotation = memoria ? Quaternion.Euler(rotacionSolMemoria) : skyboxController.RotacionLuzActual;
        volumeMemoria.weight = memoria ? 1f : 0f;
        volumeRealista.weight = memoria ? 0f : 1f;

        RenderSettings.fogColor = memoria ? fogColorMemoria : fogColorRealista;
        RenderSettings.fogDensity = memoria ? fogDensityMemoria : fogDensityRealista;

        cameraController.SetOffsetAltura(memoria ? -bajarAlturaEnMemoria : 0f);
        manoJugador.SetOffsetAltura(memoria ? -bajarAlturaEnMemoria : 0f);
    }

    private IEnumerator BlendLuz(bool memoria)
    {
        float t = 0;
        Color colorInicio = solDireccional.color;
        Color colorFin = memoria ? colorLuzMemoria : skyboxController.ColorLuzActual;
        Quaternion rotInicio = solDireccional.transform.rotation;
        Quaternion rotFin = memoria ? Quaternion.Euler(rotacionSolMemoria) : skyboxController.RotacionLuzActual;
        float pesoInicioMemoria = volumeMemoria.weight;
        float pesoFinMemoria = memoria ? 1f : 0f;

        float offsetInicio = memoria ? 0f : -bajarAlturaEnMemoria;
        float offsetFin = memoria ? -bajarAlturaEnMemoria : 0f;

        while (t < duracionBlend)
        {
            t += Time.deltaTime;
            float p = t / duracionBlend;
            solDireccional.color = Color.Lerp(colorInicio, colorFin, p);
            solDireccional.transform.rotation = Quaternion.Slerp(rotInicio, rotFin, p);
            volumeMemoria.weight = Mathf.Lerp(pesoInicioMemoria, pesoFinMemoria, p);
            volumeRealista.weight = 1f - volumeMemoria.weight;
            RenderSettings.fogColor = Color.Lerp(memoria ? fogColorRealista : fogColorMemoria, memoria ? fogColorMemoria : fogColorRealista, p);
            RenderSettings.fogDensity = Mathf.Lerp(memoria ? fogDensityRealista : fogDensityMemoria, memoria ? fogDensityMemoria : fogDensityRealista, p);

            cameraController.SetOffsetAltura(Mathf.Lerp(offsetInicio, offsetFin, p));
            manoJugador.SetOffsetAltura(Mathf.Lerp(offsetInicio, offsetFin, p));

            yield return null;
        }
    }
}