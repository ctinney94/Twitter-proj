using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class gullCam : MonoBehaviour {

    public List<gullMaker> gullCollections = new List<gullMaker>();
    public Button thisButton;
    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button3) && thisButton.enabled && thisButton.interactable)
            EnterGullCam();
    }

    public void EnterGullCam()
    {
        thisButton.onClick.Invoke();
    }

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
