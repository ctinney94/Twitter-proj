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
    Mesh mesh;
    public GameObject InputBit;
    public Material PentMat, HexMat;
    bool insidePent = true;
    List<GameObject> hexs = new List<GameObject>();

    float h = 0, w = 0;

    void Start()
	{
        MeshSetup();
        //Fill this pentagon in with hexagons.
	}
    void SpawnHexs()
    {
        //Create a new hexagon
        while (insidePent)
        {
            //Make a hex
            GameObject hex = new GameObject("hex " + h +","+ w);
            HexInfo hexinf = hex.AddComponent<HexInfo>();
            hexinf.mat = HexMat;

            //Move it into position
            float x = w * 1.5f * (hexinf.scale * 6);
            float y;
            if (w % 2 != 0)//If w is odd
            {
                    y = h * 2*(hexinf.scale * Mathf.Sqrt(36 - 9));
            }
            else
            {
                y = (h * 2*(hexinf.scale * Mathf.Sqrt(36 - 9))) +(hexinf.scale * Mathf.Sqrt(36 - 9));
            }
            hex.transform.Translate(x, 0f, y);
            //hex.transform.Translate(0f, 0, h * 2 * (hexinf.scale * Mathf.Sqrt(36 - 9)));

            w++;
            hexs.Add(hex);
            if (!mesh.bounds.Contains(GameObject.Find(hex.name).transform.position))
            {
                //insidePent = false;
                h++;
                w = 0;
                hexs.Remove(hex);
                Destroy(hex);
            }
            if (h > 10)
                insidePent = false;
            //h++;
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i=0; i < hexs.Count; i++)
            {
                Destroy(hexs[i].gameObject);
            }
            hexs.Clear();
            insidePent = true;
            h = 0;
            w = 0;                        
        }
    }
    
    void OnTriggerStay(Collider col)
    {
        Debug.Log(col.gameObject.name + "is in me");
    }
    void OnCollisionStay(Collision col)
    {
        //Debug.Log(col.gameObject.name + "is in me");
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
            /*new Vector3(-3f, -floorLevel, 0f),  //0
            new Vector3(0f, -floorLevel, 2f ),  //1
            new Vector3(3f, -floorLevel, 0f),   //2
            new Vector3(2f , -floorLevel, -4f), //3
            new Vector3(-2, -floorLevel, -4f), */ //4
        };
        #endregion

        #region triangles

        int[] triangles =
       {
            1,4,0,
            1,3,4,
            1,2,3
            /*old tris
            4,0,1,
            4,1,3,
            1,2,3,*/

            /*
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
            6,7,8*/
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

        //add a mesh filter to the GameObject the script is attached to; cache it for later
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        //add a mesh renderer to the GO the script is attached to
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
       
        //add our vertices to the mesh
        mesh.vertices = pentVerts;
        //add our triangles to the mesh
        mesh.triangles = triangles;
        //add out UV coordinates to the mesh
       // mesh.uv = uv;

        //Material mat = Resources.Load("something") as Material;
        meshRenderer.material = PentMat;

        //make it play nicely with lighting
        mesh.RecalculateNormals();
        mesh.name = "Pentagonal";
        //set the GO's meshFilter's mesh to be the one we just made
        meshFilter.mesh = mesh;

        meshCollider.sharedMesh = mesh;
        // meshCollider.convex = true;
        //meshCollider.isTrigger = true;

        //UV TESTING
        //renderer.material.mainTexture = texture;

        #endregion
    }

    void Update()
    {
        SpawnHexs();
       
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
            new Vector3(-3.0f - 0.33f*vowelCount[0], 0.0f, 0.0f),
            new Vector3(0f, 0f, 2f + 0.2f*vowelCount[1]),
            new Vector3(3f + 0.33f*vowelCount[2], 0.0f, 0f),
            new Vector3(2f + 0.2f*vowelCount[3], 0f, -4f - 0.4f*vowelCount[3]),
            new Vector3(-2 - 0.2f*vowelCount[4], 0f, -4f - 0.4f*vowelCount[4]),
        };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.vertices = tempVerts;

        //Is this line actually did something, that'd be fucking marvelous, but noooo
        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        //I am destorying and remaking the collider EVERY FRAME
        //If anyone find this I want them to know I'm not proud of what I've done here
        Destroy(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        // meshCollider.convex = true;
        //meshCollider.isTrigger = true;
        meshCollider.sharedMesh = mesh;
    }
}