using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform origenRaycast;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private KeyCode teclaInteractuar = KeyCode.E;

    private IInteractable actual;
    public IInteractable Actual => actual; // <- esta es la línea que probablemente falta

    void Update()
    {
        if (SceneDirector.Instance.EstadoActual != EstadoJuego.ExploracionLibre) return;

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
            Debug.Log("Rayo golpeó: " + hit.collider.name);
            actual = hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}