using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    Vector3 startPos;

    public float amplitude = 0.3f;
    public float speed = 2f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position =
            startPos + Vector3.up *
            Mathf.Sin(Time.time * speed) * amplitude;
    }
}