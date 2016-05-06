using UnityEngine;
using System.Collections;


//Not used in current version of Tweet Islands.


public class DayNightController : MonoBehaviour
{

    public Light sun, moon;
    [Range(.25f, .75f)]
    public float currentTimeOfDay = 0;
    float sunInitialIntensity;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        UpdateSun();
    }

    public void newTOD(float TOD)
    {
        currentTimeOfDay = TOD;
    }

    void Update()
    {
        UpdateSun();
        //UpdateMoon();
        //currentTimeOfDay += (Time.deltaTime / secondsInFullDay);
    }

    void UpdateMoon()
    {
        var r = sun.transform.rotation.eulerAngles;
        moon.transform.rotation = Quaternion.Euler(-r.x, -r.y, -r.z);
    }

    public void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 310, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay < 0.23f || currentTimeOfDay > 0.73f)
        {
            intensityMultiplier = 0.1f;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.5f) * (1 / 0.02f)));
        }
        //sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}