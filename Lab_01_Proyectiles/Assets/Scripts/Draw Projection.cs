using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    CannonController cannonController;
    LineRenderer lineRenderer;

    public int numPoints = 50;

    public float timeBetweenPoints = 0.02f;

    public LayerMask CollidableLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cannonController = GetComponent<CannonController>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = numPoints;

        List<Vector3> points = new List<Vector3>();

        Vector3 startingPosition = cannonController.ShotPoint.position;
        Vector3 startingVelocity = cannonController.ShotPoint.up * cannonController.BlastPower;

        Vector3 previousPoint = startingPosition;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeBetweenPoints;

            Vector3 newPoint = startingPosition
                             + startingVelocity * t
                             + 0.5f * Physics.gravity * t * t;

            // DETECCIÓN DE COLISIÓN REAL
            if (Physics.Raycast(previousPoint, newPoint - previousPoint, out RaycastHit hit,
                                (newPoint - previousPoint).magnitude, CollidableLayers))
            {
                points.Add(hit.point); // punto exacto de impacto
                break;
            }

            points.Add(newPoint);
            previousPoint = newPoint;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
