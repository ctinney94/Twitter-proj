using UnityEngine;
using System.Collections;

public class finishedIsland : MonoBehaviour
{

    Color grass = Color.Lerp(Color.green, Color.black, 0.5f);
    Color sand = Color.Lerp(Color.yellow, Color.white, 0.2f);
    Color rock = Color.gray;
    Color dirt = new Color(0.96f, 0.64f, 0.38f);
    [Range(0, 0.85f)]
    public float blackness = 0;
    Light worldLight;
    public Twitter.API.Tweet thisTweet;
    public int islandIndex;
    GameObject tweetText;
    public SentimentAnalysis SA;
    TextMesh meshy;
    MenuMover IslandInfoUI;

    public void WakeUp()
    {
        worldLight = GameObject.Find("WorldLight").GetComponent<Light>();
        tweetText = new GameObject("Tweet text");
        tweetText.transform.parent = transform;
        tweetText.AddComponent<TextMesh>();
        meshy = tweetText.GetComponent<TextMesh>();
        meshy.anchor = TextAnchor.LowerCenter;
        meshy.alignment = TextAlignment.Center;
        meshy.transform.position = GameObject.Find("flagpole " + islandIndex).transform.position + new Vector3(0,1,0);
        meshy.fontSize = 50;

        numbersToSliders nums = GameObject.Find("numbersToSliders").GetComponent<numbersToSliders>();

        meshy.characterSize = nums.findRank(thisTweet.RTs) / 20;
        FormatString(thisTweet.Text, meshy);

        IslandInfoUI= GameObject.Find("IslandInfo UI").GetComponent<MenuMover>();
    }

    void Update()
    {
        //Distance from Camera
        float distance;
        if (Camera.main!=null)
            distance= Vector3.Distance(tweetText.transform.position, Camera.main.transform.position);
        else
            distance = Vector3.Distance(tweetText.transform.position, GameObject.Find("Camera").transform.position);


        if (distance < 250)
        {
            tweetText.SetActive(true);
            float val = Mathf.Clamp(1 / distance * 15, 0, .5f);
            tweetText.transform.localScale = new Vector3(val, val, val);

        }
        else
            tweetText.SetActive(false);

        if (tweetText != null)
        {
            if (Camera.main != null)
            {
                tweetText.transform.LookAt(2*tweetText.transform.position - Camera.main.transform.position);
                var d = tweetText.transform.localRotation.eulerAngles;
                d.y += 180;
                //tweetText.transform.localRotation = Quaternion.Euler(d);
            }
            else
            {
                tweetText.transform.LookAt(2 * tweetText.transform.position - GameObject.Find("Camera").transform.position);
            }
        }
    }
    
    void FormatString(string text, TextMesh textObject)
    {
        int maxLineChars = 35; //maximum number of characters per line...experiment with different values to make it work

        string[] words;
        var result = "";
        int charCount = 0;
        words = text.Split(" "[0]); //Split the string into seperate words
        result = "";

        for (var index = 0; index < words.Length; index++)
        {

            var word = words[index].Trim();

            if (index == 0)
            {
                result = words[0];
                textObject.text = result;
            }

            if (index > 0)
            {
                charCount += word.Length + 1; //+1, because we assume, that there will be a space after every word
                if (charCount <= maxLineChars)
                {
                    result += " " + word;
                }
                else
                {
                    charCount = 0;
                    result += "\n " + word;
                }
                textObject.text = result;
            }
        }
        result = result.Replace("\\n", "\n ");
        textObject.text = SA.getFormattedText(result);
    }

    //not currently used
    void OnTriggerStay(Collider col)
    {
        int val = 250;
        if (col.tag == "Island")
        {
        Debug.Log("TRIGGERED " + col.name + " at " + gameObject.name);
            Vector3 newPos = Vector3.zero;
            while (newPos.x < 75 && newPos.x > -75)
            {
                while (newPos.z < 75 && newPos.z > -75)
                {
                    newPos = new Vector3(Random.Range(-val, val), 25, Random.Range(-val, val));
                }
            }
            transform.position = newPos;
        }
    }

    void OnMouseDown()
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<cameraOrbitControls>().newTarget = transform.position;
            Camera.main.GetComponent<cameraOrbitControls>().currentIsland = islandIndex;
            Camera.main.GetComponent<cameraOrbitControls>().newTargetOffset = Vector3.zero;
            worldLight.GetComponent<lighting>().newShadowStrength = blackness;
            worldLight.GetComponent<lighting>().newTimeOfDay = (float)thisTweet.dateTime.Hour / 24;
        }
        //I tried adding the poofs back in as a thing.
        //Turns out looking through every single vertex of a big island is kinda time consuming, and slows the program right down.

        /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Color centreColour = GetComponent<MeshFilter>().mesh.colors[NearestVertexTo(hit.point)];

            if (centreColour == Color.Lerp(Color.black, grass, 0.8f) || centreColour == grass)
            {
                GameObject poof = Instantiate(Resources.Load("forestPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == sand || centreColour == Color.Lerp(Color.white, sand, 0.75f))
            {
                GameObject poof = Instantiate(Resources.Load("sandPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == rock || centreColour == Color.Lerp(Color.black, dirt, 0.5f))
            {
                GameObject poof = Instantiate(Resources.Load("rockPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == Color.white)
            {
                GameObject poof = Instantiate(Resources.Load("snowPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else
            {
                GameObject poof = Instantiate(Resources.Load("rockPoof") as GameObject);
                poof.transform.position = hit.point;
            }
        }*/
    }

    public void updateUI()
    {
        float sentiment = GetComponentInChildren<mood>().moodness;
        IslandInfoUI.updateUI(thisTweet,sentiment);
    }

    public int NearestVertexTo(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);
        
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        float minDistanceSqr = Mathf.Infinity;
        int nearestVertex=0;
        // scan all vertices to find nearest
        /*for (int i=0; i < mesh.vertexCount;i++)
        {
            //Find difference between 2 points
            Vector3 diff = point - mesh.vertices[i];

            //Find this difference in vectors as a singular float value
            float distSqr = diff.sqrMagnitude;

            //If this the lowest distance EVAR
            if (distSqr < minDistanceSqr)
            {
                //New lowest distance is current
                minDistanceSqr = distSqr;
                nearestVertex = i;

                if (minDistanceSqr < .5f)
                {
                    i = mesh.vertexCount - 1;
                }
            }
        }*/

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            float distance = Vector3.Distance(point, mesh.vertices[i]);
            if (distance < .5f)
            {
                nearestVertex = i;
                i = mesh.vertexCount - 1;
            }
        }
        return nearestVertex;
    }
}
