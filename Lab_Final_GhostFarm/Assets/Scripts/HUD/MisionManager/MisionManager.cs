using System.Collections.Generic;
using UnityEngine;

public class MisionManager : MonoBehaviour
{
    public static MisionManager Instance { get; private set; }
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

    public void ActualizarDescripcion(string id, string nuevaDescripcion)
    {
        var obj = principales.Find(o => o.id == id) ?? secundarios.Find(o => o.id == id);
        if (obj == null) return;
        obj.descripcion = nuevaDescripcion;
        hud.ActualizarTextoFila(id, nuevaDescripcion);
    }

    public void CompletarObjetivo(string id)
    {
        var obj = principales.Find(o => o.id == id);
        if (obj != null) { obj.completado = true; hud.MarcarCompletado(id, esSecundario: false); return; }

        obj = secundarios.Find(o => o.id == id);
        if (obj != null) { obj.completado = true; hud.MarcarCompletado(id, esSecundario: true); }
    }
}