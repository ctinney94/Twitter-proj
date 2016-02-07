using UnityEngine;
using System.Collections;

public class lighting : MonoBehaviour {
    bool lerp;
    public float newShadowStrength;
    Light worldLight;
    void Start()
    {
        worldLight = GetComponent<Light>();
    }
	// Update is called once per frame
	void Update () {
        if (newShadowStrength != worldLight.shadowStrength)
            worldLight.shadowStrength = Mathf.Lerp(worldLight.shadowStrength, newShadowStrength, Time.deltaTime*10);
	}
}
