using System.Collections.Generic;
using UnityEngine;

public class InventarioRecursos : MonoBehaviour
{
    public static InventarioRecursos Instance { get; private set; }
    [SerializeField] private HUDController hud;

    private Dictionary<string, int> recursos = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Agregar(string tipo, int cantidad = 1)
    {
        if (!recursos.ContainsKey(tipo)) recursos[tipo] = 0;
        recursos[tipo] += cantidad;
        hud.ActualizarItem(tipo, recursos[tipo]);
    }

    // NUEVO: a diferencia de Agregar (que suma), esto fija la cantidad exacta.
    // Pensado para StageLoader, que necesita dejar el inventario en un estado
    // puntual (ej. "5 de Avena") sin depender de cuánto hubiera antes.
    public void ForzarCantidad(string tipo, int cantidad)
    {
        recursos[tipo] = cantidad;
        hud.ActualizarItem(tipo, cantidad);
    }

    public int Cantidad(string tipo) => recursos.TryGetValue(tipo, out int c) ? c : 0;
}