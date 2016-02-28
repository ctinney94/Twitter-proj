using UnityEngine;
using System.Collections;

public class lighting : MonoBehaviour {
    bool lerp;
    public float newShadowStrength, newTimeOfDay;
    Light worldLight;
    DayNightController dayNight;
    void Start()
    {
        GameObject temp = GameObject.Find("DayNightController");
        if (dayNight != null)
            dayNight = temp.GetComponent<DayNightController>();

        worldLight = GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {

        if (newShadowStrength != worldLight.shadowStrength)
            worldLight.shadowStrength = Mathf.Lerp(worldLight.shadowStrength, newShadowStrength, Time.deltaTime * 10);

        //Debug.Log(newShadowStrength);
        if (dayNight != null)
        {
            if (newTimeOfDay != dayNight.currentTimeOfDay)
                dayNight.currentTimeOfDay = Mathf.Lerp(dayNight.currentTimeOfDay, newTimeOfDay, Time.deltaTime * 2);

            newTimeOfDay += (Time.deltaTime / 30f);
        }
    }
}
