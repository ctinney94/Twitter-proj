using UnityEngine;
using System.Collections;

public class HexInfo : MonoBehaviour
{
	//basic hexagon mesh making
	public Vector3[] Vertices;
	public Vector2[] uv;
	public int[] Triangles;
    public Material mat;
	
	void Start()
	{
        MeshSetup();
	}
	
	public void MeshSetup()
	{
        #region verts
        float floorLevel = 0.1f;
        Vector3[] vertices =  
        {
            new Vector3(-1f, floorLevel, -.5f),
            new Vector3(-1f, floorLevel, .5f),
            new Vector3(0f, floorLevel, 1f),
            new Vector3(1f, floorLevel, .5f),
            new Vector3(1f, floorLevel, -.5f),
            new Vector3(0f, floorLevel, -1f)
        };
        #endregion

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

        //UV TESTING
        //renderer.material.mainTexture = texture;
        
        #endregion

    }

    void OnCollisionStay(Collision col)
    {

    }
}