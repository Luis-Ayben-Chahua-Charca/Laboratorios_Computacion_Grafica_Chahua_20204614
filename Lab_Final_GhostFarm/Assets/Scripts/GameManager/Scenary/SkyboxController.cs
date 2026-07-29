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
    public static SkyboxController Instance { get; private set; }

    [SerializeField] private SkyboxEntry[] skyboxesDelDia;
    [SerializeField] private Material skyboxMemoria;
    [SerializeField] private Light solDireccional;
    [SerializeField] private CondicionCielo condicionInicial = CondicionCielo.Manana;

    public CondicionCielo CondicionActual { get; private set; }
    public Color ColorLuzActual { get; private set; }
    public Quaternion RotacionLuzActual { get; private set; }

    // NUEVO: singleton, consistente con el resto de managers del proyecto —
    // así CercaRota y AlmacenHerramientas pueden llamar a SetCondicion sin
    // necesitar una referencia serializada.
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SetCondicion(condicionInicial);
    }

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