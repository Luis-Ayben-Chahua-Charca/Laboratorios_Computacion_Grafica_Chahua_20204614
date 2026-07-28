using System.Collections.Generic;
using UnityEngine;

public class CampoAvena : MonoBehaviour
{
    // NUEVO: singleton, siguiendo el mismo patrón que SceneDirector,
    // MisionManager, InventarioRecursos, etc. Necesario para que StageLoader
    // pueda encontrarlo sin depender de FindObjectOfType.
    public static CampoAvena Instance { get; private set; }

    private const string idMisionPrincipal = "cortar_avena";
    private const string idMisionSecundaria = "cortar_parejo";

    [SerializeField] private int objetivoCorte = 5;

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

    void HandleObjetivoCompletado(string id)
    {
        if (id == "recoger_hoz") IniciarMision();
    }
    public void IniciarMision()
    {
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = idMisionPrincipal, descripcion = $"Cortar avena (0/{objetivoCorte})" }
        });
    }
    public void ReportarCorte()
    {
        // FIX: sin este guard, cortar mechones más allá del objetivo (6to, 7mo...)
        // volvía a entrar en el bloque de abajo cada vez, re-llamando a
        // IniciarMisionSecundaria() y generando objetivos "cortar_parejo"
        // duplicados. El jugador puede seguir cortando avena de más (se ve y
        // se siente bien), simplemente ya no cuenta para la misión.
        if (cortadosMedio >= objetivoCorte)
        {
            InventarioRecursos.Instance.Agregar("Avena");
            return;
        }

        cortadosMedio++;
        InventarioRecursos.Instance.Agregar("Avena");

        if (cortadosMedio == 1)
            flashbackPrimerCorte.Disparar();

        if (cortadosMedio >= objetivoCorte)
        {
            MisionManager.Instance.CompletarObjetivo(idMisionPrincipal);
            flashbackPrimerCorte.Finalizar(); //  acá sale del recuerdo, no por tiempo
            IniciarMisionSecundaria();
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
        // FIX: mismo problema — emparejar mechones cortados de más (fuera de
        // los 5 contados) seguía sumando a corregidosParejo contra un
        // cortadosMedio que ya no representaba el objetivo real, y volvía a
        // llamar CompletarObjetivo sobre una misión ya completada.
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

    // NUEVO: usado por StageLoader para saltar directo a un progreso de
    // corte específico, sin jugar el flujo real. Fija las cuentas internas Y
    // actualiza el visual de los mechones correspondientes (los primeros N
    // como ya cortados), para que el campo se vea consistente con la etapa.
    public void ForzarEstadoDebug(int cortadosMedioObjetivo, bool misionSecundariaActivaObjetivo, int corregidosParejoObjetivo)
    {
        cortadosMedio = Mathf.Clamp(cortadosMedioObjetivo, 0, objetivoCorte);
        misionSecundariaActiva = misionSecundariaActivaObjetivo;
        corregidosParejo = Mathf.Clamp(corregidosParejoObjetivo, 0, cortadosMedio);

        for (int i = 0; i < mechones.Count; i++)
        {
            if (i >= cortadosMedio) { mechones[i].ForzarEstadoDebug(EstadoMechon.Crecido); continue; }
            bool yaParejo = i < corregidosParejo;
            mechones[i].ForzarEstadoDebug(yaParejo ? EstadoMechon.CortadoParejo : EstadoMechon.CortadoMedio);
        }
    }
}