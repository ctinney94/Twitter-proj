using UnityEngine;
using System.Collections;

public class gull : MonoBehaviour {

    private float angle = 0;
    public float radius = 10;
    public float offset;

	//Move seagull in a circle
	void Update () {
        //I remember building this function based of a forum post or something similar
        //Can't 
        float x = 0;
        float y = 0;
        
        x = radius * Mathf.Cos(angle+(Mathf.Deg2Rad * offset));
        y = radius * Mathf.Sin(angle+(Mathf.Deg2Rad * offset));
        
        //Seagulls always positioned 13 units above origin
        //Maybe this should be influenced by global warming
        transform.localPosition = new Vector3(x, -13,y);
        angle += (.1f/radius) * Mathf.Rad2Deg * Time.deltaTime;
        transform.rotation = Quaternion.Euler(15,(-angle* Mathf.Rad2Deg)-90-offset, 0);
    }
}
