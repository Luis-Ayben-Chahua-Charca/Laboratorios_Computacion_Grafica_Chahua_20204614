using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjetivoInicial
{
    public string id;
    public string descripcion;
}

[System.Serializable]
public class ItemInicial
{
    public string tipo;
    public int cantidad;
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
    public bool jugadorTieneHoz = false;
    public int avenaCortadosMedio = 0;
    public int avenaCorregidosParejo = 0;
    public bool avenaMisionSecundariaActiva = false;

    // A futuro: cuando armes la escena de la cerca/cocina/lámpara, sumás acá
    // un bloque [Header("Progreso específico — Cerca")] con sus propios campos,
    // siguiendo el mismo patrón. StageLoader.CargarEtapa() es el único lugar
    // que necesita enterarse de los campos nuevos.
}