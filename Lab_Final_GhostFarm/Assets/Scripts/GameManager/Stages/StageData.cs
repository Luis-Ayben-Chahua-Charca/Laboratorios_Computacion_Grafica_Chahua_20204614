using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjetivoInicial
{
    public MisionId id;
    public string descripcion;
}

[System.Serializable]
public class ItemInicial
{
    public TipoRecurso tipo;
    public int cantidad;
}

// NUEVO: agrupar el progreso de la escena de Avena en su propia clase
// (en vez de campos sueltos directo en StageData) hace que, cuando sumes
// ProgresoCerca / ProgresoCocina / ProgresoLampara más adelante, el
// Inspector se mantenga ordenado por secciones plegables, y cada sistema
// de escena solo necesita mirar su propio bloque.
[System.Serializable]
public class ProgresoAvena
{
    public bool jugadorTieneHoz = false;
    public int cortadosMedio = 0;
    public int corregidosParejo = 0;
    public bool misionSecundariaActiva = false;
}

[CreateAssetMenu(fileName = "NuevaEtapa", menuName = "Granja/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Identificación (solo para vos, no se usa en runtime)")]
    public string nombre;

    [Header("Jugador")]
    public Vector3 posicionJugador;
    public Vector3 rotacionJugadorEuler;

    [Header("Tiempo del día")]
    public CondicionCielo condicionCielo = CondicionCielo.Manana;

    [Header("Misiones (tal como deberían verse en el HUD al llegar acá)")]
    public List<ObjetivoInicial> misionesPrincipales = new List<ObjetivoInicial>();
    public List<ObjetivoInicial> misionesSecundarias = new List<ObjetivoInicial>();

    [Header("Inventario")]
    public List<ItemInicial> itemsIniciales = new List<ItemInicial>();

    [Header("Progreso específico — Escena de Avena")]
    public ProgresoAvena avena = new ProgresoAvena();

    // A futuro: cuando armes la escena de la cerca/cocina/lámpara, sumás acá
    // un campo "public ProgresoCerca cerca = new ProgresoCerca();" siguiendo
    // el mismo patrón. StageLoader no necesita tocarse — el sistema nuevo
    // (ej. CampoCerca) implementa IEstadoDebug y lee etapa.cerca por su cuenta.
}