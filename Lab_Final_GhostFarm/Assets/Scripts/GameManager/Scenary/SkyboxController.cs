using UnityEngine;

public enum CondicionCielo { Manana, Tarde, Noche, Nublado }

[System.Serializable]
public class SkyboxEntry
{
    public CondicionCielo condicion;
    public Material skyboxMaterial;
}

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private SkyboxEntry[] skyboxesDelDia;
    [SerializeField] private Material skyboxMemoria;

    public void SetCondicion(CondicionCielo condicion)
    {
        var entry = System.Array.Find(skyboxesDelDia, e => e.condicion == condicion);
        if (entry != null) Aplicar(entry.skyboxMaterial);
    }

    public void EntrarModoMemoria() => Aplicar(skyboxMemoria);

    private void Aplicar(Material mat)
    {
        RenderSettings.skybox = mat;
        DynamicGI.UpdateEnvironment();
    }
}