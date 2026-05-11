using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 10f;
    /*[Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);*/


    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            cameraPivot.position,
            followSpeed * Time.deltaTime
        );

        transform.rotation = cameraPivot.rotation;
        /*Vector3 desiredPosition =
            target.position +
            target.rotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.5f);*/
    }
}