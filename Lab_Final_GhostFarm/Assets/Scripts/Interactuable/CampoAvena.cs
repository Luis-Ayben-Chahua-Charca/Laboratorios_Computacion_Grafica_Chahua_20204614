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

    // NUEVO: reemplaza la lógica de EscalarObsesion() que antes vivía acá
    // adentro, y a SalidaCampoAvena.cs (que ya no hace falta). Este
    // componente vive en el mismo GameObject que el Collider trigger del
    // perímetro del campo — ver instrucciones de configuración.
    [SerializeField] private ComponenteObsesion obsesion;

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
        obsesion.Activar();
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
        {
            MisionManager.Instance.CompletarObjetivo(idMisionSecundaria);
            obsesion.MarcarCompleta();
        }
    }

    public bool MisionSecundariaCompleta() => !misionSecundariaActiva || corregidosParejo >= cortadosMedio;

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

        // NUEVO: sincroniza el ComponenteObsesion con el progreso de la
        // etapa, para que salir del área inmediatamente después de un salto
        // se comporte igual que si se hubiera jugado normalmente hasta ahí.
        if (!misionSecundariaActiva)
        {
            obsesion.Desactivar();
        }
        else
        {
            obsesion.Activar();
            if (MisionSecundariaCompleta()) obsesion.MarcarCompleta();
        }

        if (cortadosMedio < objetivoCorte)
            MisionManager.Instance.ActualizarDescripcion(idMisionPrincipal, $"Cortar avena ({cortadosMedio}/{objetivoCorte})");

        if (misionSecundariaActiva)
            MisionManager.Instance.ActualizarDescripcion(idMisionSecundaria, $"Cortar parejo ({corregidosParejo}/{cortadosMedio})");
    }
}