using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    public Transform target;
    public float cameraSpeed;

	// Update is called once per frame
	void Update () {
        transform.LookAt(target);
        moveCamera();
	}

    void moveCamera()
    {
        transform.Translate(new Vector3(cameraSpeed, 0, 0) * Time.deltaTime);
    }

}
