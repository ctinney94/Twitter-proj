using UnityEngine;
using System.Collections;

public class lighting : MonoBehaviour {
    bool lerp;
    public float newShadowStrength, newTimeOfDay;
    Light worldLight;
    DayNightController dayNight;
    void Start()
    {
        dayNight = GameObject.Find("DayNightController").GetComponent<DayNightController>();
        worldLight = GetComponent<Light>();
    }
	// Update is called once per frame
	void Update () {
        if (newShadowStrength != worldLight.shadowStrength)
            worldLight.shadowStrength = Mathf.Lerp(worldLight.shadowStrength, newShadowStrength, Time.deltaTime*10);

        if (newTimeOfDay != dayNight.currentTimeOfDay)
            dayNight.currentTimeOfDay = Mathf.Lerp(dayNight.currentTimeOfDay,newTimeOfDay, Time.deltaTime * 2);
        
    }
}
