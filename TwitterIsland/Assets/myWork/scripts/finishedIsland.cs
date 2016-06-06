using UnityEngine;
using System.Collections;

public class finishedIsland : MonoBehaviour
{
    //Island colour scheme
    Color grass = Color.Lerp(Color.green, Color.black, 0.5f);
    Color sand = Color.Lerp(Color.yellow, Color.white, 0.2f);
    Color rock = Color.gray;
    Color dirt = new Color(0.96f, 0.64f, 0.38f);

    //Lighting shadow strength, influenced by mood variable of the attached particle system
    //See mood/sentiment analysis scripts for more info
    [Range(0, 0.85f)]
    public float blackness = 0;

    //The tweet associated with this island
    public Twitter.API.Tweet thisTweet;

    public int islandIndex;
    float sizeRank, heightRank;
    GameObject tweetText;
    public SentimentAnalysis SA;
    TextMesh meshy;
    MenuMover IslandInfoUI;

    //Initialization
    public void WakeUp()
    {
        //Set up references
        tweetText = new GameObject("Tweet text");
        tweetText.transform.parent = transform;

        //Add this text object to a list held by the main camera so we can enable/disable text
        Camera.main.GetComponent<cameraOrbitControls>().textMeshObjects.Add(tweetText.AddComponent<TextMesh>());

        numbersToSliders nums = GameObject.Find("numbersToSliders").GetComponent<numbersToSliders>();
        IslandInfoUI = GameObject.Find("IslandInfo UI").GetComponent<MenuMover>();

        //Create a text mesh object to float above the island, displaying the tweet
        meshy = tweetText.GetComponent<TextMesh>();
        meshy.anchor = TextAnchor.LowerCenter;
        meshy.alignment = TextAlignment.Center;
        //Place the text mesh above the flagpole, the highest point on the island, in the middle
        meshy.transform.position = GameObject.Find("flagpole " + islandIndex).transform.position + new Vector3(0,1,0);
        meshy.fontSize = 50;
        //Scale the character size of the text mesh based on number of re-tweets
        meshy.characterSize = Mathf.Clamp(nums.findRank(thisTweet.retweet_count,false) / 20,0.25f,1);
        //Format the tweet text appropriately for display
        FormatString(thisTweet.text, meshy);

        //Find the height and size rank for the associated tweet
        sizeRank = nums.findRank(thisTweet.retweet_count, false);
        heightRank =nums.findRank(thisTweet.favorite_count,true);
    }

    //Rotate and scale text mesh displayed tweet above island
    void Update()
    {
        //Distance from Camera
        float distance;

        //If the island view is active
        if (Camera.main)
        {
            //Rotate the text mesh to face the main camera
            tweetText.transform.LookAt(2 * tweetText.transform.position - Camera.main.transform.position);
            
            //Find the distance between the text mesh and the main camera
            distance = Vector3.Distance(tweetText.transform.position, Camera.main.transform.position);

            //If the camera is close enough...
            if (distance < 100)
            {
                //Enable text
                tweetText.SetActive(true);
                //Scale text based of distance to camera
                float val = Mathf.Clamp(1 / distance * 10, 0, .5f);
                tweetText.transform.localScale = new Vector3(val, val, val);
            }
            else
                tweetText.transform.localScale = Vector3.zero;
        }
        else
        {
            //Rotate the text mesh to face the other camera
            tweetText.transform.LookAt(2 * tweetText.transform.position - GameObject.Find("Camera").transform.position);

            //Find the distance between the text mesh and the other camera
            distance = Vector3.Distance(tweetText.transform.position, GameObject.Find("Camera").transform.position);

            //If the text is within a range
            if (distance < 50)
            {
                //Scale text based of distance to camera
                float val = Mathf.Clamp(distance / 50, .05f, .5f);
                tweetText.transform.localScale = new Vector3(val, val, val);
                tweetText.GetComponent<TextMesh>().color = Color.white;
                if (!tweetText.GetComponent<TextMesh>().richText)
                {
                    //Get the original text back
                    FormatString(thisTweet.text, meshy);
                    tweetText.GetComponent<TextMesh>().richText = true;
                }
            }
            else
            {
                tweetText.transform.localScale = Vector3.one / 2;

                //Fade the text depending on distance from camera
                //Rich text has to be replaced and disabled due to the lack of alpha channel options
                if (tweetText.GetComponent<TextMesh>().richText)
                {
                    string text = tweetText.GetComponent<TextMesh>().text;
                    text = text.Replace("<color=red>", "");
                    text = text.Replace("<color=lime>", "");
                    text = text.Replace("</color>", "");
                    tweetText.GetComponent<TextMesh>().text = text;
                    tweetText.GetComponent<TextMesh>().richText = false;
                }
                tweetText.GetComponent<TextMesh>().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), (distance - 50) / 25);
            }

        }
    }

    //Format the input string with line breaks, apply to the text mesh object
    //Slightly modified from code in the following post;
    //SOURCE:
    //http://forum.unity3d.com/threads/3d-text-wrap.32227/#post-321756
    void FormatString(string text, TextMesh textObject)
    {
        //maximum number of characters per line
        int maxLineChars = 35;
        int charCount = 0;
        var result = "";

        string[] words;
        words = text.Split(" "[0]); //Split the string into separate words

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

        //Analyse the sentiment of this text
        textObject.text = SA.getFormattedText(result);
    }

    //Not currently used
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
    
    //When clicked on, change the target of the camera to this island
    void OnMouseDown()
    {
        if (Camera.main != null)
        {
            var camControls =  Camera.main.GetComponent<cameraOrbitControls>();
            if (!camControls.screenshotMode)
            //Change target to this
            {
                camControls.newTarget = transform.position;

                //If this island is not the currently selected island, update the UI to display new info
                if (camControls.currentIsland != islandIndex)
                    updateUI();

                camControls.currentIsland = islandIndex;
                camControls.newTargetOffset = Vector3.zero;

                //Change world light shadow strength to that appropriate of the island
            }
            //worldLight.GetComponent<lighting>().newTimeOfDay = (float)thisTweet.dateTime.Hour / 24;
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

    //Update information to display in UI
    public void updateUI()
    {
        //Grab sentiment value from game object parented to the island
        float sentiment = GetComponentInChildren<mood>().moodness;
        //Update UI with information for this island
        IslandInfoUI.updateUI(thisTweet,sentiment,heightRank,sizeRank);
    }

    //Not currently used
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
