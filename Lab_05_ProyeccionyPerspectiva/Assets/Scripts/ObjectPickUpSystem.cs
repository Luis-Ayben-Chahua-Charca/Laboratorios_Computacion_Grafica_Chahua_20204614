using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    private GameObject currentObject;

    private float objectStartDistance;
    private Vector3 objectStartScale;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (currentObject == null)
            {
                if (Input.GetMouseButtonDown(0) && hit.transform.gameObject.layer == 6)
                {
                    currentObject = hit.transform.gameObject;
                    objectStartDistance = Vector3.Distance(transform.position, currentObject.transform.position);
                    objectStartScale = currentObject.transform.localScale;
                    currentObject.GetComponent<Collider>().enabled = false;
                    currentObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    currentObject.GetComponent<Collider>().enabled = true;
                    currentObject.GetComponent<Rigidbody>().isKinematic = false;
                    currentObject = null;
                }
                else 
                {
                    currentObject.transform.position = hit.point;
                    currentObject.transform.localScale = objectStartScale + (Vector3.one * (Vector3.Distance(transform.position, currentObject.transform.position) - objectStartDistance));
                }
            }
        }
    }


}
