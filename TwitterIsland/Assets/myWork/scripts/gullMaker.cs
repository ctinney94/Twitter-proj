using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

//This script handles the creation of seagulls placed around islands

public class gullMaker : MonoBehaviour {

    int gulls = 0,gullIndex;
    public GameObject gull, birthdayGull, valentineSeagull;
    public GameObject exitButton, dummyCameraParent;
    public List<GameObject> myGulls = new List<GameObject>();
    public Material SecretWaluigiSkin;
    public AudioClip[] gullNoises,WaluigiNoises,partyWhistle;
    public bool inGullCam;

    public Vector3 GullCamOffset = new Vector3(7.2f, 11.81f, -27);
    public Vector3 GullCamOffsetRotation = new Vector3(8.75f, 9.65f, 8.75f);

    void Awake()
    {
        //Set up references
        exitButton = GameObject.Find("exitGullCamButton");
        GameObject.Find("Gull cam!").GetComponent<gullCam>().gullCollections.Add(this);
    }

    public void reloadGulls(string text)
    {
        int hashtags=0;
        //Hastags are always preceded by a blank space and then must contain at least one character;
        bool canHash = true;
        bool lastCharHash = false;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                canHash = true;
                lastCharHash = false;
            }
            else if (text[i] == '#' && canHash == true)
            {
                lastCharHash = true;
                canHash = false;
            }
            else if (lastCharHash == true)
            {
                hashtags++;
                lastCharHash = false;
            }
        }
        gulls = hashtags;

        makeGulls(gulls, text.ToLower());
    }

    void makeGulls(int gulls, string input)
    {
        List<string> gullNames = new List<string>();
        using (StreamReader sr = new StreamReader("Assets/myWork/gullNames.txt"))
        {
            while (!sr.EndOfStream)
            {
                gullNames.Add(sr.ReadLine() + " Chips");
            }
        }

        for (int i = 0; i < gulls; i++)
        {
            GameObject gullyGuy = Instantiate(gull);
            int theChosenGull = Random.Range(0, gullNames.Count - 1);
            gullyGuy.name = gullNames[theChosenGull];
            gullNames.Remove(gullNames[theChosenGull]);
            gullyGuy.transform.parent = gameObject.transform;
            myGulls.Add(gullyGuy);
        }
        #region Special gulls!
        if (input.Contains("waluigi"))
        {
            GameObject gullyGuy = Instantiate(gull);
            gullyGuy.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = new Color(136f / 255f, 21f / 255f, 216f / 255f);
            gullyGuy.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].SetColor("_EmissionColor", Color.black);
            gullyGuy.name = "Waluigi Chips";
            gullyGuy.transform.parent = gameObject.transform;
            myGulls.Add(gullyGuy);
        }
        if (input.Contains("birthday"))
        {
            GameObject gullyGuy = Instantiate(birthdayGull);
            int theChosenGull = Random.Range(0, gullNames.Count - 1);
            gullyGuy.name = gullNames[theChosenGull];
            gullyGuy.transform.parent = gameObject.transform;
            myGulls.Add(gullyGuy);
        }
        if (input.Contains("love") || input.Contains("valentine"))
        {
            GameObject gullyGuy = Instantiate(valentineSeagull);
            int theChosenGull = Random.Range(0, gullNames.Count - 1);
            gullyGuy.name = gullNames[theChosenGull];
            gullyGuy.transform.parent = gameObject.transform;
            myGulls.Add(gullyGuy);
        }
        #endregion

        int o = 0;
        foreach(GameObject gull in myGulls)
        {
            gull.GetComponent<gull>().offset = o * (360 / myGulls.Count);
            gull.GetComponent<Animator>().SetFloat("offset", Random.value);
            gull.GetComponent<Animator>().SetFloat("speed", Random.Range(0.75f, 1.4f));
            gull.tag = "gull";
            o++;
        }
    }

    void destoryGulls()
    {
        GameObject[] gulls = GameObject.FindGameObjectsWithTag("gull");
        for (int i = 0; i < gulls.Length; i++)
        {
            //gullNames.Add(gulls[i].name);
            Destroy(gulls[i]);
        }
    }

    public void UpdateRadius(float newRadius)
    {
        gull[] gulls = GetComponentsInChildren<gull>();

        for (int i = 0; i < gulls.Length; i++)
        {
            gulls[i].radius = 3f + (newRadius * 1.5f);
        }
    }

    bool allowSwitch;
    void Update()
    {
        if (inGullCam)
        {
            //Lerp main camera position + rotation
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, dummyCameraParent.transform.position, Time.deltaTime * 3);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, dummyCameraParent.transform.rotation, Time.deltaTime * 3);
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                exitGullCam();

            if (Mathf.Abs(Input.GetAxis("SwitchIsland")) > 0 && allowSwitch)
                gullCam(Input.GetAxis("SwitchIsland") > 0 ? 1 : -1);

            if (myGulls.Count > 1)
            {
                if (Vector3.Distance(Camera.main.transform.position, dummyCameraParent.transform.position) < 2.75f)
                    allowSwitch = true;
            }
        }
    }

    void allowMove()
    {
        allowSwitch = true;
    }


    public void gullCam(int dir)
    {
        allowSwitch = false;
        Invoke("allowMove",1);
        if (myGulls.Count != 0)
        {
            if (dir > 0)
            {
                gullIndex++;
                if (gullIndex > myGulls.Count - 1)
                    gullIndex = 0;
            }
            else
            {
                gullIndex--;
                if (gullIndex < 0)
                    gullIndex = myGulls.Count - 1;
            }

            inGullCam = true;

            if (myGulls[gullIndex].name == "Waluigi Chips")
                soundManager.instance.GetComponent<AudioSource>().PlayOneShot(WaluigiNoises[Random.Range(0, WaluigiNoises.Length)]);
            else
                soundManager.instance.GetComponent<AudioSource>().PlayOneShot(gullNoises[Random.Range(0, gullNoises.Length)]);

            //Move the camera to look at the seagull
            dummyCameraParent.transform.parent = myGulls[gullIndex].transform;
            dummyCameraParent.transform.transform.localPosition = GullCamOffset;
            dummyCameraParent.transform.localRotation = Quaternion.Euler(GullCamOffsetRotation);

            //Set text for UI element
            GameObject.Find("Current gull:").GetComponent<Text>().text = "Current Gull: " + myGulls[gullIndex].name;
        }
        else
            exitGullCam();
    }

    public void exitGullCam()
    {
        inGullCam = false;
        exitButton.GetComponent<Button>().onClick.Invoke();
    }
}
