using UnityEngine;

public class ManoJugador : MonoBehaviour
{
    [SerializeField] private Transform puntoDeAgarre;
    [SerializeField] private KeyCode teclaSoltar = KeyCode.G;
    [SerializeField] private float distanciaCaida = 1f;

    public ObjetoAgarrable ObjetoActual { get; private set; }

    void Update()
    {
        if (SceneDirector.Instance.EstadoActual != EstadoJuego.ExploracionLibre) return;
        if (ObjetoActual != null && Input.GetKeyDown(teclaSoltar))
            Soltar();
    }

    public void Equipar(ObjetoAgarrable objeto)
    {
        ObjetoActual = objeto;
        objeto.AlEquipar(puntoDeAgarre);
    }

    public void Soltar()
    {
        if (ObjetoActual == null) return;
        Vector3 posicionCaida = puntoDeAgarre.position + puntoDeAgarre.forward * distanciaCaida;
        ObjetoActual.AlSoltar(posicionCaida);
        ObjetoActual = null;
    }
}