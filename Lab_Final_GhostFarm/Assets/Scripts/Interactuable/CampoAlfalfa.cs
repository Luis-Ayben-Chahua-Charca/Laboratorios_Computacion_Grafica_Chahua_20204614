using System.Collections.Generic;
using UnityEngine;

public class CampoAlfalfa : MonoBehaviour
{
    private List<AlfalfaMechon> mechones = new List<AlfalfaMechon>();
    private int cortados = 0;

    void Start()
    {
        mechones.AddRange(GetComponentsInChildren<AlfalfaMechon>());
        MisionManager.Instance.SetTarea($"Cortar alfalfa (0/{mechones.Count})"); // <- acá arranca la misión
    }

    public void ReportarCorte()
    {
        cortados++;
        MisionManager.Instance.SetTarea($"Cortar alfalfa ({cortados}/{mechones.Count})"); // <- acá se actualiza
        InventarioRecursos.Instance.Agregar("Alfalfa"); // <- acá se suma al inventario

        if (cortados >= mechones.Count)
            MisionManager.Instance.LimpiarTarea(); // opcional: acá dispararías la próxima tarea
    }
}