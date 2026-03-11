using UnityEngine;
using Vuforia;

public class PlanePresenceManager : MonoBehaviour
{
    public ObserverBehaviour cube1;
    public ObserverBehaviour cube2;

    public Transform torontoBuilding;
    public Transform nycBuilding;

    public GameObject plane;

    public float orbitSpeed = 25f;
    public float orbitHeight = 0.6f;
    public float orbitRadiusMultiplier = 0.6f;
    public float bankAmount = 35f;
    [Range(0f, 20f)] public float followSmoothing = 8f;

    private bool planeActive = false;

    private Vector3 centerPoint;
    private float orbitRadius;
    private float angle = 0f;

    void Start()
    {
        if (plane != null)
        {
            plane.SetActive(false);
        }
    }

    void Update()
    {
        if (!HasReferences())
        {
            return;
        }

        bool cube1Tracked = IsTracked(cube1);
        bool cube2Tracked = IsTracked(cube2);

        if (cube1Tracked && cube2Tracked)
        {
            bool wasActive = planeActive;
            SetPlaneActive(true);
            UpdateOrbitTarget(wasActive);
            OrbitPlane();
        }
        else
        {
            SetPlaneActive(false);
        }
    }

    bool HasReferences()
    {
        return cube1 != null
            && cube2 != null
            && torontoBuilding != null
            && nycBuilding != null
            && plane != null;
    }

    bool IsTracked(ObserverBehaviour observer)
    {
        return observer.TargetStatus.Status == Status.TRACKED
            || observer.TargetStatus.Status == Status.EXTENDED_TRACKED;
    }

    void SetPlaneActive(bool isActive)
    {
        if (planeActive == isActive)
        {
            return;
        }

        planeActive = isActive;
        plane.SetActive(isActive);
    }

    void UpdateOrbitTarget(bool wasActive)
    {
        Vector3 desiredCenter = (torontoBuilding.position + nycBuilding.position) / 2f;
        float desiredRadius =
            Vector3.Distance(torontoBuilding.position, nycBuilding.position) * orbitRadiusMultiplier;

        if (!wasActive)
        {
            centerPoint = desiredCenter;
            orbitRadius = desiredRadius;
            return;
        }

        float t = Mathf.Clamp01(followSmoothing * Time.deltaTime);
        centerPoint = Vector3.Lerp(centerPoint, desiredCenter, t);
        orbitRadius = Mathf.Lerp(orbitRadius, desiredRadius, t);
    }

    void OrbitPlane()
    {
        angle += orbitSpeed * Time.deltaTime;
        if (angle > 360f)
        {
            angle -= 360f;
        }

        float rad = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            orbitHeight,
            Mathf.Sin(rad) * orbitRadius
        );

        Vector3 pos = centerPoint + offset;

        plane.transform.position = pos;

        Vector3 tangent = new Vector3(-Mathf.Sin(rad), 0, Mathf.Cos(rad));

        Quaternion lookRot = Quaternion.LookRotation(tangent);

        float bank = Mathf.Sin(rad) * bankAmount;

        plane.transform.rotation =
            lookRot * Quaternion.AngleAxis(bank, Vector3.forward);
    }
}
