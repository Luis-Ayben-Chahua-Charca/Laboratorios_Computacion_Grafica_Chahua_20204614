using UnityEngine;
using System.Collections;

public class MachineRotate : MonoBehaviour
{
    public float interactRange = 1.5f;
    public LayerMask boxLayer;

    public Sprite leverUp;
    public Sprite leverDown;

    private SpriteRenderer sr;
    private bool isBusy = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        if (isBusy) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, boxLayer);

        if (hit != null)
        {
            Transform obj = hit.transform;

            // ROTAR 90°
            obj.Rotate(0, 0, 90f);

            StartCoroutine(LeverAnimation());
        }
    }

    IEnumerator LeverAnimation()
    {
        isBusy = true;

        sr.sprite = leverDown;

        yield return new WaitForSeconds(1f);

        sr.sprite = leverUp;

        isBusy = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}