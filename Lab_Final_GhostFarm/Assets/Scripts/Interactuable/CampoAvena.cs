using System.Collections.Generic;
using UnityEngine;

public class CampoAvena : MonoBehaviour, IEstadoDebug
{
    public static CampoAvena Instance { get; private set; }

    private const MisionId idMisionPrincipal = MisionId.CortarAvena;
    private const MisionId idMisionSecundaria = MisionId.CortarParejo;

    [SerializeField] private int objetivoCorte = 5;

    [Header("Diálogo al completar el corte principal (opcional)")]
    [SerializeField] private DialogoData dialogoAlCompletar;

    private List<AvenaMechon> mechones = new List<AvenaMechon>();
    private FlashbackTrigger flashbackPrimerCorte;

    private int cortadosMedio = 0;
    private int corregidosParejo = 0;
    private bool misionSecundariaActiva = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        mechones.AddRange(GetComponentsInChildren<AvenaMechon>());
        flashbackPrimerCorte = GetComponent<FlashbackTrigger>();
    }

    void OnEnable() => MisionManager.OnObjetivoCompletado += HandleObjetivoCompletado;
    void OnDisable() => MisionManager.OnObjetivoCompletado -= HandleObjetivoCompletado;

    void HandleObjetivoCompletado(MisionId id)
    {
        if (id == MisionId.RecogerHoz) IniciarMision();
    }
    public void IniciarMision()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = idMisionPrincipal, descripcion = $"Cortar avena (0/{objetivoCorte})" }
        });
    }
    public void ReportarCorte()
    {
        if (cortadosMedio >= objetivoCorte)
        {
            InventarioRecursos.Instance.Agregar(TipoRecurso.Avena);
            return;
        }

        cortadosMedio++;
        InventarioRecursos.Instance.Agregar(TipoRecurso.Avena);

        if (cortadosMedio == 1)
            flashbackPrimerCorte.Disparar();

        if (cortadosMedio >= objetivoCorte)
        {
            MisionManager.Instance.CompletarObjetivo(idMisionPrincipal);
            flashbackPrimerCorte.Finalizar();

            // NUEVO: antes esto llamaba a IniciarMisionSecundaria() directo.
            // Ahora, si hay un diálogo asignado, se muestra primero (esperando
            // Espacio) y la secundaria recién aparece cuando el diálogo
            // termina. Si dialogoAlCompletar es null, DialogoController llama
            // al callback de inmediato — mismo comportamiento que antes.
            DialogoController.Instance.MostrarDialogo(dialogoAlCompletar, IniciarMisionSecundaria);
        }
        else
        {
            MisionManager.Instance.ActualizarDescripcion(idMisionPrincipal, $"Cortar avena ({cortadosMedio}/{objetivoCorte})");
        }
    }

    private void IniciarMisionSecundaria()
    {
        misionSecundariaActiva = true;
        MisionManager.Instance.AgregarSecundario(new Objetivo
        {
            id = idMisionSecundaria,
            descripcion = $"Cortar parejo (0/{cortadosMedio})"
        });
    }
    public void ReportarCorteParejo()
    {
        if (!misionSecundariaActiva || MisionSecundariaCompleta()) return;

        corregidosParejo++;
        MisionManager.Instance.ActualizarDescripcion(idMisionSecundaria, $"Cortar parejo ({corregidosParejo}/{cortadosMedio})");

        if (corregidosParejo >= cortadosMedio)
            MisionManager.Instance.CompletarObjetivo(idMisionSecundaria);
    }

    public bool MisionSecundariaCompleta() => !misionSecundariaActiva || corregidosParejo >= cortadosMedio;

    public void EscalarObsesion()
    {
        if (!misionSecundariaActiva || MisionSecundariaCompleta()) return;
        MisionManager.Instance.EscalarAPrincipal(idMisionSecundaria);
    }

    public void AplicarEstadoDebug(StageData etapa)
    {
        var progreso = etapa.avena;

        cortadosMedio = Mathf.Clamp(progreso.cortadosMedio, 0, objetivoCorte);
        misionSecundariaActiva = progreso.misionSecundariaActiva;
        corregidosParejo = Mathf.Clamp(progreso.corregidosParejo, 0, cortadosMedio);

        for (int i = 0; i < mechones.Count; i++)
        {
            if (i >= cortadosMedio) { mechones[i].ForzarEstadoDebug(EstadoMechon.Crecido); continue; }
            bool yaParejo = i < corregidosParejo;
            mechones[i].ForzarEstadoDebug(yaParejo ? EstadoMechon.CortadoParejo : EstadoMechon.CortadoMedio);
        }

        // NUEVO: en vez de depender de que quien arma el StageData tipee a
        // mano un texto de descripción que coincida con el que generaría el
        // juego real (ej. "Cortar avena (3/5)"), recalculamos acá con el
        // mismo formato exacto que usa ReportarCorte/ReportarCorteParejo.
        // Esto hace que el campo "descripcion" de esas entradas en el
        // StageData sea puramente cosmético/ignorable para estas dos
        // misiones puntuales — StageLoader ya creó la fila con ForzarEstado
        // antes de llegar acá, así que ActualizarDescripcion la encuentra y
        // la corrige con el valor real.
        if (cortadosMedio < objetivoCorte)
            MisionManager.Instance.ActualizarDescripcion(idMisionPrincipal, $"Cortar avena ({cortadosMedio}/{objetivoCorte})");

        if (misionSecundariaActiva)
            MisionManager.Instance.ActualizarDescripcion(idMisionSecundaria, $"Cortar parejo ({corregidosParejo}/{cortadosMedio})");
    }
}