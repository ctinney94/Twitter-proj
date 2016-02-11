using UnityEngine;
using System.Collections;

public class DayNightController : MonoBehaviour
{

    public Light sun;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    float sunInitialIntensity;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        UpdateSun();
    }

    void Update()
    {
        UpdateSun();
    }

    public void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}