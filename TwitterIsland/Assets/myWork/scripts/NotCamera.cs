using UnityEngine;
using System.Collections;

public class NotCamera : MonoBehaviour {

    public GameObject target;
    float cameraSpeed = 0.1f;
    float zoom = 15, camHeight = 10;

	// Update is called once per frame
	void Update ()
    {
        var max = target.GetComponent<PentInfo>().mesh.bounds.center;
        transform.LookAt(max);
        moveCamera();
	}

    float timeCounter =0;

    public void setZoom(float newOffset)
    {
        zoom = newOffset;
    }
    public void setCamHeight(float newOffset)
    {
        camHeight = newOffset;        
    }

    public void changeCameraSpeed(float speed)
    {
        cameraSpeed = speed;
    }

    void moveCamera()
    {
        timeCounter += Time.deltaTime;
        transform.position = new Vector3(Mathf.Cos(timeCounter * cameraSpeed) * zoom, camHeight, Mathf.Sin(timeCounter*cameraSpeed)*zoom);
        //transform.Translate(new Vector3(cameraSpeed*Time.deltaTime, 0, 0)*  transform.localRotation.eulerAngles.y);
    }

}