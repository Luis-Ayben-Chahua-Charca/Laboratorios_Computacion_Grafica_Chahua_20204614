using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform referencePivot;  // Arrastra aquí el 'pivot'
    [SerializeField] private Transform referenceAnchor; // Arrastra aquí el 'TpsAnchor'

    [Header("Configuration")]
    [SerializeField] private float sphereRadius = 0.2f; // Radio de la cámara para no atravesar esquinas
    [SerializeField] private LayerMask collisionMask;   // Capas que bloquean la cámara
    [SerializeField] private float smoothSpeed = 10f;   // Suavizado del movimiento

    private Vector3 _currentVelocity;

    void LateUpdate()
    {
        // 1. Calculamos la posición ideal (donde está el Anchor)
        Vector3 targetPosition = referenceAnchor.position;

        // 2. Calculamos la dirección y distancia máxima desde el pivot
        Vector3 direction = (targetPosition - referencePivot.position).normalized;
        float maxDistance = Vector3.Distance(referencePivot.position, targetPosition);

        RaycastHit hit;

        // 3. Lanzamos un SphereCast desde el pivot hacia el Anchor
        if (Physics.SphereCast(referencePivot.position, sphereRadius, direction, out hit, maxDistance, collisionMask))
        {
            // Si hay obstáculo, el nuevo destino es el punto de impacto (un poco desplazado hacia el pivot)
            targetPosition = referencePivot.position + direction * hit.distance;
        }

        // 4. Aplicamos la posición con un suavizado para que no sea brusco
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Mantener la rotación del Anchor (para que mire hacia donde debe)
        transform.rotation = referenceAnchor.rotation;
    }
}