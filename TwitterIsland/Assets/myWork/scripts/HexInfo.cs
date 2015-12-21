using UnityEngine;
using System.Collections;

public class HexInfo : MonoBehaviour
{
	//basic hexagon mesh making
	public Vector3[] Vertices;
	public Vector2[] uv;
	public int[] Triangles;
    public Material mat;
    public Rigidbody rig;
    //public float scale = 0.25f;

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.shader = Shader.Find("Custom/wireframe");
    }
    void OnMouseExit()
    {
        GetComponent<Renderer>().material.shader = Shader.Find("Standard");
    }

    public void MeshSetup(float scale)
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
            new Vector3(3f, floorLevel, -Mathf.Sqrt(36-9)),      //5
            new Vector3(0,0,0),
        };

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = Vector3.Scale(vertices[i], new Vector3(scale, 1, scale));
        //Based off this hex here:
        //https://s-media-cache-ak0.pinimg.com/236x/7e/f2/a7/7ef2a733a6fa0fe4ae0d64cfbb1f5b2c.jpg
        #endregion


        #region triangles

        int[] triangles = 
       {
            1,6,0,
            2,6,1,
            3,6,2,
            4,6,3,
            5,6,4,
            0,6,5,

            /*1,5,0,
            1,4,5,
            1,2,4,
            2,3,4*/
       };
        #endregion

        #region uv
        uv = new Vector2[]
        {
            new Vector2(0.25f,0),   //0
            new Vector2(0,0.5f),    //1
            new Vector2(0.25f,1),   //2
            new Vector2(0.75f,1),   //3
            new Vector2(1f,0.5f),   //4
            new Vector2(0.75f,0),   //5
            new Vector2(0.5f,0.5f)  //6
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
        rig = gameObject.AddComponent<Rigidbody>();
        rig.isKinematic = true;
        meshCol.sharedMesh = mesh;
        //UV TESTING
        //renderer.material.mainTexture = texture;

        #endregion

    }
    
}