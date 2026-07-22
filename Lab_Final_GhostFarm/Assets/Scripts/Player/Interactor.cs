using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform origenRaycast;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private KeyCode teclaInteractuar = KeyCode.E;

    private IInteractable actual;

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
        if (Physics.Raycast(origenRaycast.position, origenRaycast.forward, out RaycastHit hit, distanciaInteraccion))
            actual = hit.collider.GetComponent<IInteractable>();
    }
}