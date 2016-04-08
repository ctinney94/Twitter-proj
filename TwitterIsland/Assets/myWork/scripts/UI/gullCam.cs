using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gullCam : MonoBehaviour {

    public List<gullMaker> gullCollections = new List<gullMaker>();

	public void cutToGullCam()
    {
        //Enter GULL CAM view
        gullCollections[Camera.main.GetComponent<cameraOrbitControls>().currentIsland-1].gullCam();
    }

    public void exitGullCam()
    {
        //Exit view
        gullCollections[Camera.main.GetComponent<cameraOrbitControls>().currentIsland-1].exitGullCam();
    }
}
