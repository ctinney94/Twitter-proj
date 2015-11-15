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

	void Start()
	{
		MeshSetup();
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

        //set the GO's meshFilter's mesh to be the one we just made
        meshFilter.mesh = mesh;

        //UV TESTING
        //renderer.material.mainTexture = texture;
        
        #endregion
    }

    void Update()
    {
        InputField tweetData = InputBit.GetComponent<InputField>();
        updateMesh(tweetData.text);

        Bounds meshBounds = mesh.bounds;
        if (mesh.bounds.Contains(GameObject.FindGameObjectWithTag("TEST").transform.position))
        {
            Debug.Log("DJAKSDJALSK");
        }
        else
            Debug.Log("naw");
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
            new Vector3(-3.0f - 0.5f*vowelCount[0], 0.0f, 0.0f),
            new Vector3(0f, 0f, 2f + 0.5f*vowelCount[1]),
            new Vector3(3f + 0.5f*vowelCount[2], 0.0f, 0f),
            new Vector3(2f + 0.5f*vowelCount[3], 0f, -4f - vowelCount[3]),
            new Vector3(-2 - 0.5f*vowelCount[4], 0f, -4f - vowelCount[4]),
        };
        mesh.RecalculateBounds();

        mesh.vertices = tempVerts;
    }
}