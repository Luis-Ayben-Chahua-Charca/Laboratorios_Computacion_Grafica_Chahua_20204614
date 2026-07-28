using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform origenRaycast;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private KeyCode teclaInteractuar = KeyCode.E;

    private IInteractable actual;
    public IInteractable Actual => actual;

    void Update()
    {
        // FIX: antes se comparaba EstadoActual != ExploracionLibre, lo que
        // bloqueaba la interacción apenas arrancaba cualquier Flashback,
        // incluso los que declaran mantenerControlJugador = true.
        // JugadorTieneControl ya contempla ese caso.
        if (!SceneDirector.Instance.JugadorTieneControl) return;

        DetectarInteractuable();
        if (actual != null && Input.GetKeyDown(teclaInteractuar))
            actual.Interactuar(gameObject);
    }

    void DetectarInteractuable()
    {
        actual = null;
        Debug.DrawRay(origenRaycast.position, origenRaycast.forward * distanciaInteraccion, Color.red);

        if (Physics.Raycast(origenRaycast.position, origenRaycast.forward, out RaycastHit hit, distanciaInteraccion))
        {
            actual = hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}