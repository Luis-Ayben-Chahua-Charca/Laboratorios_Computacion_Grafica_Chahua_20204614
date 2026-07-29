using UnityEngine;

public class ManoJugador : MonoBehaviour
{
    [SerializeField] private Transform puntoDeAgarre;
    [SerializeField] private KeyCode teclaSoltar = KeyCode.G;
    [SerializeField] private float distanciaCaida = 1f;

    public ObjetoAgarrable ObjetoActual { get; private set; }

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

        // FIX: antes era "if (objeto.Nombre == "Hoz") CompletarObjetivo(RecogerHoz)"
        // hardcodeado acá. Ahora cualquier ObjetoAgarrable declara su propia
        // misión al recogerse (ver ObjetoAgarrable.cs) — la Hoz y la Cuerda
        // usan el mismo camino sin que este script conozca sus nombres.
        if (objeto.CompletaMisionAlRecoger)
            MisionManager.Instance.CompletarObjetivo(objeto.MisionAlRecoger);
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

    // NUEVO: a diferencia de Soltar() (que tira el objeto al mundo) esto lo
    // destruye directamente — para objetos que se "gastan" al usarse, como
    // la cuerda al reparar la cerca.
    public void ConsumirObjetoActual()
    {
        if (ObjetoActual == null) return;
        Destroy(ObjetoActual.gameObject);
        ObjetoActual = null;
    }

    // NUEVO: usado por AlmacenHerramientas — deja el objeto quieto en un
    // punto fijo (a diferencia de Soltar(), que lo tira al piso).
    public void DejarEn(Transform punto)
    {
        if (ObjetoActual == null) return;
        ObjetoActual.ColocarEn(punto);
        ObjetoActual = null;
    }

    public void SetOffsetAltura(float offset)
    {
        Vector3 pos = puntoDeAgarre.localPosition;
        pos.y = alturaOriginalY + offset;
        puntoDeAgarre.localPosition = pos;
    }
}