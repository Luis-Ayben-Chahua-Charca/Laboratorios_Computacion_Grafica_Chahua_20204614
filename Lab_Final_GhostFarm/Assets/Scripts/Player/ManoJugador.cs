using UnityEngine;

public class ManoJugador : MonoBehaviour
{
    [SerializeField] private Transform puntoDeAgarre;
    [SerializeField] private KeyCode teclaSoltar = KeyCode.G;
    [SerializeField] private float distanciaCaida = 1f;

    public ObjetoAgarrable ObjetoActual { get; private set; }

    void Update()
    {
        // FIX: mismo problema que en Interactor — usar JugadorTieneControl
        // en vez de comparar EstadoActual directamente.
        if (!SceneDirector.Instance.JugadorTieneControl) return;
        if (ObjetoActual != null && Input.GetKeyDown(teclaSoltar))
            Soltar();
    }

    public void Equipar(ObjetoAgarrable objeto)
    {
        ObjetoActual = objeto;
        objeto.AlEquipar(puntoDeAgarre);

        if (objeto.Nombre == "Hoz")
            MisionManager.Instance.CompletarObjetivo("recoger_hoz");
    }

    // NUEVO: usado por StageLoader al saltar a una etapa donde el jugador ya
    // debería tener la hoz. A propósito NO llama a CompletarObjetivo — el
    // StageLoader ya dejó las misiones en el estado que corresponde a esa
    // etapa, así que "recoger_hoz" no debe volver a completarse de nuevo.
    public void EquiparSilencioso(ObjetoAgarrable objeto)
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