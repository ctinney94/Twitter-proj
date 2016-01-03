using UnityEngine;
using System.Collections;

public class mood : MonoBehaviour {
    
    [Range(0,0.85f)]public float blackness=0;
    public GameObject cloud, rain, lightning, worldLight;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Color cloudClour = Color.Lerp(Color.white, Color.black, blackness);
        cloud.GetComponent<Renderer>().material.SetColor("_TintColor", cloudClour);
        worldLight.GetComponent<Light>().shadowStrength = blackness;

        rain.GetComponent<ParticleSystem>().emissionRate = blackness * 150;
        lightning.GetComponent<ParticleSystem>().emissionRate = blackness*2;
    }

    public void move(Vector3 newPos)
    {
        gameObject.transform.position = newPos + new Vector3(0, 7, 0);
        //Change energy, size and emission
        //mb also rnd velocity
    }
}
