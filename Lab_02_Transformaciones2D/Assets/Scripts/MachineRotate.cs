using UnityEngine;
using System.Collections;

public class MachineRotate : MonoBehaviour, IMachine
{
    //public float interactRange = 1.5f;
    //public LayerMask boxLayer;

    public Sprite leverUp;
    public Sprite leverDown;

    private SpriteRenderer sr;
    private bool isBusy = false;

    public MachineSlot slot;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {

        if (isBusy) return;
        Debug.Log("Objeto en slot: " + slot.storedObject);
        if (slot.storedObject != null)
        {
            Transform obj = slot.storedObject;

            obj.rotation = Quaternion.Euler(0, 0, obj.eulerAngles.z + 90f);

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
        //Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}