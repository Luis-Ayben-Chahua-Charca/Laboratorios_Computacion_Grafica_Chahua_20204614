using UnityEngine;

public enum EstadoMechon { Crecido, Cortado }

public class AlfalfaMechon : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject visualCrecido;
    [SerializeField] private GameObject visualCortado;
    [SerializeField] private CampoAlfalfa campo;

    public EstadoMechon estado = EstadoMechon.Crecido;
    public string TextoInteraccion => "Cortar alfalfa [E]";

    public void Interactuar(GameObject jugador)
    {
        if (estado != EstadoMechon.Crecido) return;
        Cortar();
    }

    public void Cortar()
    {
        if (campo == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene asignado el campo Campo — revisar Inspector.", this);
            return;
        }

        estado = EstadoMechon.Cortado;
        visualCrecido.SetActive(false);
        visualCortado.SetActive(true);
        campo.ReportarCorte();
    }
}
