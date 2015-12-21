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
    public Material PentMat, HexMat;

    public float meshScale = 1, hexScale = 0.25f;

    List<GameObject> itemsToDestroy = new List<GameObject>();

    bool CreateHexs = true;
    List<GameObject> hexs = new List<GameObject>();
    int buildDirection = 0;
    //0 = Top Right
    //1 = Bottom Right
    //2 = Bottom Left
    //3 = Top Left

    float h = 0, w = 0;

    void Start()
	{
        MeshSetup();
        //Fill this pentagon in with hexagons.
	}

    void OnTriggerEnter(Collider col)
    {
        itemsToDestroy.Remove(col.gameObject);
    }
    
    void SpawnHexNEW()
    {
        Vector3 max = mesh.bounds.max;
        Vector3 min = mesh.bounds.min;

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

            hex.transform.Translate(x, 0f, y);
            #endregion

            if (hex.transform.position.x > max.x + 2 * (hexScale * Mathf.Sqrt(36 - 9)))
            {
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
        }
        #endregion

        Invoke("hexRemoval", 0.1f);
    }

    void MeshSetup()
    {
        #region verts

        float floorLevel = 1;
        Vector3[] pentVerts =
        {
            new Vector3(-3f, floorLevel, 0f),  //0
            new Vector3(0f, floorLevel, 2f ),  //1
            new Vector3(3f, floorLevel, 0f),   //2
            new Vector3(2f , floorLevel, -4f), //3
            new Vector3(-2, floorLevel, -4f),  //4
            new Vector3(-3f, -floorLevel, 0f),  //0
            new Vector3(0f, -floorLevel, 2f ),  //1
            new Vector3(3f, -floorLevel, 0f),   //2
            new Vector3(2f , -floorLevel, -4f), //3
            new Vector3(-2, -floorLevel, -4f),  //4
        };
        #endregion

        #region triangles

        int[] triangles =
       {
            /*1,4,0,
            1,3,4,
            1,2,3*/
            
            
            0,4,5,
            4,5,9,
            4,8,9,
            4,8,3,
            3,7,8,
            2,3,7,
            2,6,7,
            1,2,6,
            1,0,6,
            0,6,5,

            9,5,6,
            9,6,8,
            6,7,8
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
    
    void hexRemoval()
    {
        for (int j = 0; j < itemsToDestroy.Count; j++)
        {
            hexs.Remove(itemsToDestroy[j]);
            Destroy(itemsToDestroy[j]);
        }
        itemsToDestroy.Clear();

        for (int i = 0; i < hexs.Count; i++)
        {
            Destroy(hexs[i].GetComponent<HexInfo>().rig);
        }

    }

    public void updateHexs()
    {
        for (int i = 0; i < hexs.Count; i++)
        {
            Destroy(hexs[i].gameObject);
        }
        hexs.Clear();
        CreateHexs = true;
        buildDirection = 0;
        h = 0;
        w = 0;
        SpawnHexNEW();
    }

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

    public void updateMesh()
    {
        InputField tweetData = InputBit.GetComponent<InputField>();
        string inputText = tweetData.text;
        inputText = inputText.ToLower();
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        float[] vowelCount = new float[5];

        for (int i = 0; i < inputText.Length; i++)
        {
            for (int v = 0; v < 5; v++)
            {
                if (inputText[i] == vowels[v])
                {
                    ++vowelCount[v];
                }
            }
        }
        Vector3[] tempVerts =
        {
            new Vector3(-3.0f - 0.33f*vowelCount[0] * meshScale, 0.0f, 0.0f),
            new Vector3(0f, 0f, 2f + 0.2f*vowelCount[1] * meshScale),
            new Vector3(3f + 0.33f*vowelCount[2] * meshScale, 0.0f, 0f),
            new Vector3(2f + 0.2f*vowelCount[3] * meshScale, 0f, -4f - 0.4f*vowelCount[3] * meshScale),
            new Vector3(-2 - 0.2f*vowelCount[4] * meshScale, 0f, -4f - 0.4f*vowelCount[4]*  meshScale),

            new Vector3(-3.0f - 0.33f*vowelCount[0] * meshScale, -1f, 0.0f),
            new Vector3(0f, -1f, 2f + 0.2f*vowelCount[1] * meshScale),
            new Vector3(3f + 0.33f*vowelCount[2] * meshScale, -1f, 0f),
            new Vector3(2f + 0.2f*vowelCount[3] * meshScale, -1f, -4f - 0.4f*vowelCount[3] * meshScale),
            new Vector3(-2 - 0.2f*vowelCount[4] * meshScale, -1f, -4f - 0.4f*vowelCount[4] * meshScale),
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
    }
}