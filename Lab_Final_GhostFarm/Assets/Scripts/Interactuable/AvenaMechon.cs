using UnityEngine;

public enum EstadoMechon { Crecido, CortadoMedio, CortadoParejo }

public class AvenaMechon : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject visualCrecido;
    [SerializeField] private GameObject visualCortadoMedio;
    [SerializeField] private GameObject visualCortadoParejo;
    [SerializeField] private CampoAvena campo;

    public EstadoMechon estado = EstadoMechon.Crecido;
    public string TextoInteraccion => "Cortar avena [E]";

    public void Interactuar(GameObject jugador)
    {
        var mano = jugador.GetComponent<ManoJugador>();
        if (mano == null || mano.ObjetoActual == null || mano.ObjetoActual.Nombre != "Hoz") return;

        switch (estado)
        {
            case EstadoMechon.Crecido: CortarMedio(); break;
            case EstadoMechon.CortadoMedio: CortarParejo(); break;
        }
    }

    private void CortarMedio()
    {
        estado = EstadoMechon.CortadoMedio;
        visualCrecido.SetActive(false);
        visualCortadoMedio.SetActive(true);
        campo.ReportarCorte();
    }

    private void CortarParejo()
    {
        estado = EstadoMechon.CortadoParejo;
        visualCortadoMedio.SetActive(false);
        visualCortadoParejo.SetActive(true);
        campo.ReportarCorteParejo();
    }
}
