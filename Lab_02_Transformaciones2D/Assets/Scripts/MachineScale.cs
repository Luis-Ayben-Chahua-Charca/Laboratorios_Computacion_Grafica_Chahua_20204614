using UnityEngine;
using System.Collections;

public class MachineScale : MonoBehaviour, IMachine
{
    public Sprite leverUp;
    public Sprite leverDown;

    public MachineSlot slot;

    private SpriteRenderer sr;
    private bool isBusy = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        Debug.Log("MachineScale ACTIVADA");

        if (isBusy) return;

        if (slot == null || slot.storedObject == null)
        {
            Debug.Log("No hay objeto en slot");
            return;
        }

        Transform obj = slot.storedObject;

        //  PROTECCIÓN TOTAL
        if (obj.localScale == Vector3.zero)
        {
            Debug.LogWarning("Escala en cero detectada, corrigiendo...");
            obj.localScale = Vector3.one;
        }

        //  alternar entre 1 y 0.5
        if (obj.localScale.x > 0.75f)
        {
            obj.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        else
        {
            obj.localScale = new Vector3(1f, 1f, 1f);
        }

        StartCoroutine(LeverAnimation());
    }

    IEnumerator LeverAnimation()
    {
        isBusy = true;

        sr.sprite = leverDown;

        yield return new WaitForSeconds(1f);

        sr.sprite = leverUp;

        isBusy = false;
    }
}