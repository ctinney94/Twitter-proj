using UnityEngine;
using System.Collections;

public class mood : MonoBehaviour {

    [Range(-1,1)]public float moodness = 0;

    [Range(0,0.85f)]public float blackness=0;
    public GameObject cloudObject, rainObject, lightningObject, worldLightObject, fireworksOjbect, rainbowObject; 
    ParticleSystem rain, lightning, cloud, fireworks, rainbow;
    Light lighty;

    public float floater;
	// Use this for initialization
	void Start () {
        cloud = cloudObject.GetComponent<ParticleSystem>();
        rain = rainObject.GetComponent<ParticleSystem>();
        lightning = lightningObject.GetComponent<ParticleSystem>();
        lighty = worldLightObject.GetComponent<Light>();
        fireworks = fireworksOjbect.GetComponent<ParticleSystem>();
        rainbow = rainbowObject.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {

        //Also scale
        var scale = GameObject.Find("IslandMaker").GetComponent<IslandMaker>().meshScale;
        transform.localScale = new Vector3(scale / 7, scale / 7, scale / 7);
        if (moodness > 0)
        {
            GameObject flag = GameObject.Find("flagpole(Clone)");
            if (flag != null)
            {
                fireworks.transform.position = flag.transform.position;
                rainbow.transform.position = flag.transform.position + new Vector3(-7.5f,-2.5f,0);
            }

            if (moodness > 0.8f)
                fireworks.enableEmission = true;
            else
                fireworks.enableEmission = false;

            if (moodness > 0.4f)
                rainbow.enableEmission = true;
            else
                rainbow.enableEmission = false;

            cloud.emissionRate = 0;
            lightning.emissionRate = 0;
            lighty.shadowStrength = 0;
            rain.emissionRate = 0;
        }
        else
        {
            fireworks.enableEmission = false;
            rainbow.enableEmission = false;
            var temp = -1 * moodness;//turn this back into a positive so maths can happen.

            Color cloudClour = Color.Lerp(Color.white, Color.black, temp * 0.85f);
            cloud.GetComponent<Renderer>().material.SetColor("_TintColor", cloudClour);
            lighty.shadowStrength = temp;

            rain.emissionRate = (10 * (temp - 0.5f)) + (scale * scale * 2 * (temp - 0.5f));
            lightning.emissionRate = scale * (temp - 0.6f);
            cloud.emissionRate = scale * scale * temp;
        }
    }

    public void move(Vector3 newPos)
    {
        gameObject.transform.position = newPos + new Vector3(0, 17.5f, 0);
    }
    public void setMood(float mood)
    {
        moodness = mood;
    }

}
