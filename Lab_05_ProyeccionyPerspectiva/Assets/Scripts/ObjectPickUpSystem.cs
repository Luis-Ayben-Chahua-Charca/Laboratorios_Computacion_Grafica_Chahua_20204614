using UnityEngine;

public class ObjectPickUpSystem : MonoBehaviour
{
    [Header("Configuracion de Perspectiva")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 100f;
    //[SerializeField] private float holdDistance = 3f;


    private Vector3 originalScale;
    private float originalDistance;

    private Vector3 lastValidPosition;
    private Vector3 lastValidScale;

    private GrabableObject currentObject;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentObject == null)
            {
                TryPickUp();
            }
            else
            {
                DropObject();
            }
        }

        if (currentObject != null)
        {
            FollowCamera();
            UpdateScale();
        }
    }

    private void TryPickUp()
    {
        Ray ray = new Ray(
        playerCamera.transform.position,
        playerCamera.transform.forward
    );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            GrabableObject grabable =
                hit.collider.GetComponent<GrabableObject>();

            if (grabable == null)
            {
                grabable =
                    hit.collider.GetComponentInParent<GrabableObject>();
            }

            if (grabable != null)
            {
                currentObject = grabable;

                originalDistance =
                    Vector3.Distance(
                        playerCamera.transform.position,
                        currentObject.transform.position
                    );

                originalScale =
                    currentObject.transform.localScale;

                Rigidbody rb =
                    currentObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                Collider col =
                    currentObject.GetComponent<Collider>();

                if (col != null)
                {
                    col.enabled = false;
                }

                Debug.Log($"Objeto agarrado: {currentObject.name}");
            }
        }
    }

    private void DropObject()
    {
        Rigidbody rb =
        currentObject.GetComponent<Rigidbody>();

        currentObject.transform.position = lastValidPosition;
        currentObject.transform.localScale = lastValidScale;

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col =
            currentObject.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }

        Debug.Log($"Objeto soltado: {currentObject.name}");

        currentObject = null;
    }
    private void FollowCamera()
    {
        Ray ray = new Ray(
        playerCamera.transform.position,
        playerCamera.transform.forward);

        lastValidPosition = currentObject.transform.position;
        lastValidScale = currentObject.transform.localScale;


        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Collider col = currentObject.GetComponent<Collider>();

            float surfaceOffset = 0.5f;

            if (col != null)
            {
                surfaceOffset = col.bounds.extents.magnitude;
            }

            currentObject.transform.position =
                hit.point +
                hit.normal * surfaceOffset;
        }
    }
    private void UpdateScale()
    {
        GrabableObject data = currentObject;

        float currentDistance =
            Vector3.Distance(
                playerCamera.transform.position,
                currentObject.transform.position
            );

        float scaleFactor =
            currentDistance / originalDistance;

        Vector3 targetScale =
            originalScale * scaleFactor;

        float clampedScale =
            Mathf.Clamp(
                targetScale.x,
                data.minScale,
                data.maxScale
            );

        currentObject.transform.localScale =
            Vector3.one * clampedScale;
    }
}




