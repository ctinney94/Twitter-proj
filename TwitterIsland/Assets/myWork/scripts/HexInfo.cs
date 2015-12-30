using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexInfo : MonoBehaviour
{
	//basic hexagon mesh making
	public Vector3[] Vertices;
	public Vector2[] uv;
	public int[] Triangles;
    public Material mat;
    public Rigidbody rig;
    //public float scale = 0.25f;

    void Update()
    {
    }

    public bool above, below, upperLeft, upperRight, lowerRight, lowerLeft;
    public GameObject[] pals = { null, null, null, null, null, null };    

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material = GameObject.Find("PentTest").GetComponent<PentInfo>().wireframeMat;
    }
    void OnMouseExit()
    {
        GetComponent<Renderer>().material = GameObject.Find("PentTest").GetComponent<PentInfo>().HexMat;
    }

    public void MeshSetup(float scale)
	{
        #region verts
        float floorLevel = 0f;
        Vector3[] vertices =
        {
            //These are the values required to create a hexagon with a side length of 6
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
   
    public void AddNoiseToMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        
        Vector3[] verts = mesh.vertices;
        
        for (int i=0; i < verts.Length; i++)
        {
            verts[i].y = SmoothRandom.Get(Random.Range(0,50));
        }
        mesh.vertices = verts;
    }

    public void nameMe()
    {
        int pals=0;

        if (above)
            pals++;
        if (upperLeft)
            pals++;
        if (lowerLeft)
            pals++;
        if (below)
            pals++;
        if (lowerRight)
            pals++;
        if (upperRight)
            pals++;

        if (pals == 1)
            GetComponent<Renderer>().material.SetFloat("_Blend1",0);
        if (pals == 2)
            GetComponent<Renderer>().material.SetFloat("_Blend1", 0.1f);
        if (pals == 3)
            GetComponent<Renderer>().material.SetFloat("_Blend1", 0.25f);
        if (pals == 4)
            GetComponent<Renderer>().material.SetFloat("_Blend1", 0.75f);
        if (pals == 5)
            GetComponent<Renderer>().material.SetFloat("_Blend1", 0.9f);
        if (pals == 6)
            GetComponent<Renderer>().material.SetFloat("_Blend1", 1f);
    }

    public void workOnMe()
    {
        //Create vertex colouring thats blends colours based on;
        //  Height
        //  Number of boundering hexagons, and where

        if (above)
        {
            moveVert(2, 0.5f);
            moveVert(3, 0.5f);
            moveVert(6, 0.25f);
        }
        if (upperLeft)
        {
            moveVert(2, 0.5f);
            moveVert(1, 0.5f);
            moveVert(6, 0.25f);
        }
        if (lowerLeft)
        {
            moveVert(0, 0.5f);
            moveVert(1, 0.5f);
            moveVert(6, 0.25f);
        }
        if (below)
        {
            moveVert(0, 0.5f);
            moveVert(5, 0.5f);
            moveVert(6, 0.25f);
        }
        if (lowerRight)
        {
            moveVert(4, 0.5f);
            moveVert(5, 0.5f);
            moveVert(6, 0.25f);
        }
        if (upperRight)
        {
            moveVert(3, 0.5f);
            moveVert(4, 0.5f);
            moveVert(6, 0.25f);
        }

        //Add height to vertices
        //First try with an entire hexagons
        //Then try moving the corresponding vertices of surrounding 
    }

    public void moveVert(int vertNum, float height)
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] oldVerts = mesh.vertices;

        float[] Heights =
            {
            oldVerts[0].y,
            oldVerts[1].y,
            oldVerts[2].y,
            oldVerts[3].y,
            oldVerts[4].y,
            oldVerts[5].y,
            oldVerts[6].y
        };

        Heights[vertNum] += height;

        Vector3[] newVerts =
        {
            new Vector3(oldVerts[0].x, Heights[0], oldVerts[0].z),    //0
            new Vector3(oldVerts[1].x, Heights[1], oldVerts[1].z),    //1
            new Vector3(oldVerts[2].x, Heights[2], oldVerts[2].z),    //2
            new Vector3(oldVerts[3].x, Heights[3], oldVerts[3].z),    //3
            new Vector3(oldVerts[4].x, Heights[4], oldVerts[4].z),    //4
            new Vector3(oldVerts[5].x, Heights[5], oldVerts[5].z),    //5
            new Vector3(oldVerts[6].x, Heights[6], oldVerts[6].z),    //6
        };

        mesh.vertices = newVerts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //Don't need this for the time being
        /*DestroyImmediate(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;*/
    }
}