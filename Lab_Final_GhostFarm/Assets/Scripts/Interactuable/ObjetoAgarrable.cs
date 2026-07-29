using UnityEngine;

public class ObjetoAgarrable : MonoBehaviour, IInteractable
{
    // NUEVO: identifica QUÉ objeto es esto (para comparaciones en código,
    // ej. "¿el jugador tiene la Cuerda?"). "nombre" de acá abajo queda
    // como texto puramente decorativo para el HUD ("Recoger {nombre} [E]"),
    // ya no se usa para comparar nada.
    [SerializeField] private ObjetoId id;

    [SerializeField] private string nombre = "Hoz";
    [SerializeField] private Vector3 rotacionAlEquipar = Vector3.zero;

    [Header("Misión al recoger (opcional)")]
    [SerializeField] private bool completaMisionAlRecoger = false;
    [SerializeField] private MisionId misionAlRecoger;

    private Rigidbody rb;
    private Collider col;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private Transform padreOriginal;
    private Vector3 escalaOriginal;

    public ObjetoId Id => id;
    public string Nombre => nombre;
    public bool CompletaMisionAlRecoger => completaMisionAlRecoger;
    public MisionId MisionAlRecoger => misionAlRecoger;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        padreOriginal = transform.parent;
        escalaOriginal = transform.localScale;
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
        transform.localScale = escalaOriginal;
    }

    public void AlSoltar(Vector3 posicionFrenteJugador)
    {
        transform.SetParent(null);
        transform.position = posicionFrenteJugador;
        transform.localScale = escalaOriginal;

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false; // la física la deja caer y asentarse sola en el terreno
    }

    public void ResetearAEstadoInicial()
    {
        transform.SetParent(padreOriginal);
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;
        transform.localScale = escalaOriginal;

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
    }

    // NUEVO: usado por ManoJugador.DejarEn (ej. AlmacenHerramientas) para
    // dejar el objeto quieto en un punto fijo (un estante, un gancho) en vez
    // de tirarlo al piso frente al jugador como hace AlSoltar.
    public void ColocarEn(Transform punto)
    {
        transform.SetParent(punto);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // FIX: reparentar sin esto hace que el objeto herede cualquier
        // escala rara que tenga "punto" (el Empty de destino). Forzar la
        // escala original acá garantiza que la Hoz siempre se vea del
        // mismo tamaño, sin importar la escala del punto de guardado.
        transform.localScale = escalaOriginal;

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = true; // queda quieto en su lugar, no cae ni rueda
    }
}