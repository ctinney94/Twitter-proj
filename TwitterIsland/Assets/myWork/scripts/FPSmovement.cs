using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public GameObject cameraObject, mainCam;
    public bool allowJetpack;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
    }
    public void enter()
    {
        transform.position = GameObject.Find("flagpole(Clone)").transform.position;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    { 
        float rotX = CrossPlatformInputManager.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = CrossPlatformInputManager.GetAxis("Mouse Y") * mouseSensitivity;

        float posX=0, posZ=0;

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
        // gameObject.transform.Translate(posX*moveSpeed, 0, posZ*moveSpeed);
        gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
        cameraObject.transform.rotation *= Quaternion.Euler(-rotY, 0f, 0f);
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
        
        #region mouse things
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mainCam.SetActive(true);
            gameObject.SetActive(false);
        }

        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.visible = false;
        
        #endregion
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
