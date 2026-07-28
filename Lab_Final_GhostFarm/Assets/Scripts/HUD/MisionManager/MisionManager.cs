using System.Collections.Generic;
using UnityEngine;

public class MisionManager : MonoBehaviour
{
    public static MisionManager Instance { get; private set; }
    public static event System.Action<MisionId> OnObjetivoCompletado;

    [SerializeField] private HUDController hud;

    private List<Objetivo> principales = new List<Objetivo>();
    private List<Objetivo> secundarios = new List<Objetivo>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void IniciarMisionPrincipal(List<Objetivo> objetivos)
    {
        principales = objetivos;
        hud.RenderizarPrincipales(principales);
    }

    public void AgregarSecundario(Objetivo objetivo)
    {
        secundarios.Add(objetivo);
        hud.AgregarFilaSecundaria(objetivo);
    }

    // Usado por StageLoader para saltar directo a un estado de misiones
    // específico. No dispara OnObjetivoCompletado — a propósito, ya que
    // estamos fijando un estado, no completando nada.
    public void ForzarEstado(List<Objetivo> nuevasPrincipales, List<Objetivo> nuevasSecundarias)
    {
        principales = nuevasPrincipales ?? new List<Objetivo>();
        secundarios = nuevasSecundarias ?? new List<Objetivo>();
        hud.RenderizarPrincipales(principales);
        hud.RenderizarSecundarios(secundarios);
    }

    public void ActualizarDescripcion(MisionId id, string nuevaDescripcion)
    {
        var obj = principales.Find(o => o.id == id) ?? secundarios.Find(o => o.id == id);
        if (obj == null) return;
        obj.descripcion = nuevaDescripcion;
        hud.ActualizarTextoFila(id, nuevaDescripcion);
    }

    public void CompletarObjetivo(MisionId id)
    {
        var obj = principales.Find(o => o.id == id);
        if (obj != null)
        {
            obj.completado = true;
            hud.MarcarCompletado(id, esSecundario: false);
            OnObjetivoCompletado?.Invoke(id);
            return;
        }

        obj = secundarios.Find(o => o.id == id);
        if (obj != null)
        {
            obj.completado = true;
            hud.MarcarCompletado(id, esSecundario: true);
            OnObjetivoCompletado?.Invoke(id);
            return;
        }

        Debug.LogWarning($"MisionManager.CompletarObjetivo: no se encontró ningún objetivo con id '{id}' en principales ni secundarios (¿falta agregarlo antes?)", this);
    }

    public void EscalarAPrincipal(MisionId id)
    {
        var obj = secundarios.Find(o => o.id == id);
        if (obj == null) return;
        secundarios.Remove(obj);
        principales.Add(obj);
        hud.EscalarFilaAPrincipal(id);
    }
}