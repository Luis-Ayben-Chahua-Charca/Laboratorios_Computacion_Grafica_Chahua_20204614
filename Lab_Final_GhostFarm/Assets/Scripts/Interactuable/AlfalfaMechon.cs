using UnityEngine;

public enum EstadoMechon { Crecido, Cortado }

public class AlfalfaMechon : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject visualCrecido;
    [SerializeField] private GameObject visualCortado;
    [SerializeField] private CampoAlfalfa campo; // arrastrás acá el padre "CampoAlfalfa"
    public enum EstadoMechon { Crecido, Cortado }

    public EstadoMechon estado = EstadoMechon.Crecido;
    public string TextoInteraccion => "Cortar alfalfa [E]";

    public void Interactuar(GameObject jugador)
    {
        if (estado != EstadoMechon.Crecido) return; // ya cortado, ignora otro click
        Cortar();
    }

    public void Cortar()
    {
        estado = EstadoMechon.Cortado;
        visualCrecido.SetActive(false);
        visualCortado.SetActive(true);

        campo.ReportarCorte();
    }
}