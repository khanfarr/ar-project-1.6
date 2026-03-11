using UnityEngine;
using Vuforia;

public class TaxiMove : MonoBehaviour
{
    [Header("Movement")]
    public float rotateSpeed = 50f; // Kept for inspector compatibility; used to derive travel speed.
    public float turnSpeed = 180f;
    public float edgeHalfSize = 0.45f;
    public float pathHeight = 0.26f;
    public bool clockwise = true;
    public float modelForwardYawOffset = 0f;

    [Header("Tracking")]
    public ObserverBehaviour triggerFace;

    private Vector3[] waypoints;
    private int targetWaypointIndex;

    void Start()
    {
        BuildWaypoints();
        targetWaypointIndex = FindClosestWaypointIndex(transform.localPosition);
    }

    void Update()
    {
        if (triggerFace == null || triggerFace.TargetStatus.Status != Status.TRACKED)
        {
            return;
        }

        MoveAlongPerimeter();
    }

    void MoveAlongPerimeter()
    {
        Vector3 current = transform.localPosition;
        current.y = pathHeight;
        transform.localPosition = current;

        Vector3 target = waypoints[targetWaypointIndex];
        Vector3 toTarget = target - current;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
        {
            targetWaypointIndex = GetNextWaypointIndex(targetWaypointIndex);
            target = waypoints[targetWaypointIndex];
            toTarget = target - current;
            toTarget.y = 0f;
        }

        if (toTarget.sqrMagnitude < 0.000001f)
        {
            return;
        }

        Quaternion movementRotation = GetMovementRotation();
        Quaternion desiredMovementRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        movementRotation = Quaternion.RotateTowards(
            movementRotation,
            desiredMovementRotation,
            turnSpeed * Time.deltaTime
        );

        ApplyMovementRotation(movementRotation);

        // Maps old "rotate speed" feel to perimeter travel speed.
        float perimeter = edgeHalfSize * 8f;
        float linearSpeed = Mathf.Max(0f, rotateSpeed) * (perimeter / 360f);
        float remaining = toTarget.magnitude;
        float step = Mathf.Min(linearSpeed * Time.deltaTime, remaining);

        Vector3 moveForward = movementRotation * Vector3.forward;
        moveForward.y = 0f;

        transform.localPosition += moveForward.normalized * step;
        Vector3 corrected = transform.localPosition;
        corrected.y = pathHeight;
        transform.localPosition = corrected;

        if (Vector3.Distance(transform.localPosition, target) < 0.01f)
        {
            targetWaypointIndex = GetNextWaypointIndex(targetWaypointIndex);
        }
    }

    Quaternion GetMovementRotation()
    {
        Quaternion visualRotation = transform.localRotation;
        Quaternion visualOffset = Quaternion.Euler(0f, modelForwardYawOffset, 0f);
        return visualRotation * Quaternion.Inverse(visualOffset);
    }

    void ApplyMovementRotation(Quaternion movementRotation)
    {
        Quaternion visualOffset = Quaternion.Euler(0f, modelForwardYawOffset, 0f);
        transform.localRotation = movementRotation * visualOffset;
    }

    void BuildWaypoints()
    {
        waypoints = new[]
        {
            new Vector3(edgeHalfSize, pathHeight, edgeHalfSize),
            new Vector3(-edgeHalfSize, pathHeight, edgeHalfSize),
            new Vector3(-edgeHalfSize, pathHeight, -edgeHalfSize),
            new Vector3(edgeHalfSize, pathHeight, -edgeHalfSize)
        };
    }

    int FindClosestWaypointIndex(Vector3 point)
    {
        int closest = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = (point - waypoints[i]).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = i;
            }
        }

        return closest;
    }

    int GetNextWaypointIndex(int currentIndex)
    {
        if (clockwise)
        {
            return (currentIndex + 1) % waypoints.Length;
        }

        return (currentIndex - 1 + waypoints.Length) % waypoints.Length;
    }
}
