using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private float bajarAlturaEnMemoria = 0.4f;

    [Header("Fog")]
    [SerializeField] private Color fogColorRealista;
    [SerializeField] private Color fogColorMemoria;
    [SerializeField] private float fogDensityRealista = 0.02f;
    [SerializeField] private float fogDensityMemoria = 0.01f;

    private Color colorLuzOriginal;
    private Quaternion rotacionSolOriginal;
    private IDualStyle[] objetosDuales;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        objetosDuales = FindObjectsByType<DualStyleObject>(FindObjectsInactive.Include) is var _ ? GetDualStyles() : null;
        colorLuzOriginal = solDireccional.color;
        rotacionSolOriginal = solDireccional.transform.rotation;
    }

    private IDualStyle[] GetDualStyles()
    {
        var lista = new System.Collections.Generic.List<IDualStyle>();
        lista.AddRange(FindObjectsByType<DualStyleObject>(FindObjectsInactive.Include));
        lista.AddRange(FindObjectsByType<DualStyleObject>(FindObjectsInactive.Include));
        return lista.ToArray();
    }

    public void EntrarModoMemoria(bool instantaneo = false)
    {

        if (skyboxController == null) Debug.LogError("Falta asignar skyboxController", this);
        if (solDireccional == null) Debug.LogError("Falta asignar solDireccional", this);
        if (volumeRealista == null) Debug.LogError("Falta asignar volumeRealista", this);
        if (volumeMemoria == null) Debug.LogError("Falta asignar volumeMemoria", this);
        if (cameraController == null) Debug.LogError("Falta asignar cameraController", this);

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
        foreach (var obj in objetosDuales) obj.SetModo(false);

        if (instantaneo) AplicarLuzInstantanea(false);
        else StartCoroutine(BlendLuz(false));
    }

    private void AplicarLuzInstantanea(bool memoria)
    {
        solDireccional.color = memoria ? colorLuzMemoria : colorLuzOriginal;
        solDireccional.transform.rotation = memoria ? Quaternion.Euler(rotacionSolMemoria) : rotacionSolOriginal;
        volumeMemoria.weight = memoria ? 1f : 0f;
        volumeRealista.weight = memoria ? 0f : 1f;

        cameraController.SetOffsetAltura(memoria ? -bajarAlturaEnMemoria : 0f);
    }

    private IEnumerator BlendLuz(bool memoria)
    {
        float t = 0;
        Color colorInicio = solDireccional.color;
        Color colorFin = memoria ? colorLuzMemoria : colorLuzOriginal;
        Quaternion rotInicio = solDireccional.transform.rotation;
        Quaternion rotFin = memoria ? Quaternion.Euler(rotacionSolMemoria) : rotacionSolOriginal;
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

            yield return null;
        }
    }
}