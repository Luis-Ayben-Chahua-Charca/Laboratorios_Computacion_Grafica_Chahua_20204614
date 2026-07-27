using System.Collections.Generic;
using UnityEngine;

public class CampoAvena : MonoBehaviour
{
    private const string idMisionPrincipal = "cortar_avena";
    private const string idMisionSecundaria = "cortar_parejo";

    [SerializeField] private int objetivoCorte = 5;

    private List<AvenaMechon> mechones = new List<AvenaMechon>();
    private FlashbackTrigger flashbackPrimerCorte;

    private int cortadosMedio = 0;
    private int corregidosParejo = 0;
    private bool misionSecundariaActiva = false;


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
        if (!misionSecundariaActiva) return;
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
}