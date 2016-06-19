using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This is the island builder class
//It handles the size and shape of the pentagonal object used to create islands
//Also manages the creation of hexs and the handling of data 
public class IslandMaker : MonoBehaviour
{
    //basic pentagon mesh making
    public Mesh mesh;
    public Material PentMat, HexMat, wireframeMat;
    public static Texture2D avatar;
    GameObject particles;
    public SentimentAnalysis SA;
    public TextAsset gullNamesFile;
    string[] gullNames;

    public bool enableNoise { get; set; }
    public bool enableHexBlending { get; set; }

    [HideInInspector]
    public float meshScale = .1f, 
        hexScale = .2f, 
        favs = 1, 
        floorLevel;

    GameObject flag;

    public GameObject flagPrefab,particlePrefab;
    bool CreateHexs = true;
    public List<GameObject> hexs = new List<GameObject>();
    List<GameObject> itemsToDestroy = new List<GameObject>();

    float h = 0, w = 0;
    int finishedIslands=0;

    #region Initialization
    void Start()
    {
        enableNoise = true;
        MeshSetup();
        //Fill this pentagon in with hexagons.
	}

    //Create pentagonal mesh
    void MeshSetup()
    {
        #region vertex set-up
        
        Vector3[] pentVerts =
        {
            //Points calculated using this regular pentagon calculator, given a side length of 1
            //The origin/centre of this shape is (0,0,0)
            //http://www.calculatorsoup.com/calculators/geometry-plane/polygon.php
            
            //Calculations for each point can be seen in this diagram:
            //http://i.imgur.com/Ad5DPcu.jpg
            new Vector3((-0.5f-0.5f)*meshScale,floorLevel,(-0.688f-0.688f) *meshScale),//0
            new Vector3((-0.809f-0.809f)*meshScale,floorLevel,(0.263f+0.263f)*meshScale),//1
            new Vector3(0,floorLevel, (0.851f+0.851f)*meshScale),//2
            new Vector3((0.809f+0.809f)*meshScale,floorLevel,(0.263f+0.263f) *meshScale),//3
            new Vector3((0.5f+0.5f)*meshScale,floorLevel,(-0.688f-0.688f) *meshScale),//4
            
            //Same as above, only 1 unit below
            new Vector3((-0.5f-0.5f)*meshScale,floorLevel-1,(-0.688f-0.688f) *meshScale),//0
            new Vector3((-0.809f -0.809f)*meshScale,floorLevel-1,(0.263f+0.263f)*meshScale),//1
            new Vector3(0,floorLevel-1, (0.851f+(0.851f))*meshScale),//2
            new Vector3((0.809f+0.809f)*meshScale,floorLevel-1,(0.263f+0.263f) *meshScale),//3
            new Vector3((0.5f+0.5f)*meshScale,floorLevel-1,(-0.688f-0.688f) *meshScale),//4
        };
        #endregion

        #region triangles set-up

        int[] triangles =
       {
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

        //Create a mesh to pass data into
        mesh = new Mesh();
        //Add verts to the mesh
        mesh.vertices = pentVerts;
        //add triangles to the mesh
        mesh.triangles = triangles;
        //Add UV coordinates to the mesh

        //Create mesh filter and render components to this mesh can be viewed
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //Also add a collider
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

        //...and a material
        meshRenderer.material = PentMat;

        //make it play nicely with lighting
        mesh.RecalculateNormals();
        mesh.name = "Pentagonal";

        //Set the new meshfilter to use the hexagonal mesh created in the previous section
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }
    #endregion

    #region HuD and interactivity functions

    //Pretty self explanatory
    public void setPentVisibility(bool b)
    {
        GetComponent<MeshRenderer>().enabled = b;
        GetComponent<MeshCollider>().enabled = b;
    }
    
    //Remove any hexagon touching the pentagon from the list of items to be destroyed
    void OnTriggerEnter(Collider col)
    {
        if (col.tag!="Player")
         itemsToDestroy.Remove(col.gameObject);
    }
    
    //Reshape the pentagonal shape based on text input
    public void UpdatePentMesh(string inputText)
    {
        inputText = inputText.ToLower();
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };//Vowels of the English language
        int[] vowelCount = new int[] { 1, 1, 1, 1, 1 };//Vowels feature a base count of 1

        //Count the number of vowels present in the string
        for (int i = 0; i < inputText.Length; i++)
            for (int v = 0; v < 5; v++)
                if (inputText[i] == vowels[v])
                    ++vowelCount[v];

        //Find the highest frequency of all vowels
        int mostVowels=0;
        for (int i=0; i < vowelCount.Length;i++)
            if (vowelCount[i] > mostVowels)
                mostVowels = vowelCount[i];

        //Set a maximum distance for vertices to extend in
        float max = (1.0f / mostVowels);
        //Ensures the mesh is scaled to a regular size
        //Another scale value can then be used

        Vector3[] NewPentVerts =
        {
            //Please refer to the MeshSetup() section of this script for reference on vertex creation
            //This is essentially the same thing, only moving the vertices out from the centre for each vowel
            //Like so;
            //          I
            //         ,'.
            //       ,'   `.
            //     ,'       `.
            //   ,'           `. O
            // E \             /
            //    \           /
            //     \         /
            //      \_______/
            //      A        U

            new Vector3((-0.5f-(0.5f*(vowelCount[0]*max)))*meshScale,floorLevel,(-0.688f-(0.688f*(vowelCount[0]*max))) *meshScale),//0
            new Vector3((-0.809f-(0.809f*(vowelCount[1]*max)))*meshScale,floorLevel,(0.263f+(0.263f*(vowelCount[1]*max)))*meshScale),//1
            new Vector3(0,floorLevel, (0.851f+(0.851f*(vowelCount[2]*max)))*meshScale),//2
            new Vector3((0.809f+(0.809f*(vowelCount[3]*max)))*meshScale,floorLevel,(0.263f+(0.263f*(vowelCount[3]*max))) *meshScale),//3
            new Vector3((0.5f+(0.5f*(vowelCount[4]*max)))*meshScale,floorLevel,(-0.688f-(0.688f*(vowelCount[4]*max))) *meshScale),//4
            
            new Vector3((-0.5f-(0.5f*(vowelCount[0]*max)))*meshScale,floorLevel-1,(-0.688f-(0.688f*(vowelCount[0]*max))) *meshScale),//0
            new Vector3((-0.809f -(0.809f*(vowelCount[1]*max)))*meshScale,floorLevel-1,(0.263f+(0.263f*(vowelCount[1]*max)))*meshScale),//1
            new Vector3(0,floorLevel-1, (0.851f+(0.851f*(vowelCount[2]*max)))*meshScale),//2
            new Vector3((0.809f+(0.809f*(vowelCount[3]*max)))*meshScale,floorLevel-1,(0.263f+(0.263f*(vowelCount[3]*max))) *meshScale),//3
            new Vector3((0.5f+(0.5f*(vowelCount[4]*max)))*meshScale,floorLevel-1,(-0.688f-(0.688f*(vowelCount[4]*max))) *meshScale),//4
        };
        mesh.vertices = NewPentVerts;
        
        //I am destorying and remaking the collider all the time
        //If anyone find this I want them to know I'm not proud of what I've done here
        DestroyImmediate(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        GetComponent<MeshFilter>().mesh = mesh;
    }
    #endregion

    #region islandRelated

    //Create a grid a hexagons
    public void updateHexs(string text)
    {
        //Initialization n that
        CreateHexs = true;
        h = 0;
        w = 0;
        largestHeightValue = 0;
        setPentVisibility(true);

        //Reshape the pentagonal shape based on the text input
        UpdatePentMesh(text);

        //Go make some hexagons
        SpawnHexNEW();
    }

    //Hexagon time baby
    void SpawnHexNEW()
    {
        //Find max and min values for bounds
        Vector3 max = GetComponent<MeshCollider>().bounds.max;
        Vector3 min = GetComponent<MeshCollider>().bounds.min;

        #region Hex creation
        while (CreateHexs)
        {
            //Make a hex
            GameObject hex = new GameObject("hex " + h + "," + w);
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
                //Move along
                w++;
                hexs.Add(hex);

                //Every hexagon is born impure
                //Sanctuary can be found within the warm boson of the pentagon
                //All glory to the pentagon 
                itemsToDestroy.Add(hex);
            }

            //If the created hex has reached a line above the maximum bounds, get rid of it and stop making hexs
            if (hex.transform.position.z > max.z + 2 * (hexScale * Mathf.Sqrt(36 - 9)))
            {
                CreateHexs = false;
                hexs.Remove(hex);
                Destroy(hex);
            }
        }
        #endregion

        //Allow a few moments for the pentagon mesh to be updated until anything is destroyed
       Invoke("hexRemoval",.5f);
    }

