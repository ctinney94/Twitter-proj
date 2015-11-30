using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PentInfo : MonoBehaviour
{
    //basic pentgon mesh making
	public Vector3[] pentVerts;
	public Vector2[] uv;
	public int[] Triangles;
    Mesh mesh;
    public GameObject InputBit;
    public Material mat;
    float h = 0;
    
	void Start()
	{
        MeshSetup();
        //Fill this pentagon in with hexagons.

	}

    void SpawnHexs()
    {
        bool insidePent= false;
        //Create a new hexagon
        while (insidePent)
        {
            GameObject hex = new GameObject("A hexagon");
            HexInfo hexinf = hex.AddComponent<HexInfo>();
            hexinf.mat = mat;
            //Move it into position
            hex.transform.Translate(h * 2, 0.2f, 0);
            h++;
        }
    }
        

    void MeshSetup()
    {
        #region verts

        float floorLevel = 0;
        Vector3[] pentVerts =
        {
            new Vector3(-3f, floorLevel, 0f),  //0
            new Vector3(0f, floorLevel, 2f ),  //1
            new Vector3(3f, floorLevel, 0f),   //2
            new Vector3(2f , floorLevel, -4f), //3
            new Vector3(-2, floorLevel, -4f),  //4
        };
        #endregion

        #region triangles

        int[] triangles = 
       {
            4,0,1,
            4,1,3,
            1,2,3
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
        mesh.uv = uv;

        //Material mat = Resources.Load("something") as Material;
        meshRenderer.material = mat;

        //make it play nicely with lighting
        mesh.RecalculateNormals();
        mesh.name = "Pentagonal";
        //set the GO's meshFilter's mesh to be the one we just made
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        //UV TESTING
        //renderer.material.mainTexture = texture;
        
        #endregion
    }

    void Update()
    {
        InputField tweetData = InputBit.GetComponent<InputField>();
        updateMesh(tweetData.text);

        //SpawnHexs();
       
        if (mesh.bounds.Contains(GameObject.FindGameObjectWithTag("TEST").transform.position))
        {
            GameObject.FindGameObjectWithTag("TEST").GetComponent<Renderer>().material.color = Color.green;
        }
        else
            GameObject.FindGameObjectWithTag("TEST").GetComponent<Renderer>().material.color = Color.red;
    }

    void updateMesh(string inputText)
    {
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

        /*mesh.vertices[0] = new Vector3(-3.0f - 0.33f * vowelCount[0], 0.0f, 0.0f);
        mesh.vertices[1] = new Vector3(0f, 0f, 2f + 0.2f * vowelCount[1]);
        mesh.vertices[2] = new Vector3(3f + 0.33f * vowelCount[2], 0.0f, 0f);
        mesh.vertices[3] = new Vector3(2f + 0.2f * vowelCount[3], 0f, -4f - 0.4f * vowelCount[3]);
        mesh.vertices[4] = new Vector3(-2 - 0.2f * vowelCount[4], 0f, -4f - 0.4f * vowelCount[4]);*/
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.vertices = tempVerts;

        //Is this line actually did something, that'd be fucking marvelous, but noooo
        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        //I am destorying and remaking the collider EVERY FRAME
        //If anyone find this I want them to know I'm not proud of what I've done here
        Destroy(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}