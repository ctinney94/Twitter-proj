using UnityEngine;
using System.Collections;

public class HexInfo : MonoBehaviour
{
	//basic hexagon mesh making
	public Vector3[] Vertices;
	public Vector2[] uv;
	public int[] Triangles;
    public Material mat;
    public float scale = 0.25f;

    void Start()
	{
        MeshSetup();
	}
	
	public void MeshSetup()
	{
        #region verts
        float floorLevel = 0f;
        Vector3[] vertices =
        {
            new Vector3(-3f, floorLevel, -Mathf.Sqrt(36-9)),    //0
            new Vector3(-6f, floorLevel, 0),                    //1
            new Vector3(-3f, floorLevel, Mathf.Sqrt(36-9)),     //2
            new Vector3(3f, floorLevel, Mathf.Sqrt(36-9)),      //3
            new Vector3(6f, floorLevel, 0),                     //4
            new Vector3(3f, floorLevel, -Mathf.Sqrt(36-9))      //5
        };
        //Based off this hex here:
        //https://s-media-cache-ak0.pinimg.com/236x/7e/f2/a7/7ef2a733a6fa0fe4ae0d64cfbb1f5b2c.jpg
        #endregion

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = Vector3.Scale(vertices[i], new Vector3(scale, scale, scale));

        #region triangles

        int[] triangles = 
       {
            1,5,0,
            1,4,5,
            1,2,4,
            2,3,4
       };
        #endregion

        #region uv
        uv = new Vector2[]
        {
            new Vector2(0,0.25f),
            new Vector2(0,0.75f),
            new Vector2(0.5f,1),
            new Vector2(1,0.75f),
            new Vector2(1,0.25f),
            new Vector2(0.5f,0),
        };
        #endregion

        #region finalize

        //add a mesh filter to the GameObject the script is attached to; cache it for later
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //add a mesh renderer to the GO the script is attached to

        MeshCollider meshCol = gameObject.AddComponent<MeshCollider>();

        //create a mesh object to pass our data into
        Mesh mesh = new Mesh();

        //add our vertices to the mesh
        mesh.vertices = vertices;
        //add our triangles to the mesh
        mesh.triangles = triangles;
        //add out UV coordinates to the mesh
        mesh.uv = uv;

        //and material
        meshRenderer.material = mat;
        
        //make it play nicely with lighting
        mesh.RecalculateNormals();

        //set the GO's meshFilter's mesh to be the one we just made
        meshFilter.mesh = mesh;
        meshCol.sharedMesh = mesh;
        meshCol.sharedMesh = mesh;

        //UV TESTING
        //renderer.material.mainTexture = texture;

        #endregion

    }
    
}