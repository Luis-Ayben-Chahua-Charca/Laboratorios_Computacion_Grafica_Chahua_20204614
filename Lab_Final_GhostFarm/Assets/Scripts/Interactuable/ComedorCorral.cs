using System.Collections.Generic;
using UnityEngine;

// Usa el mismo sistema de interacción por raycast que ya tenés (Interactor +
// tecla E) — el jugador ya trae 5 de Avena en el inventario abstracto (de
// cortar el campo), y acá simplemente las "deposita" de a una, igual que
// CampoAvena cuenta cortes.
public class ComedoroCorral : MonoBehaviour, IInteractable, IEstadoDebug
{
    private const MisionId idMision = MisionId.EntregarPasto;
    [SerializeField] private int objetivoPasto = 5;

    // NUEVO: en vez de solo un contador de texto, cada unidad de pasto
    // depositada activa uno de estos GameObjects — pre-colocados a mano en
    // el comedero, todos inactivos al empezar. Da la sensación de "se va
    // llenando" sin necesidad de física ni de instanciar nada en runtime.
    [Header("Visual: pasto ya depositado (uno por unidad, todos inactivos al inicio)")]
    [SerializeField] private GameObject[] pastoVisuals;

    [Header("Modo recuerdo al depositar el primero")]
    [SerializeField] private FlashbackTrigger flashbackPrimerPasto;

    private bool misionActiva = false;
    private int pastosDejados = 0;

    public string TextoInteraccion => $"Dejar pasto [E] ({pastosDejados}/{objetivoPasto})";

    void OnEnable() => MisionManager.OnObjetivoCompletado += HandleObjetivoCompletado;
    void OnDisable() => MisionManager.OnObjetivoCompletado -= HandleObjetivoCompletado;

    // FIX: antes se disparaba con CortarParejo. Ahora hay una misión
    // intermedia (AlmacenHerramientas / DejarHoz) entre cortar la avena y
    // empezar a alimentar a los animales, así que esperamos a que esa
    // termine primero.
    void HandleObjetivoCompletado(MisionId id)
    {
        if (id == MisionId.DejarHoz) IniciarMision();
    }

    public void IniciarMision()
    {
        misionActiva = true;
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = idMision, descripcion = $"Dejar pasto en el comedero (0/{objetivoPasto})" }
        });
    }

    public void Interactuar(GameObject jugador)
    {
        // Guard: mismo patrón que CampoAvena.ReportarCorte — evita que
        // seguir interactuando después de completar la misión reabra o
        // duplique nada.
        if (!misionActiva || pastosDejados >= objetivoPasto) return;
        if (InventarioRecursos.Instance.Cantidad(TipoRecurso.Avena) <= 0) return;

        InventarioRecursos.Instance.Consumir(TipoRecurso.Avena, 1);
        pastosDejados++;

        if (pastoVisuals != null && pastosDejados - 1 < pastoVisuals.Length)
            pastoVisuals[pastosDejados - 1].SetActive(true);

        // NUEVO: el modo recuerdo arranca en el primer pasto depositado, no
        // al cortar la avena (ese ya tiene su propio flashback en CampoAvena).
        // salidaManual debe estar tildado en este FlashbackData — el que lo
        // termina es CercaRota, al detectar la cerca por raycast.
        if (pastosDejados == 1)
        {
            if (flashbackPrimerPasto == null)
                Debug.LogWarning("ComedoroCorral: falta asignar Flashback Primer Pasto — el modo recuerdo no se va a activar", this);
            else
                flashbackPrimerPasto.Disparar();
        }

        if (pastosDejados >= objetivoPasto)
            MisionManager.Instance.CompletarObjetivo(idMision);
        else
            MisionManager.Instance.ActualizarDescripcion(idMision, $"Dejar pasto en el comedero ({pastosDejados}/{objetivoPasto})");
    }

    public void AplicarEstadoDebug(StageData etapa)
    {
        pastosDejados = Mathf.Clamp(etapa.corral.pastosDejados, 0, objetivoPasto);
        misionActiva = pastosDejados < objetivoPasto;

        if (pastoVisuals != null)
            for (int i = 0; i < pastoVisuals.Length; i++)
                pastoVisuals[i].SetActive(i < pastosDejados);

        if (pastosDejados < objetivoPasto)
            MisionManager.Instance.ActualizarDescripcion(idMision, $"Dejar pasto en el comedero ({pastosDejados}/{objetivoPasto})");
    }
}