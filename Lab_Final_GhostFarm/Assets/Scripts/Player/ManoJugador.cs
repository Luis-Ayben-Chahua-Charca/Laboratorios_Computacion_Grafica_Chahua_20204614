using UnityEngine;

public class ManoJugador : MonoBehaviour
{
    [SerializeField] private Transform puntoDeAgarre;
    [SerializeField] private KeyCode teclaSoltar = KeyCode.G;
    [SerializeField] private float distanciaCaida = 1f;

    public ObjetoAgarrable ObjetoActual { get; private set; }

    // NUEVO: mismo patrón que FPSCameraController.alturaOriginalY. Se captura
    // en Awake (no en Start) para estar lista antes que cualquier Start() de
    // otro script (ej. VisualModeController) intente usarla.
    private float alturaOriginalY;

    void Awake()
    {
        alturaOriginalY = puntoDeAgarre.localPosition.y;
    }

    void Update()
    {
        if (!SceneDirector.Instance.JugadorTieneControl) return;
        if (ObjetoActual != null && Input.GetKeyDown(teclaSoltar))
            Soltar();
    }

    public void Equipar(ObjetoAgarrable objeto)
    {
        ObjetoActual = objeto;
        objeto.AlEquipar(puntoDeAgarre);

        if (objeto.Nombre == "Hoz")
            MisionManager.Instance.CompletarObjetivo(MisionId.RecogerHoz);
    }

    public void EquiparSilencioso(ObjetoAgarrable objeto)
    {
        ObjetoActual = objeto;
        objeto.AlEquipar(puntoDeAgarre);
    }

    public void SoltarSilencioso()
    {
        if (ObjetoActual == null) return;
        ObjetoActual.ResetearAEstadoInicial();
        ObjetoActual = null;
    }

    public void Soltar()
    {
        if (ObjetoActual == null) return;
        Vector3 posicionCaida = puntoDeAgarre.position + puntoDeAgarre.forward * distanciaCaida;
        ObjetoActual.AlSoltar(posicionCaida);
        ObjetoActual = null;
    }

    // NUEVO: espejo de FPSCameraController.SetOffsetAltura. VisualModeController
    // llama a este método con el mismo valor que le pasa a la cámara, para que
    // la mano (y cualquier objeto que tengas agarrado) baje en sincronía con
    // la cámara al entrar en modo memoria, en vez de quedarse flotando a la
    // altura "realista" mientras la cámara ya bajó.
    public void SetOffsetAltura(float offset)
    {
        Vector3 pos = puntoDeAgarre.localPosition;
        pos.y = alturaOriginalY + offset;
        puntoDeAgarre.localPosition = pos;
    }
}