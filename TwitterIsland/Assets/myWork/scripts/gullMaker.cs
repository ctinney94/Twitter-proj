using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class gullMaker : MonoBehaviour {

    public int gulls = 0;
    GameObject gull;
    List<string> gullNames = new List<string>();
    public GameObject islandMenus, gullcam;

    public AudioClip[] gullNoises;

    // Use this for initialization
    void Start()
    {
        using (StreamReader sr = new StreamReader("Assets/myWork/gullNames.txt"))
        {
            while (!sr.EndOfStream)
            {
                gullNames.Add(sr.ReadLine() + " Chips");
            }
        }
        gull = Resources.Load("gull") as GameObject;
        makeGulls(gulls);
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
        
        //Sorry guys :(
        destoryGulls();
        makeGulls(hashtags);
    }

    void makeGulls(int gulls)
    {
        for (int i = 0; i < gulls; i++)
        {
            GameObject gullyGuy = Instantiate(gull);
            gullyGuy.GetComponent<gull>().offset = i * (360 / gulls);

            int theChosenGull = Random.Range(0, gullNames.Count);

            gullyGuy.GetComponent<Animator>().SetFloat("offset", Random.value);
            gullyGuy.GetComponent<Animator>().SetFloat("speed", Random.Range(0.75f, 1.4f));
            gullyGuy.name = gullNames[theChosenGull];
            gullNames.Remove(gullNames[theChosenGull]);
            gullyGuy.tag = "gull";
            gullyGuy.transform.SetParent(gameObject.transform);
        }
    }

    void destoryGulls()
    {
        GameObject[] gulls = GameObject.FindGameObjectsWithTag("gull");
        for (int i = 0; i < gulls.Length; i++)
        {
            gullNames.Add(gulls[i].name);
            Destroy(gulls[i]);
        }
    }

    // Update is called once per frame
    public void UpdateRadius(float newRadius)
    {
        GameObject[] gulls = GameObject.FindGameObjectsWithTag("gull");

        for (int i=0; i < gulls.Length;i++)
        {
            gulls[i].GetComponent<gull>().radius = 3f+(newRadius*1.5f);
        }
    }

    public void gullCam()
    {
        var audio = gullcam.GetComponent<AudioSource>();
        audio.PlayOneShot(gullNoises[Random.Range(0, gullNoises.Length)]);
        islandMenus.SetActive(false);
        gullcam.SetActive(true);
        GameObject[] gulls = GameObject.FindGameObjectsWithTag("gull");

        //Grab the camera, disable iesland menu canvas, enable a new one
        Camera.main.GetComponent<cameraOrbitControls>().enabled = false;
        //Camera.main.GetComponent<mobileControls>().enabled = false;

        int randomGull = Random.Range(0, gulls.Length);
        if (gulls.Length > 1)
        {
            while (Camera.main.transform.parent == gulls[randomGull].transform)
                randomGull = Random.Range(0, gulls.Length);
        }
        Camera.main.transform.parent = gulls[randomGull].transform;

        Camera.main.transform.localPosition = new Vector3(7.2f,11.81f, -21);
        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(8.75f, 9.65f, 8.75f));
        
        GameObject.Find("Current gull:").GetComponent<Text>().text = "Current Gull: " + Camera.main.transform.parent.name;
    }

    public void exitGullCam()
    {
        islandMenus.SetActive(true);
        gullcam.SetActive(false);
        Camera.main.transform.parent = null;
        Camera.main.GetComponent<cameraOrbitControls>().enabled = true;
    }
}
