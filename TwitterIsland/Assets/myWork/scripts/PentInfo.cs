using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PentInfo : MonoBehaviour
{
    //basic pentgon mesh making
	public Vector3[] pentVerts;
	public Vector2[] uv;
	public int[] Triangles;
    public Mesh mesh;
    public GameObject InputBit;
    public Material PentMat, HexMat, wireframeMat;

    public float meshScale = 1, hexScale = 0.25f;
    [Range(0.1f, 2)]
    public float favs=1;
    public float floorLevel;

    GameObject flag;
    List<GameObject> itemsToDestroy = new List<GameObject>();

    bool CreateHexs = true;
    List<GameObject> hexs = new List<GameObject>();

    float h = 0, w = 0;
    #region Initialization
    void Start()
	{
        MeshSetup();
        //Fill this pentagon in with hexagons.
	}

    void MeshSetup()
    {
        #region verts

        float floorLevel = 1;
        Vector3[] pentVerts =
        {

            new Vector3(-3.0f - 0.33f* meshScale, floorLevel, 0.0f),
            new Vector3(0f, floorLevel, 2f + 0.2f * meshScale),
            new Vector3(3f + 0.33f * meshScale, floorLevel, 0f),
            new Vector3(2f + 0.2f * meshScale, floorLevel, -4f - 0.4f * meshScale),
            new Vector3(-2 - 0.2f * meshScale, floorLevel, -4f - 0.4f*  meshScale),

            new Vector3(-3.0f - 0.33f * meshScale, floorLevel-1f, 0.0f),
            new Vector3(0f, floorLevel-1f, 2f + 0.2f * meshScale),
            new Vector3(3f + 0.33f* meshScale, floorLevel-1f, 0f),
            new Vector3(2f + 0.2f* meshScale, floorLevel-1f, -4f - 0.4f * meshScale),
            new Vector3(-2 - 0.2f * meshScale, floorLevel-1f, -4f - 0.4f * meshScale),
        };
        #endregion

        #region triangles

        int[] triangles =
       {
            /*1,4,0,
            1,3,4,
            1,2,3*/

            9,5,4,
            4,5,0,
            8,9,4,
            4,3,8,
            3,7,8,
            2,7,3,
            2,6,7,
            6,2,1,
            1,0,6,
            5,6,0,

            4,0,1,
            4,1,3,
            1,2,3,

            6,5,9,
            8,6,9,
            8,7,6
       };
        #endregion

        #region uv
        uv = new Vector2[]
        {
            new Vector2(-3,0),
            new Vector2(0,2),
            new Vector2(3,0),
            new Vector2(2,-4),
            new Vector2(-2,4),
        };
        #endregion

        #region finalize
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

        mesh.vertices = pentVerts;
        mesh.triangles = triangles;

        //add out UV coordinates to the mesh
        //mesh.uv = uv;

        meshRenderer.material = PentMat;

        //make it play nicely with lighting
        mesh.RecalculateNormals();
        mesh.name = "Pentagonal";
        //set the GO's meshFilter's mesh to be the one we just made
        meshFilter.mesh = mesh;

        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        //UV TESTING
        //renderer.material.mainTexture = texture;

        #endregion
    }
    #endregion

    #region HuD and interactivity functions

    public void togglePentVisibility()
    {
        GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
        GetComponent<MeshCollider>().enabled = GetComponent<MeshRenderer>().enabled;
    }

    public void setPentScale(float newScale)
    {
        meshScale = newScale;
    }

    public void setHexScale(float newScale)
    {
        hexScale = newScale;
    }

    void OnTriggerEnter(Collider col)
    {
        itemsToDestroy.Remove(col.gameObject);
    }

    [HideInInspector]
    public float LargestLowestValue = 0;
    public void heights()
    {
        detectHexEdges();
        for (int i = 0; i < hexs.Count; i++)
        {
            hexs[i].GetComponent<HexInfo>().hexWeighter(hexs.Count);
        }

        //Find the leaststs
        for (int i = 0; i < hexs.Count; i++)
        {
            if (hexs[i].GetComponent<HexInfo>().least > LargestLowestValue)
                LargestLowestValue = hexs[i].GetComponent<HexInfo>().avg;
        }
        for (int i = 0; i < hexs.Count; i++)
        {
            hexs[i].GetComponent<HexInfo>().addHeight(LargestLowestValue,favs);
        }
    }

    public void smooth()
    {
        //for (int i = 0; i < hexs.Count; i++)
        for (int i = hexs.Count-1; i > -1; i--)
        {
            for (int j = 0; j < 7; j++)
            {
                hexs[i].GetComponent<HexInfo>().interlopeCorner(j);
            }
        }
        for (int i = 0; i < hexs.Count; i++)
        {
            hexs[i].GetComponent<HexInfo>().heightColour();
        }


        float highestPoint = 0;
        Vector3 flagPos = Vector3.zero;

        //Find highest point on map
        for (int i = 0; i < hexs.Count; i++)
        {
            if (hexs[i].GetComponent<HexInfo>().least == LargestLowestValue)
            {
                Vector3[] points = hexs[i].GetComponent<HexInfo>().getVerts();
                for (int j = 0; j < points.Length; j++)
                {
                    if (points[j].y > highestPoint)
                    {
                        highestPoint = points[j].y;
                        flagPos = points[j] + hexs[i].transform.position;
                    }
                }
            }
        }

        //Place flag at top point!
        flag = Instantiate(Resources.Load("flagpole")) as GameObject;
        flag.transform.position = flagPos + new Vector3(0, .75f, 0);
    }

    public void blendColours()
    {
        for (int i = 0; i < hexs.Count; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                hexs[i].GetComponent<HexInfo>().blendCols(j);
            }
        }
    }

    public void dirtPath()
    {
        bool dirtAdded = false;

        while (!dirtAdded)
        {
            //pick a random hex which is nest to a border hex (first green hex inward)
            var random = Random.Range(0, hexs.Count - 1);
            var randomHex = hexs[random].GetComponent<HexInfo>();

            //Is the random hex one we can use to start a path?
            if (((randomHex.least / LargestLowestValue) > 0.25f))
            {
                if (randomHex.least < ((int)(0.25 * LargestLowestValue)) + 2)
                {
                    //We have a hex we can use!
                    while (!dirtAdded)
                    {
                        var startingHex = (int)Random.Range(0, randomHex.pals.Length);
                        
                        if (randomHex.pals[startingHex].GetComponent<HexInfo>().least / LargestLowestValue <= 0.25f)
                        {
                            int temp2 = startingHex + 4;
                            if (temp2 > 5)
                                temp2 -= 6;
                            int temp3 = startingHex + 3;
                            if (temp3 > 5)
                                temp3 -= 6;
                            int pal = startingHex + 6;
                                if (pal > 5)
                                pal -= 6;

                            randomHex.pals[pal].GetComponent<HexInfo>().moveVert(temp2, -99, Color.Lerp(Color.black, new Color(0.96f, 0.64f, 0.38f), 0.75f));
                            randomHex.pals[pal].GetComponent<HexInfo>().moveVert(temp3, -99, Color.Lerp(Color.black, new Color(0.96f, 0.64f, 0.38f), 0.75f));
                            randomHex.dirtPath(startingHex, LargestLowestValue, 1);
                            dirtAdded = true;
                        }
                    }
                }
            }
        }
    }

    public void UpdatePentMesh()
    {
        InputField tweetData = InputBit.GetComponent<InputField>();
        string inputText = tweetData.text;
        inputText = inputText.ToLower();
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        float[] vowelCount = new float[] { 1, 1, 1, 1, 1 };

        for (int i = 0; i < inputText.Length; i++)
            for (int v = 0; v < 5; v++)
                if (inputText[i] == vowels[v])
                    ++vowelCount[v];

        Vector3[] tempVerts =
        {
            new Vector3(-3.0f - 0.33f*vowelCount[0] * meshScale, floorLevel, 0.0f),
            new Vector3(0f, floorLevel, 2f + 0.2f*vowelCount[1] * meshScale),
            new Vector3(3f + 0.33f*vowelCount[2] * meshScale, floorLevel, 0f),
            new Vector3(2f + 0.2f*vowelCount[3] * meshScale, floorLevel, -4f - 0.4f*vowelCount[3] * meshScale),
            new Vector3(-2 - 0.2f*vowelCount[4] * meshScale, floorLevel, -4f - 0.4f*vowelCount[4]*  meshScale),

            new Vector3(-3.0f - 0.33f*vowelCount[0] * meshScale, floorLevel-1f, 0.0f),
            new Vector3(0f, floorLevel-1f, 2f + 0.2f*vowelCount[1] * meshScale),
            new Vector3(3f + 0.33f*vowelCount[2] * meshScale, floorLevel-1f, 0f),
            new Vector3(2f + 0.2f*vowelCount[3] * meshScale, floorLevel-1f, -4f - 0.4f*vowelCount[3] * meshScale),
            new Vector3(-2 - 0.2f*vowelCount[4] * meshScale, floorLevel-1f, -4f - 0.4f*vowelCount[4] * meshScale),
        };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.vertices = tempVerts;

        //Is this line actually did something, that'd be fucking marvelous, but noooo
        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;


        //I am destorying and remaking the collider all the time
        //If anyone find this I want them to know I'm not proud of what I've done here
        DestroyImmediate(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        GameObject.Find("Particle effects").GetComponent<mood>().move(mesh.bounds.center);
    }
    #endregion

    void SpawnHexNEW()
    {
        Vector3 max = mesh.bounds.max;
        Vector3 min = mesh.bounds.min;
        
        #region Hex creation
        while (CreateHexs)
        {
            //Make a hex
            GameObject hex = new GameObject("hex " + h + "," + w);
            hex.tag = "Hex";
            HexInfo hexinf = hex.AddComponent<HexInfo>();
            hexinf.mat = HexMat;
            hexinf.MeshSetup(hexScale);
            hexinf.transform.position = min;

            #region Move Hex into position
            //Move along by 1 hex
            float x = w * 1.5f * (hexScale * 6);
            float y;
            if (w % 2 != 0)//If w is odd
            {
                //move up by 1 hex
                y = h * 2 * (hexScale * Mathf.Sqrt(36 - 9));
            }
            else
            {
                //Move p by a hex and a half
                y = (h * 2 * (hexScale * Mathf.Sqrt(36 - 9))) + (hexScale * Mathf.Sqrt(36 - 9));
            }

            hex.transform.Translate(x, .5f, y);
            #endregion

            //If we've reached the edge of the pentagon bounding box
            if (hex.transform.position.x > max.x + 2 * (hexScale * Mathf.Sqrt(36 - 9)))
            {
                //Start a new line
                h++;
                w = 0;
                Destroy(hex);
            }
            else
            {
                w++;
                hexs.Add(hex);
                itemsToDestroy.Add(hex);
            }

            if (hex.transform.position.z > max.z + 2 * (hexScale * Mathf.Sqrt(36 - 9)))
            {
                CreateHexs = false;
                hexs.Remove(hex);
                Destroy(hex);
            }

            if (hexs.Count > 4500)
                CreateHexs = false;
        }
        #endregion

        Invoke("hexRemoval", 0.1f);
    }

    void hexRemoval()
    {
        for (int j = 0; j < itemsToDestroy.Count; j++)
        {
            hexs.Remove(itemsToDestroy[j]);
            Destroy(itemsToDestroy[j]);
        }
        itemsToDestroy.Clear();

        //Delete rigidbody
        /*for (int i = 0; i < hexs.Count; i++)
        {
            if (hexs[i] !=null)
            Destroy(hexs[i].GetComponent<HexInfo>().rig);
        }*/

        /*
        #region combine mesh
        CombineInstance[] combine = new CombineInstance[hexs.Count];

        for (int i = 0; i < hexs.Count; i++)
        {

            if (hexs[i] != null)
            {
                combine[i].mesh = hexs[i].GetComponent<MeshFilter>().sharedMesh;
                combine[i].transform = hexs[i].GetComponent<MeshFilter>().transform.localToWorldMatrix;
            }
        }
        if (hexs.Count > 1)
        {
            hexs[0].GetComponent<MeshFilter>().mesh = new Mesh();
            hexs[0].GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            hexs[0].transform.position = Vector3.zero;
            for (int i = 1; i < hexs.Count; i++)
            {
                Destroy(hexs[i]);
            }


            DestroyImmediate(hexs[0].GetComponent<MeshCollider>());
            MeshCollider meshCollider = hexs[0].AddComponent<MeshCollider>();
            meshCollider.sharedMesh = hexs[0].GetComponent<MeshFilter>().mesh;
        }
        #endregion
        */
    }

    public void updateHexs()
    {
        for (int i = 0; i < hexs.Count; i++)
        {
            Destroy(hexs[i].gameObject);
        }
        hexs.Clear();
        CreateHexs = true;
        h = 0;
        w = 0;
        SpawnHexNEW();
        Destroy(flag);
    }

    public void detectHexEdges()
    {
        for (int i = 0; i < hexs.Count; i++)
        {
            Vector3 hexOrigin = hexs[i].transform.position;

            Vector3 above = hexOrigin + new Vector3(0, 0, (Mathf.Sqrt(36 - 9) * hexScale) * 2);
            Vector3 below = hexOrigin - new Vector3(0, 0, (Mathf.Sqrt(36 - 9) * hexScale) * 2);
            Vector3 upperLeft = hexOrigin + new Vector3(-4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale);
            Vector3 upperRight = hexOrigin + new Vector3(4.5f* hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale);
            Vector3 lowerRight = hexOrigin - new Vector3(-4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale);
            Vector3 lowerLeft = hexOrigin - new Vector3(4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale);

            RaycastHit hit;
            Ray ray;
            hexOrigin += new Vector3(0, 1, 0);
            ray = new Ray(hexOrigin, lowerLeft - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[0] = hit.transform.gameObject;
            }

            ray = new Ray(hexOrigin, upperLeft - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[1] = hit.transform.gameObject;
            }

            ray = new Ray(hexOrigin, above - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[2] = hit.transform.gameObject;
            }

            ray = new Ray(hexOrigin, upperRight - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[3] = hit.transform.gameObject;
            }

            ray = new Ray(hexOrigin, lowerRight - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[4] = hit.transform.gameObject;
            }

            ray = new Ray(hexOrigin, below - hexOrigin);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hexs[i].GetComponent<HexInfo>().pals[5] = hit.transform.gameObject;
            }
        }
    }
}