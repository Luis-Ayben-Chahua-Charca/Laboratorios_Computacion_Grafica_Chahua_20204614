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
    public int cortadosMedio = 0;
    public int corregidosParejo = 0;
    public bool misionSecundariaActiva = false;
}

// NUEVO: progreso de la escena del corral (comedero + cerca), siguiendo el
// mismo patrón que ProgresoAvena.
[System.Serializable]
public class ProgresoCorral
{
    public int pastosDejados = 0;
    public bool dialogoCercaMostrado = false;
    public bool cercaDetectada = false;
    public bool cercaReparada = false;
}

[CreateAssetMenu(fileName = "NuevaEtapa", menuName = "Granja/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Identificación (solo para vos, no se usa en runtime)")]
    public string nombre;

    [Header("Jugador")]
    public Vector3 posicionJugador;
    public Vector3 rotacionJugadorEuler;

    // Lista genérica de qué objetos (identificados por ObjetoId, no por la
    // misión que completan) el jugador ya tiene en mano al llegar a esta
    // etapa. StageLoader busca los ObjetoAgarrable de la escena cuyo Id
    // coincida con alguno de esta lista.
    [Header("Objetos que el jugador ya tiene en mano")]
    public List<ObjetoId> objetosEnMano = new List<ObjetoId>();

    [Header("Tiempo del día")]
    public CondicionCielo condicionCielo = CondicionCielo.Manana;

    [Header("Misiones (tal como deberían verse en el HUD al llegar acá)")]
    public List<ObjetivoInicial> misionesPrincipales = new List<ObjetivoInicial>();
    public List<ObjetivoInicial> misionesSecundarias = new List<ObjetivoInicial>();

    [Header("Inventario")]
    public List<ItemInicial> itemsIniciales = new List<ItemInicial>();

    [Header("Progreso específico — Escena de Avena")]
    public ProgresoAvena avena = new ProgresoAvena();

    [Header("Progreso específico — Corral (comedero + cerca)")]
    public ProgresoCorral corral = new ProgresoCorral();

    // A futuro: cuando armes la cocina/lámpara, sumás acá un campo
    // "public ProgresoCocina cocina = new ProgresoCocina();" siguiendo el
    // mismo patrón. StageLoader no necesita tocarse — el sistema nuevo
    // implementa IEstadoDebug y lee etapa.cocina por su cuenta.
}