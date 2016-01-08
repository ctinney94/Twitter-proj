using UnityEngine;
using System.Collections;

public class mood : MonoBehaviour {
    
    [Range(0,0.85f)]public float blackness=0;
    public GameObject cloudObject, rainObject, lightningObject, worldLightObject; 
    ParticleSystem rain, lightning, cloud;
    Light light;

    public float floater;
	// Use this for initialization
	void Start () {
        cloud = cloudObject.GetComponent<ParticleSystem>();
        rain = rainObject.GetComponent<ParticleSystem>();
        lightning = lightningObject.GetComponent<ParticleSystem>();
        light = worldLightObject.GetComponent<Light>();

    }
	
	// Update is called once per frame
	void Update () {
        Color cloudClour = Color.Lerp(Color.white, Color.black, blackness);
        cloud.GetComponent<Renderer>().material.SetColor("_TintColor", cloudClour);
        light.shadowStrength = blackness;

        rain.emissionRate = blackness * 150;
        lightning.emissionRate = blackness*2;

        //0.5
        //5

        //Also scale
        var scale = GameObject.Find("PentTest").GetComponent<PentInfo>().meshScale;
        transform.localScale = new Vector3(scale/7, scale/7, scale/7);

        cloud.emissionRate = scale* scale * 2;
    }

    public void move(Vector3 newPos)
    {
        gameObject.transform.position = newPos + new Vector3(0, 14, 0);
        //Change energy, size and emission
        //mb also rnd velocity
    }
}
