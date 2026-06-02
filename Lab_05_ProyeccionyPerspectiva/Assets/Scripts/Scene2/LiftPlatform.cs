using UnityEngine;

public class LiftPlatform : MonoBehaviour
{
    [SerializeField] private float upperHeight = 5f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 lowerPosition;
    private Vector3 upperPosition;
    private Vector3 targetPosition;

    private bool isUp = false;

    private void Start()
    {
        lowerPosition = transform.position;

        upperPosition = lowerPosition +
                        Vector3.up * upperHeight;

        targetPosition = lowerPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TogglePlatform();
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void TogglePlatform()
    {
        if (isUp)
        {
            targetPosition = lowerPosition;
        }
        else
        {
            targetPosition = upperPosition;
        }

        isUp = !isUp;
    }
}