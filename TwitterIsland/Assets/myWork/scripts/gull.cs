using UnityEngine;
using System.Collections;

public class gull : MonoBehaviour {

    private float angle = 0;
    public float radius = 10;
    public float offset;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float x = 0;
        float y = 0;

        Vector2 direction = Vector2.zero;

        x = radius * Mathf.Cos(angle+(Mathf.Deg2Rad*offset));
        y = radius * Mathf.Sin(angle+ (Mathf.Deg2Rad * offset));
        
        transform.position = new Vector3(x, 5,y);
        angle += (.1f/radius) * Mathf.Rad2Deg * Time.deltaTime;
        transform.rotation = Quaternion.Euler(15,(-angle* Mathf.Rad2Deg)-90-offset, 0);
    }
}