    //Remove any hexagons of the new island mesh outside of the pentagonal mesh
    void hexRemoval()
    {
        for (int j = 0; j < itemsToDestroy.Count; j++)
        {
            hexs.Remove(itemsToDestroy[j]);
            Destroy(itemsToDestroy[j]);
        }
        itemsToDestroy.Clear();
        StartCoroutine(CreateIslandDELAY());

        //Let's make an island
        //Invoke("CreateIsland", .5f);
    }

    [HideInInspector]
    public float largestHeightValue = 0;

    //Only used for making cool videos, does the same job as CreateIsland only with a delay between hex creation
    //Probs out of date
    IEnumerator CreateIslandDELAY()
    {
        #region all that islands creating shit
        //If nothing's been done yet
        if (hexs[0].GetComponent<HexInfo>().heightValue == 0)
        {
            //Find bordering hexagons
            detectHexEdges();

            //For each hexagon...
            for (int i = 0; i < hexs.Count; i++)
            {
                //Add weightings to the island based on the number of hexs between itself and the edge of the mesh
                hexs[i].GetComponent<HexInfo>().hexWeighter(hexs.Count);
            }

            //Find the highest point on the map (the hex with the largest height value)
            for (int i = 0; i < hexs.Count; i++)
            {
                if (hexs[i].GetComponent<HexInfo>().heightValue > largestHeightValue)
                    largestHeightValue = hexs[i].GetComponent<HexInfo>().heightValue;
            }
            for (int i = 0; i < hexs.Count; i++)
            {
                //Add some height to the hexs using the previously obtained value and # of likes/favs
                hexs[i].GetComponent<HexInfo>().addHeight(largestHeightValue, favs, hexs.Count, enableNoise);
            }
        }

        //Smooth out the island
        //For each hex created...
        for (int i = hexs.Count - 1; i > -1; i--)
        {
            //For each vertice of the hexagon...
            for (int j = 0; j < 7; j++)
            {
                //If there's a camp on this hex, get rid of it.
                if (hexs[i].GetComponent<HexInfo>().camp != null)
                    Destroy(hexs[i].GetComponent<HexInfo>().camp);

                //Smooth current vertex
                hexs[i].GetComponent<HexInfo>().interlopeCorner(j);
            }
            //Update mesh
            hexs[i].GetComponent<MeshCollider>().sharedMesh = hexs[i].GetComponent<MeshFilter>().mesh;
        }

        //Colour the hexs based on position
        for (int i = 0; i < hexs.Count; i++)
            hexs[i].GetComponent<HexInfo>().heightColour(largestHeightValue,enableHexBlending);

        setPentVisibility(false);
        #endregion

        float highestPoint = 0;
        Vector3 flagPos = Vector3.zero;

        //Find highest point on current island
        for (int i = 0; i < hexs.Count; i++)
        {
            if (hexs[i].GetComponent<HexInfo>().heightValue == largestHeightValue)
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
        //Place flag at the highest point of the island
        if (flag != null)
            Destroy(flag);

        flag = Instantiate(flagPrefab) as GameObject;
        soundManager.instance.managedComponents.Add(flag.GetComponent<AudioSource>());
        soundManager.instance.setNewVolume(soundManager.instance.maxVolume);
        flag.transform.position = flagPos + new Vector3(0, -.05f, 0);
        flag.GetComponentsInChildren<Renderer>()[1].material.mainTexture = avatar;

        //If the current user is verified, try makign a dirt path and hut
        if (verified)
            dirtPath();
        yield return null;
    }

    public bool verified;

    //Take a wild guess
    //That's right! It does make island!
    public void CreateIsland()
    {
        #region all that islands creating shit
        //If nothing's been done yet
        if (hexs[0].GetComponent<HexInfo>().heightValue == 0)
        {
            //Find bordering hexagons
            detectHexEdges();

            //For each hexagon...
            for (int i = 0; i < hexs.Count; i++)
            {
                //Add weightings to the island based on the number of hexs between itself and the edge of the mesh
                hexs[i].GetComponent<HexInfo>().hexWeighter(hexs.Count);
            }

            //Find the highest point on the map (the hex with the largest height value)
            for (int i = 0; i < hexs.Count; i++)
            {
                if (hexs[i].GetComponent<HexInfo>().heightValue > largestHeightValue)
                    largestHeightValue = hexs[i].GetComponent<HexInfo>().heightValue;
            }
            for (int i = 0; i < hexs.Count; i++)
            {
                //Add some height to the hexs using the previously obtained value and # of likes/favs
                hexs[i].GetComponent<HexInfo>().addHeight(largestHeightValue, favs, hexs.Count, enableNoise);
            }
        }
    
        //Smooth out the island
        //For each hex created...
        for (int i = hexs.Count - 1; i > -1; i--)
        {
            //For each vertice of the hexagon...
            for (int j = 0; j < 7; j++)
            {
                //If there's a camp on this hex, get rid of it.
                if (hexs[i].GetComponent<HexInfo>().camp != null)
                    Destroy(hexs[i].GetComponent<HexInfo>().camp);

                //Smooth current vertex
                hexs[i].GetComponent<HexInfo>().interlopeCorner(j);
            }
            //Update mesh
            hexs[i].GetComponent<MeshCollider>().sharedMesh = hexs[i].GetComponent<MeshFilter>().mesh;
        }

        //Colour the hexs based on position
        for (int i = 0; i < hexs.Count; i++)
            hexs[i].GetComponent<HexInfo>().heightColour(largestHeightValue,enableHexBlending);

        setPentVisibility(false);
        #endregion

        float highestPoint = 0;
        Vector3 flagPos = Vector3.zero;

        //Find highest point on current island
        for (int i = 0; i < hexs.Count; i++)
        {
            if (hexs[i].GetComponent<HexInfo>().heightValue == largestHeightValue)
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
        //Place flag at the highest point of the island
        if (flag != null)
            Destroy(flag);

        flag = Instantiate(flagPrefab) as GameObject;
        soundManager.instance.managedComponents.Add(flag.GetComponent<AudioSource>());
        soundManager.instance.setNewVolume(soundManager.instance.maxVolume);
        flag.transform.position = flagPos + new Vector3(0, -.05f, 0);
        flag.GetComponentsInChildren<Renderer>()[1].material.mainTexture = avatar;

        //If the current user is verified, try makign a dirt path and hut
        if (verified)
            dirtPath();
    }

    //Find out which hexagons border eachother
    public void detectHexEdges()
    {
        //For each hex, raycast in each direction around them and return the gameobject hit, store as a pal
        for (int i = 0; i < hexs.Count; i++)
        {
            Vector3 hexOrigin = hexs[i].transform.position;

            Vector3[] directions = {
                //lower left
                hexOrigin - new Vector3(4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale),

                 //Upper left
                hexOrigin + new Vector3(-4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale),

                //Above
                hexOrigin + new Vector3(0, 0, (Mathf.Sqrt(36 - 9) * hexScale) * 2),    
                
                //Upper right
                hexOrigin + new Vector3(4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale),

                //Lower right
                hexOrigin - new Vector3(-4.5f * hexScale * 2, 0, Mathf.Sqrt(36 - 9) * hexScale),

                //Below
                hexOrigin - new Vector3(0, 0, (Mathf.Sqrt(36 - 9) * hexScale) * 2)
            };

            RaycastHit hit;
            Ray ray;

            //We need to start from above since the hexagons are 2D
            //If we try to go straight sideways, the raycast won't hit anything
            hexOrigin += Vector3.up;

            int d = 0;
            //Fpr each direction
            foreach (Vector3 v3 in directions)
            {
                //Raycast around the hexagon
                ray = new Ray(hexOrigin, v3 - hexOrigin);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //If a target is hit, add the target to the list of bording hexs
                    hexs[i].GetComponent<HexInfo>().pals[d] = hit.transform.gameObject;
                }
                d++;
            }
        }
    }

    //Not used in current version | OUTDATED
    public void blendColours()
    {
        for (int i = 0; i < hexs.Count; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                //hexs[i].GetComponent<HexInfo>().blendCols(j);
            }
        }

        //For each hex
        //Get current colour
        List<List<Color>> list = new List<List<Color>>();
        for (int j = 0; j < hexs.Count; j++)
        {
            var listB = new List<Color>();
            for (int i = 0; i < 6; i++)
            {
                var hex = hexs[j].GetComponent<HexInfo>();

                int i1 = i - 1;
                if (i1 == -1)
                    i1 = 5;

                int i3 = i + 2;
                if (i3 > 5)
                    i3 -= 6;

                int i4 = i + 4;
                if (i4 > 5)
                    i4 -= 6;

                if (hex.pals[i] != null && hex.pals[i1] != null)
                {
                    var borderHex = hex.pals[i].GetComponent<HexInfo>();
                    var borderHex2 = hex.pals[i1].GetComponent<HexInfo>();
                    Color[] border2Cols = borderHex2.getColors();
                    Color[] borderCols = borderHex.getColors();


                    var newCol = (borderCols[i4] + border2Cols[i3] + hex.getColors()[i]) / 3;
                    listB.Add(newCol);
                }
                else
                {
                    listB.Add(Color.black);
                }
            }
            list.Add(listB);
        }
        //Then for each hex
        //Lerp colours for corners.
        for (int j = 0; j < hexs.Count; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                var hex = hexs[j].GetComponent<HexInfo>();
                hex.moveVert(i, -99, list[j][i]);
            }
        }
    }

    //Creates a random path and colours vertices along it
    //Also places a little hut at the end of the path
    public void dirtPath()
    {

        bool dirtAdded = false;
        while (!dirtAdded)
        {
            //pick a random hex which is nest to a border hex (first green hex inward)
            var random = Random.Range(0, hexs.Count - 1);
            var randomHex = hexs[random].GetComponent<HexInfo>();

            //Is the random hex one we can use to start a path?
            if (((randomHex.heightValue / largestHeightValue) > 0.25f))
            {
                if (randomHex.heightValue < ((int)(0.25 * largestHeightValue)) + 2)
                {
                    //We have a hex we can use!
                    while (!dirtAdded)
                    {
                        var startingHex = (int)Random.Range(0, randomHex.pals.Length);

                        //Can we continue the path in this direction?
                        if (randomHex.pals[startingHex].GetComponent<HexInfo>().heightValue / largestHeightValue <= 0.25f)
                        {
                            //Some number fudging required for grabbing the correct vertex numbers of the hexagon
                            int temp2 = startingHex + 4;
                            if (temp2 > 5)
                                temp2 -= 6;
                            int temp3 = startingHex + 3;
                            if (temp3 > 5)
                                temp3 -= 6;
                            int pal = startingHex + 6;
                            if (pal > 5)
                                pal -= 6;

                            //Colour some vertices as dirt
                            randomHex.pals[pal].GetComponent<HexInfo>().moveVert(temp2, -99, Color.Lerp(Color.black, new Color(0.96f, 0.64f, 0.38f), 0.75f));
                            randomHex.pals[pal].GetComponent<HexInfo>().moveVert(temp3, -99, Color.Lerp(Color.black, new Color(0.96f, 0.64f, 0.38f), 0.75f));

                            //Start making a dirt path
                            randomHex.dirtPath(startingHex, largestHeightValue, 1,this);
                            dirtAdded = true;
                        }
                    }
                }
            }
        }
    }

    GameObject lastIsland;
    public List<GameObject> camps = new List<GameObject>();
    
    //Combine all the hexagon objects together into a single island game object
    public IEnumerator mergeIsland(GameObject gulls, Twitter.API.Tweet THETWEET)
    {
        //now do particle related things
        particles = Instantiate(particlePrefab) as GameObject;
        particles.GetComponent<mood>().flag = flag;
        particles.GetComponent<mood>().moodness = SA.getSAValue(THETWEET.text);
        particles.GetComponent<mood>().UpdateParticles(meshScale);

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
            finishedIslands++;
            hexs[0].name = "Finished Island " + finishedIslands;
            
            if (flag != null)
                flag.transform.parent = hexs[0].transform;

            hexs[0].GetComponent<MeshCollider>().sharedMesh.RecalculateBounds();
            var newBounds = hexs[0].GetComponent<MeshCollider>().sharedMesh.bounds;
            
            for (int i = 0; i < camps.Count; i++)
                camps[i].transform.parent = hexs[0].transform;

            particles.transform.parent = hexs[0].transform;
            gulls.transform.parent = hexs[0].transform;
            gulls.GetComponent<gullMaker>().UpdateRadius(meshScale);

            if (lastIsland != null)
            {
                var oldBounds = lastIsland.GetComponent<MeshCollider>().sharedMesh.bounds;
                hexs[0].transform.position = new Vector3(lastIsland.transform.position.x + oldBounds.size.x + newBounds.size.x, 0, Random.Range(-newBounds.size.z, newBounds.size.z));
            }
            else
                hexs[0].transform.position = new Vector3(150 + (newBounds.size.x / 2), 0, Random.Range(-newBounds.size.z, newBounds.size.z));

            //store position of last island so we can make the new one correctly
            lastIsland = hexs[0];
            Camera.main.GetComponent<cameraOrbitControls>().islands.Add(lastIsland);
            hexs.Clear();
        }
        #endregion

        //Parent the camps, seagulls and particle effects to the island
        for (int i = 0; i < camps.Count; i++)
            camps[i].transform.parent = lastIsland.transform;
        gulls.transform.parent = lastIsland.transform;
        particles.transform.parent = lastIsland.transform;
        
        //Change the name of the flagpole and remove anything not required
        flag.name = "flagpole " + finishedIslands;
        flag = null;
        Destroy(lastIsland.GetComponent<HexInfo>());

        //Set up island values
        var island = lastIsland.AddComponent<finishedIsland>();
        island.thisTweet = THETWEET;
        island.islandIndex = finishedIslands;
        island.SA = SA;

        //Initialize island
        island.WakeUp();
        island.blackness = particles.GetComponent<mood>().blackness;
        
        //This (sometimes) crashes everything when included
        #region disaster code
        //Set up a sphere collider we can use to reposition islands if other islands enter the sphere.
        /*SphereCollider sc = lastIsland.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.center = Vector3.zero;
        sc.radius = meshScale * 2;*/

        /*var val = 250;
        Debug.Log("oh shit here we go");
        lastIsland.transform.position = Vector3.zero;
        while (lastIsland.transform.position.x < 75 && lastIsland.transform.position.x > -75)
        {
            while (lastIsland.transform.position.z < 75 && lastIsland.transform.position.z > -75)
            {
                Debug.Log("Positioning... " + finishedIslands);
                lastIsland.transform.position = new Vector3(Random.Range(-val, val), 0, Random.Range(-val, val));
            }
        }*/
        #endregion

        camps.Clear();

        AudioSource waveSounds = lastIsland.AddComponent<AudioSource>();
        soundManager.instance.managedComponents.Add(waveSounds);
        soundManager.instance.setNewVolume(soundManager.instance.maxVolume);
        waveSounds.playOnAwake = false;
        waveSounds.clip = waveNoises;
        waveSounds.spatialBlend = 1;
        waveSounds.loop = true;
        waveSounds.minDistance = meshScale * 2;
        waveSounds.maxDistance = meshScale * 4;
        waveSounds.rolloffMode = AudioRolloffMode.Linear;
        waveSounds.Play();
        yield return null;
    }
    public AudioClip waveNoises;
    #endregion
}