using UnityEngine;

public enum CondicionCielo { Manana, Tarde, Noche, Nublado }

[System.Serializable]
public class SkyboxEntry
{
    public CondicionCielo condicion;
    public Material skyboxMaterial;

    [Header("Luz direccional para esta condición")]
    public Color colorLuz = Color.white;
    public Vector3 rotacionLuz;
}

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private SkyboxEntry[] skyboxesDelDia;
    [SerializeField] private Material skyboxMemoria;
    [SerializeField] private Light solDireccional;
    [SerializeField] private CondicionCielo condicionInicial = CondicionCielo.Manana;

    // FIX: antes VisualModeController fotografiaba una sola vez (en su propio
    // Awake) el color/rotación de la luz y los trataba como "el" realista fijo
    // para todo el juego. Eso ignoraba que el juego tiene distintos momentos
    // del día (Mañana/Tarde/Noche) con luz distinta. Ahora SkyboxController es
    // la única fuente de verdad: guarda cuál es la condición actual y expone
    // sus valores de luz, para que quien los necesite (VisualModeController)
    // los consulte en el momento, no una foto vieja.
    public CondicionCielo CondicionActual { get; private set; }
    public Color ColorLuzActual { get; private set; }
    public Quaternion RotacionLuzActual { get; private set; }

    // FIX: esto corre en Awake (no en Start) para garantizar que el skybox y
    // la luz de la condición inicial (ej. Mañana del Día 1) ya estén
    // aplicados antes de que cualquier otro Start() (como el de
    // VisualModeController, que consulta ColorLuzActual/RotacionLuzActual)
    // se ejecute. Awake() de todos los objetos siempre corre antes que
    // cualquier Start(), sin importar el orden entre scripts.
    void Awake() => SetCondicion(condicionInicial);

    public void SetCondicion(CondicionCielo condicion)
    {
        var entry = System.Array.Find(skyboxesDelDia, e => e.condicion == condicion);
        if (entry == null)
        {
            Debug.LogWarning($"SkyboxController: no hay SkyboxEntry configurada para la condición {condicion}", this);
            return;
        }

        CondicionActual = condicion;
        ColorLuzActual = entry.colorLuz;
        RotacionLuzActual = Quaternion.Euler(entry.rotacionLuz);

        Aplicar(entry.skyboxMaterial);

        if (solDireccional != null)
        {
            solDireccional.color = ColorLuzActual;
            solDireccional.transform.rotation = RotacionLuzActual;
        }
    }

    public void EntrarModoMemoria() => Aplicar(skyboxMemoria);

    // FIX: no existía ninguna forma de volver al skybox del día después de un
    // flashback — EntrarModoMemoria() cambiaba el material, pero nada lo
    // revertía. VisualModeController.SalirModoMemoria() ahora llama a esto.
    public void RestaurarUltimaCondicion() => SetCondicion(CondicionActual);

    private void Aplicar(Material mat)
    {
        RenderSettings.skybox = mat;
        DynamicGI.UpdateEnvironment();
    }
}