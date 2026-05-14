using UnityEngine;

public class WindmillRotation : MonoBehaviour
{
    public float speed = 120f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}