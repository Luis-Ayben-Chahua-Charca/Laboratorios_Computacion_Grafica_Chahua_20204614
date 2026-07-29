using System.Collections.Generic;
using UnityEngine;

public class InventarioRecursos : MonoBehaviour
{
    public static InventarioRecursos Instance { get; private set; }
    [SerializeField] private HUDController hud;

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

    // NUEVO: opuesto de Agregar. Usado por ComedoroCorral al depositar pasto
    // — descuenta sin bajar de 0, y refleja el cambio en el HUD igual que Agregar.
    public void Consumir(TipoRecurso tipo, int cantidad = 1)
    {
        int actual = Cantidad(tipo);
        int nuevo = Mathf.Max(0, actual - cantidad);
        recursos[tipo] = nuevo;
        hud.ActualizarItem(tipo, nuevo);
    }

    public void ForzarCantidad(TipoRecurso tipo, int cantidad)
    {
        recursos[tipo] = cantidad;
        hud.ActualizarItem(tipo, cantidad);
    }

    public int Cantidad(TipoRecurso tipo) => recursos.TryGetValue(tipo, out int c) ? c : 0;
}