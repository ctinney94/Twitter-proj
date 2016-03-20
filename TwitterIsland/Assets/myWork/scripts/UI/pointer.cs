using UnityEngine;
using System.Collections;

public class pointer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Camera.main)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in sprites)
            {
                s.GetComponent<BoxCollider>().enabled = true;
                s.enabled = true;
            }
            Vector3 rot = Camera.main.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, -90 - rot.y, transform.rotation.eulerAngles.z));
        }
        else
        {
           SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer s in sprites)
            {
                s.GetComponent<BoxCollider>().enabled = false;
                s.enabled = false;
            }
        }
    }
}
