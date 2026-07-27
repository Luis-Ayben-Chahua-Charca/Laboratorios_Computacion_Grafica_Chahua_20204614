using UnityEngine;

public class TerrenoDualStyle : MonoBehaviour, IDualStyle
{
    [Header("Texturas del terreno (mismo orden que pintaste)")]
    [SerializeField] private TerrainLayer[] capasCartoon;
    [SerializeField] private TerrainLayer[] capasRealista;

    [Header("Árboles pintados (mismo orden que los prototipos)")]
    [SerializeField] private GameObject[] prefabsArbolesCartoon;
    [SerializeField] private GameObject[] prefabsArbolesRealista;

    private Terrain terreno;

    void Awake() => terreno = GetComponent<Terrain>();

    public void SetModo(bool memoria)
    {
        var data = terreno.terrainData;

        data.terrainLayers = memoria ? capasCartoon : capasRealista;

        TreePrototype[] protos = data.treePrototypes;
        for (int i = 0; i < protos.Length; i++)
            protos[i].prefab = memoria ? prefabsArbolesCartoon[i] : prefabsArbolesRealista[i];
        data.treePrototypes = protos;

        terreno.Flush();
    }
}