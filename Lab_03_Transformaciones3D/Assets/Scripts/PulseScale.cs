using UnityEngine;

public class PulseScale : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float speed = 2f;

    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scale =
            1 + Mathf.Sin(Time.time * speed) * amplitude;

        transform.localScale =
            startScale * scale;
    }
}