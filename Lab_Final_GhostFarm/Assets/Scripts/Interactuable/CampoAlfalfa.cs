using System.Collections.Generic;
using UnityEngine;

public class CampoAlfalfa : MonoBehaviour
{
    private const string idMision = "cortar_alfalfa";
    private List<AlfalfaMechon> mechones = new List<AlfalfaMechon>();
    private int cortados = 0;

    void Start()
    {
        mechones.AddRange(GetComponentsInChildren<AlfalfaMechon>());
        MisionManager.Instance.IniciarMisionPrincipal(new List<Objetivo> {
            new Objetivo { id = idMision, descripcion = $"Cortar avena (0/{mechones.Count})" }
        });
    }

    public void ReportarCorte()
    {
        cortados++;
        InventarioRecursos.Instance.Agregar("Avena");

        if (cortados >= mechones.Count)
            MisionManager.Instance.CompletarObjetivo(idMision);
        else
            MisionManager.Instance.ActualizarDescripcion(idMision, $"Cortar avena ({cortados}/{mechones.Count})");
    }
}