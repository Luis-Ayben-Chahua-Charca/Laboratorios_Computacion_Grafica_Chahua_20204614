using UnityEngine;

public class ObjetoAgarrable : MonoBehaviour, IInteractable
{
    [SerializeField] private string nombre = "Hoz";
    [SerializeField] private Vector3 rotacionAlEquipar = Vector3.zero;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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
}