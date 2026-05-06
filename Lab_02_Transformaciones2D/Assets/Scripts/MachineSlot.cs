using UnityEngine;

public class MachineSlot : MonoBehaviour
{
    public Transform storedObject;
    public bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (storedObject == null) return;

        if (other.transform != storedObject) return;

        Rigidbody2D rb = storedObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // 🔥 solo cuando ya casi está quieto
        if (rb.linearVelocity.magnitude > 0.1f) return;

        // 🔥 centrar suavemente (snap final)
        storedObject.position = Vector3.Lerp(
            storedObject.position,
            transform.position,
            0.2f
        );

        // opcional: si ya está casi perfecto → fijarlo totalmente
        if (Vector2.Distance(storedObject.position, transform.position) < 0.01f)
        {
            storedObject.position = transform.position;
        }
    }

    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grabbable") && storedObject == null)
        {
            // FORZAR SOLTAR
            PlayerController player = Object.FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                player.ForceReleaseIfHolding(other.gameObject);
            }

            storedObject = other.transform;

            storedObject.position = transform.position;

            Rigidbody2D rb = storedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.freezeRotation = false; // IMPORTANTE
            }
        }
    }*/


    /*void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == storedObject)
        {
            storedObject = null;
        }
    }*/
}