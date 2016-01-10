using UnityEngine;
using System.Collections;

public class makeGulls : MonoBehaviour {

    public int gulls = 0;

    // Use this for initialization
    void Start()
    {
        GameObject gull = Resources.Load("gull") as GameObject;

        for (int i = 0; i < gulls; i++)
        {
            GameObject gullyGuy = Instantiate(gull);
            gullyGuy.GetComponent<gull>().offset = i * (360 / gulls);
            gullyGuy.tag = "gull";
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
