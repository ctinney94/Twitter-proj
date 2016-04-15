using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

//Script used for first person movement
//Hold Shift to sprint at x3 speed
//Hold Space to fly upwards

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public GameObject cameraObject, mainCam, IslslandHuD;
    public bool allowJetpack;

    // Use this for initialization
    void Start()
    {
        //Cursor.visible = false;
    }
    public void enter()
    {
        int currentFlag = mainCam.GetComponent<cameraOrbitControls>().currentIsland;
        IslslandHuD.SetActive(false);
        mainCam.SetActive(false);
        cameraObject.SetActive(true);
        if (currentFlag != 0)
            transform.position = GameObject.Find("flagpole " + currentFlag).transform.position + new Vector3(0, 1, 0);
        else
            transform.position = GameObject.Find("flagpole(Clone)").transform.position + new Vector3(0, 1, 0);


        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    Vector3 velocity;
    bool mouse=true;
    // Update is called once per frame
    void Update()
    {
        if (mouse)
        {
            float rotX = CrossPlatformInputManager.GetAxis("Mouse X") * mouseSensitivity;
            float rotY = CrossPlatformInputManager.GetAxis("Mouse Y") * mouseSensitivity;
            gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
            cameraObject.transform.rotation *= Quaternion.Euler(-rotY, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            mouse = !mouse;
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
            moveSpeed *= 3;

        if (Input.GetKeyUp(KeyCode.LeftShift))
            moveSpeed /= 3;

        float posX = 0, posZ = 0;

        #region movement

        if (Input.GetKey(KeyCode.W))
        {
            posZ += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            posZ -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            posX -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            posX += 1;
        }

        velocity = new Vector3(posX * moveSpeed, 0, posZ * moveSpeed);
        #endregion

        #region jetpack
        if (Input.GetKey(KeyCode.Space))
        {
            //if (gameObject.GetComponent<Rigidbody>().velocity.y < 0.01)            
            //gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);

            if (allowJetpack)
            {
                gameObject.transform.Translate(0, 0.1f, 0);
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        else
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        #endregion
        
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IslslandHuD.SetActive(true);
            mainCam.SetActive(true);
            gameObject.SetActive(false);
        }

        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.visible = false;
    }

    void FixedUpdate()
    {
        var rig = GetComponent<Rigidbody>();
        rig.transform.Translate(velocity * Time.fixedDeltaTime);
    }

    public void lockMouse()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
