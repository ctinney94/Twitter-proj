using UnityEngine;
using System.Collections;

public class pointer : MonoBehaviour {
    
    void Update()
    {
        //If the main island view is active...
        if (Camera.main)
        {
            //Enable arrow sprite rendering
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in sprites)
            {
                s.GetComponent<BoxCollider>().enabled = true;
                s.enabled = true;
            }

            //Rotate to arrows to always point left and right of the camera view
            Vector3 rot = Camera.main.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, -90 - rot.y, transform.rotation.eulerAngles.z));
        }
        else
        {
            //Disable arrow sprite rendering
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer s in sprites)
            {
                s.GetComponent<BoxCollider>().enabled = false;
                s.enabled = false;
            }
        }
    }
}
