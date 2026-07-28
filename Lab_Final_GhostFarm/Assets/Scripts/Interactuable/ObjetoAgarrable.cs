using UnityEngine;

public class ObjetoAgarrable : MonoBehaviour, IInteractable
{
    [SerializeField] private string nombre = "Hoz";
    [SerializeField] private Vector3 rotacionAlEquipar = Vector3.zero;

    private Rigidbody rb;
    private Collider col;

    // NUEVO: posición/rotación/padre originales, capturados apenas arranca la
    // escena. Permiten "devolver" el objeto a su lugar de spawn cuando
    // StageLoader salta a una etapa donde el jugador no debería tenerlo en
    // mano — sin esto, volver a una etapa anterior dejaba la hoz pegada a la
    // cámara aunque esa etapa diga "jugadorTieneHoz = false".
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Transform padreOriginal;

    public string Nombre => nombre;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        padreOriginal = transform.parent;
    }

    public string TextoInteraccion => $"Recoger {nombre} [E]";

    public void Interactuar(GameObject jugador)
    {
        jugador.GetComponent<ManoJugador>()?.Equipar(this);
    }

    public void AlEquipar(Transform puntoDeAgarre)
    {
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true; // deja de responder a física mientras está en mano

        transform.SetParent(puntoDeAgarre);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(rotacionAlEquipar);
    }

    public void AlSoltar(Vector3 posicionFrenteJugador)
    {
        transform.SetParent(null);
        transform.position = posicionFrenteJugador;

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false; // la física la deja caer y asentarse sola en el terreno
    }

    // NUEVO: usado por ManoJugador.SoltarSilencioso (llamado desde StageLoader)
    // para devolver el objeto a su posición de spawn original, en vez de
    // tirarlo frente al jugador como hace AlSoltar. Pensado específicamente
    // para saltos de etapa hacia atrás.
    public void ResetearAEstadoInicial()
    {
        transform.SetParent(padreOriginal);
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
    }
}