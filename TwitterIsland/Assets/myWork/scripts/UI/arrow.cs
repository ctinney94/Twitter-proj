using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {

    public int dir;

    //When the arrow sprite in clicked on...
    void OnMouseDown()
    {
        //Change the target island of the main camera
        Camera.main.GetComponent<cameraOrbitControls>().changeTarget(dir);
    }
}
