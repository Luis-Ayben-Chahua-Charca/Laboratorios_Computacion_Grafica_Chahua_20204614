using System.Collections.Generic;
using UnityEngine;

public class CercaRota : MonoBehaviour, IInteractable, IEstadoDebug
{
    [Header("Visuales (swap roto / reparado, no dual cartoon/realista)")]
    [SerializeField] private GameObject visualRota;
    [SerializeField] private GameObject visualReparada;

    [Header("Detección por raycast")]
    [SerializeField] private Interactor interactorJugador;
    [SerializeField] private FlashbackTrigger flashbackPasto; // el mismo que dispara ComedoroCorral

    [Header("Diálogos")]
    [SerializeField] private DialogoData dialogoRevisarCerca;  // al completar EntregarPasto
    [SerializeField] private DialogoData dialogoRepararCerca;  // al detectar la cerca por raycast
    [SerializeField] private DialogoData dialogoAlReparar;     // al terminar de repararla

    private bool dialogoRevisarMostrado = false;
    private bool cercaDetectada = false;
    private bool cercaReparada = false;

    public string TextoInteraccion => "Reparar cerca [E]";

    void OnEnable() => MisionManager.OnObjetivoCompletado += HandleObjetivoCompletado;
    void OnDisable() => MisionManager.OnObjetivoCompletado -= HandleObjetivoCompletado;

    void HandleObjetivoCompletado(MisionId id)
    {
        if (id == MisionId.EntregarPasto) MostrarDialogoRevisarCerca();
        else if (id == MisionId.BuscarCuerda) IniciarMisionRepararCerca();
    }

    // NUEVO: faltaba esto — BuscarCuerda se completaba solo (automático vía
    // ManoJugador.Equipar al agarrar la cuerda), pero nada mostraba la
    // siguiente misión en el HUD. Sin esto, el jugador se quedaba sin
    // ninguna misión principal visible después de agarrar la cuerda, aunque
    // Interactuar() ya funcionara si tocaba la cerca igual.
    private void IniciarMisionRepararCerca()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = MisionId.RepararCerca, descripcion = "Reparar la cerca" }
        });
    }

    private void MostrarDialogoRevisarCerca()
    {
        if (dialogoRevisarMostrado) return;
        dialogoRevisarMostrado = true;
        // NUEVO: este diálogo NO dispara nada más — solo empuja al jugador a
        // ir a mirar la cerca por su cuenta. Lo que sigue de la cadena
        // depende de que el raycast la detecte (ver Update).
        DialogoController.Instance.MostrarDialogo(dialogoRevisarCerca, null);
    }

    void Update()
    {
        // Solo empezamos a chequear la detección una vez que el jugador ya
        // terminó de dejar el pasto — evita que mirar la cerca "de
        // casualidad" antes de tiempo dispare el resto de la cadena fuera
        // de orden.
        if (!dialogoRevisarMostrado || cercaDetectada || cercaReparada) return;
        if (interactorJugador == null) return;

        if (interactorJugador.Actual == (IInteractable)this)
        {
            cercaDetectada = true;
            OnCercaDetectada();
        }
    }

    private void OnCercaDetectada()
    {
        // NUEVO: al detectar la cerca, cambiamos la condición del día a
        // Tarde ANTES de salir del modo recuerdo — así el blend de luz va
        // directo de "memoria" a "tarde", sin pasar por "mañana" en el medio.
        SkyboxController.Instance.SetCondicion(CondicionCielo.Tarde);
        flashbackPasto.Finalizar();
        DialogoController.Instance.MostrarDialogo(dialogoRepararCerca, IniciarMisionBuscarCuerda);
    }

    private void IniciarMisionBuscarCuerda()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = MisionId.BuscarCuerda, descripcion = "Buscar cuerda en el corral" }
        });
    }

    // NOTA: BuscarCuerda se completa solo — ManoJugador.Equipar() lo hace
    // automáticamente en cuanto el jugador agarra el ObjetoAgarrable de la
    // Cuerda (CompletaMisionAlRecoger=true, MisionAlRecoger=BuscarCuerda).

    public void Interactuar(GameObject jugador)
    {
        if (cercaReparada) return;

        var mano = jugador.GetComponent<ManoJugador>();
        if (mano == null || mano.ObjetoActual == null || mano.ObjetoActual.Id != ObjetoId.Cuerda) return;

        Reparar(mano);
    }

    private void Reparar(ManoJugador mano)
    {
        cercaReparada = true;
        visualRota.SetActive(false);
        visualReparada.SetActive(true);
        mano.ConsumirObjetoActual();

        MisionManager.Instance.CompletarObjetivo(MisionId.RepararCerca);
        DialogoController.Instance.MostrarDialogo(dialogoAlReparar, IniciarMisionIrCocina);
    }

    private void IniciarMisionIrCocina()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = MisionId.IrCocina, descripcion = "Ir a la cocina" }
        });
    }

    public void AplicarEstadoDebug(StageData etapa)
    {
        cercaReparada = etapa.corral.cercaReparada;
        dialogoRevisarMostrado = etapa.corral.dialogoCercaMostrado || cercaReparada;
        cercaDetectada = etapa.corral.cercaDetectada || cercaReparada;

        visualRota.SetActive(!cercaReparada);
        visualReparada.SetActive(cercaReparada);
    }
}