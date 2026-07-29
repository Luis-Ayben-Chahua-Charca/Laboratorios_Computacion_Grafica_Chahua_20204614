using System.Collections.Generic;
using UnityEngine;

// Misión intermedia entre "cortar avena" y "dejar pasto en el comedero":
// obliga al jugador a devolver la hoz al almacén antes de seguir. Genérico
// vía nombreHerramienta/idMision — a futuro sirve para cualquier otra
// herramienta que deba guardarse (ej. el encendedor en el taller).
public class AlmacenHerramientas : MonoBehaviour, IInteractable, IEstadoDebug
{
    [SerializeField] private Transform puntoDeGuardado;
    [SerializeField] private ObjetoId idHerramienta = ObjetoId.Hoz;
    [SerializeField] private MisionId idMision = MisionId.DejarHoz;
    [SerializeField] private MisionId idMisionQueLoDispara = MisionId.CortarParejo;

    public string TextoInteraccion => $"Dejar {idHerramienta} [E]";

    void OnEnable() => MisionManager.OnObjetivoCompletado += HandleObjetivoCompletado;
    void OnDisable() => MisionManager.OnObjetivoCompletado -= HandleObjetivoCompletado;

    void HandleObjetivoCompletado(MisionId id)
    {
        if (id == idMisionQueLoDispara) IniciarMision();
    }

    public void IniciarMision()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = idMision, descripcion = $"Dejar {idHerramienta} en el almacén" }
        });
    }

    public void Interactuar(GameObject jugador)
    {
        var mano = jugador.GetComponent<ManoJugador>();
        if (mano == null || mano.ObjetoActual == null || mano.ObjetoActual.Id != idHerramienta) return;

        mano.DejarEn(puntoDeGuardado);
        MisionManager.Instance.CompletarObjetivo(idMision);
    }

    // No hay nada que sincronizar visualmente acá — si el jugador ya tiene
    // la herramienta en mano o no al saltar de etapa lo maneja StageLoader
    // (vía StageData.objetosEnMano), no este script.
    public void AplicarEstadoDebug(StageData etapa) { }
}