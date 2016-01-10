using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class gullMaker : MonoBehaviour {

    public int gulls = 0;
    GameObject gull;
    List<string> gullNames = new List<string>();
   
    // Use this for initialization
    void Start()
    {
        using (StreamReader sr = new StreamReader("Assets/myWork/gullNames.txt"))
        {
            while (!sr.EndOfStream)
            {
                gullNames.Add(sr.ReadLine() + " chips");
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
}
