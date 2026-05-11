using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);

    [SerializeField] private float followSpeed = 10f;

    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 desiredPosition =
            target.position +
            target.rotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}