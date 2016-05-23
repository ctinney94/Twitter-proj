using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class gullCam : MonoBehaviour {

    public static gullCam instance;

    public List<gullMaker> gullCollections = new List<gullMaker>();
    public Button thisButton;
    void Awake()
    {
        instance = this;
        thisButton = GetComponent<Button>();
        thisButton.enabled = false;
    }
    void Update()
    {
        if (Camera.main)
        {
            if (!Camera.main.GetComponent<cameraOrbitControls>().screenshotMode)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) && thisButton.enabled && thisButton.interactable)
                    EnterGullCam();
            }
        }
    }

    public void EnterGullCam()
    {
        thisButton.onClick.Invoke();
    }

    public void cutToGullCam(int dir)
    {
        //Enter GULL CAM view
        gullCollections[Camera.main.GetComponent<cameraOrbitControls>().currentIsland-1].updateGullCam(dir);
    }

    public void exitGullCam()
    {
        //Exit view
        gullCollections[Camera.main.GetComponent<cameraOrbitControls>().currentIsland-1].inGullCam =false;
    }
}
