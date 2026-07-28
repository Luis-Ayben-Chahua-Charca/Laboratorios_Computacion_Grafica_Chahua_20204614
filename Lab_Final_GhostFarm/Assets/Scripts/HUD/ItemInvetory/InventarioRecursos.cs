using System.Collections.Generic;
using UnityEngine;

public class InventarioRecursos : MonoBehaviour
{
    public static InventarioRecursos Instance { get; private set; }
    [SerializeField] private HUDController hud;

    // FIX: antes era Dictionary<string, int>. Con TipoRecurso como enum,
    // la clave ahora es TipoRecurso — sin typos posibles.
    private Dictionary<TipoRecurso, int> recursos = new Dictionary<TipoRecurso, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Agregar(TipoRecurso tipo, int cantidad = 1)
    {
        if (!recursos.ContainsKey(tipo)) recursos[tipo] = 0;
        recursos[tipo] += cantidad;
        hud.ActualizarItem(tipo, recursos[tipo]);
    }

    // Usado por StageLoader: a diferencia de Agregar (que suma), esto fija
    // la cantidad exacta, sin depender de cuánto hubiera antes.
    public void ForzarCantidad(TipoRecurso tipo, int cantidad)
    {
        recursos[tipo] = cantidad;
        hud.ActualizarItem(tipo, cantidad);
    }

    public int Cantidad(TipoRecurso tipo) => recursos.TryGetValue(tipo, out int c) ? c : 0;
}