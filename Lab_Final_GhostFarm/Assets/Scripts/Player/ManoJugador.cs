using UnityEngine;

public class ManoJugador : MonoBehaviour
{
    [SerializeField] private Transform puntoDeAgarre;
    public ObjetoAgarrable ObjetoActual { get; private set; }

    public void Equipar(ObjetoAgarrable objeto)
    {
        ObjetoActual = objeto;
        objeto.AlEquipar(puntoDeAgarre);
    }
}