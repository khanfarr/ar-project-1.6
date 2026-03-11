using UnityEngine;
using System;

public class CityTimeController : MonoBehaviour
{
    public Light sceneLight;

    [Header("Testing")]
    public bool useTestTime = true;
    public int testHour = 22; // change this in inspector to test this feature out

    void Start()
    {
        int hour;

        if (useTestTime)
        {
            hour = testHour;
        }
        else
        {
            hour = DateTime.Now.Hour;
        }

        ApplyLighting(hour);
    }

    void ApplyLighting(int hour)
    {
        // DAY
        if (hour >= 7 && hour < 18)
        {
            sceneLight.color = Color.white;
            sceneLight.intensity = 1.2f;
        }

        // SUNSET
        else if (hour >= 18 && hour < 20)
        {
            sceneLight.color = new Color(1f, 0.6f, 0.3f);
            sceneLight.intensity = 0.7f;
        }

        // NIGHT
        else
        {
            sceneLight.color = new Color(0.3f, 0.4f, 0.8f);
            sceneLight.intensity = 0.25f;
        }
    }
}