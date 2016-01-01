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
    Color[] newColours;
    public int numOfPals = 0;

    Color grass = Color.Lerp(Color.green, Color.black, 0.5f);
    Color sand = Color.Lerp(Color.yellow, Color.white,0.2f);
    Color wetSand = Color.Lerp(Color.blue, Color.yellow, 0.25f);
    Color rock = Color.gray;

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
    public Vector3[] getVerts()
    {
        return GetComponent<MeshFilter>().mesh.vertices;
    }
    public Color[] getColors()
    {
        return GetComponent<MeshFilter>().mesh.colors;
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
        newColours = new Color[vertices.Length];
        mesh.colors = newColours;
        #endregion

    }
   
    /*public void AddNoiseToMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        
        Vector3[] verts = mesh.vertices;
        
        for (int i=0; i < verts.Length; i++)
        {
            verts[i].y = SmoothRandom.Get(Random.Range(0,50));
        }
        mesh.vertices = verts;
    }*/
    
    public void AddInitialHeight()
    {
        //Finding numeber of pals will have to be a seperate function so..
        //..it can be called before the next bit.
        if (above)
            numOfPals++;
        if (below)
            numOfPals++;
        if (upperLeft)
            numOfPals++;
        if (upperRight)
            numOfPals++;
        if (lowerLeft)
            numOfPals++;
        if (lowerRight)
            numOfPals++;

        if (numOfPals == 6)
        {
            for (int i = 0; i < 7; i++)
            {
                moveVert(i, .2f + ((Random.value - 0.5f)*.25f), sand);
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                moveVert(i, 0f, sand);
            }
        }
    }

    public int weight = 0;
    public void addHeightWeightings()
    {
        //Determine if this is the outer edge, 1 hex in etc.
        if (numOfPals == 6)
        {
            for (int i = 0; i < pals.Length; i++)
            {
                if (pals[i] != null)
                {
                    if (pals[i].GetComponent<HexInfo>().numOfPals == 6)//This is an edge of some sort
                    {
                        weight++;
                    }
                }
            }
        }

        if (weight == 6)//If we have 6 pals that each have 6 pals themsleves...
        {
            //Inner perimeter!
            for (int i = 0; i < 7; i++)
            {
                moveVert(i, 1f + (Random.value - 0.5f), grass);
            }

            for (int i = 0; i < pals.Length; i++)
            {
                for (int j = 0; j < pals.Length; j++)
                {
                    if (pals[i].GetComponent<HexInfo>().pals[j].GetComponent<HexInfo>().numOfPals == 6)
                    { weight++; }
                }
            }
            if (weight==42)
            {
                if (Random.value < .5)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        moveVert(i, 2f+(Random.value-0.5f), grass);
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        moveVert(i, 2f + (Random.value - 0.5f), rock);
                    }
                }

            }
        }
    }

    public void addMountains()
    {
        int secretInt = 0;
        if (above && below && upperLeft && upperRight && lowerLeft && lowerRight)
        {
            for (int i = 0; i < pals.Length; i++)
            {
                    if (pals[i].GetComponent<HexInfo>().weight == 42)
                    {
                        secretInt++;
                    }
            }
            if (secretInt == 6)
            {
                for (int i = 0; i < 7; i++)
                {
                    moveVert(i, 3f + (Random.value - 0.5f), rock);
                }
            }
        }
    }

    public Color yuk;
    public void blendCols(int vertNum)
    {
        Color[] myCols = getColors();
        Vector3[] myVerts = getVerts();

        switch (vertNum)
        {
            case 0:
                #region 0
                if (below && lowerLeft)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    var borderHex2 = pals[0].GetComponent<HexInfo>();
                    Color[] border2Cols = borderHex2.getColors();
                    Color[] borderCols = borderHex.getColors();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newCol = (borderCols[2] + border2Cols[4] + myCols[vertNum]) / 3;

                    moveVert(0, myVerts[0].y, newCol);
                    borderHex.moveVert(2, borderVerts[2].y, newCol);
                    borderHex2.moveVert(4, border2Verts[4].y, newCol);
                }
                break;
            #endregion
            case 1:
                #region 1
                if (upperLeft && lowerLeft)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    var borderHex2 = pals[0].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Vector3[] border2Verts = borderHex2.getVerts();
                    Color[] borderCols = borderHex.getColors();
                    Color[] border2Cols = borderHex2.getColors();

                    var newCol = (borderCols[5] + border2Cols[3] + myCols[vertNum]) / 3;

                    moveVert(vertNum, myVerts[vertNum].y, newCol);
                    borderHex.moveVert(5, borderVerts[5].y, newCol);
                    borderHex2.moveVert(3, border2Verts[3].y, newCol);
                }
                break;
            #endregion
            case 2:
                #region 2
                if (upperLeft && above)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    var borderHex2 = pals[2].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Vector3[] border2Verts = borderHex2.getVerts();
                    Color[] borderCols = borderHex.getColors();
                    Color[] border2Cols = borderHex2.getColors();

                    var newCol = (borderCols[4] + border2Cols[0] + myCols[vertNum]) / 3;

                    moveVert(vertNum, myVerts[vertNum].y, newCol);
                    borderHex.moveVert(4, borderVerts[4].y, newCol);
                    borderHex2.moveVert(0, border2Verts[0].y, newCol);
                }
                break;
            #endregion
            case 3:
                #region 3
                if (upperRight && above)
                {
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    var borderHex2 = pals[3].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Color[] borderCols = borderHex.getColors();
                    Color[] border2Cols = borderHex2.getColors();

                    var newCol = (borderCols[5] + border2Cols[1] + myCols[vertNum]) / 3;

                    moveVert(vertNum, myVerts[vertNum].y, newCol);
                    borderHex.moveVert(5, borderVerts[5].y, newCol);
                    borderHex2.moveVert(1, border2Verts[1].y, newCol);
                }
                break;
            #endregion
            case 4:
                #region 4
                if (upperRight && lowerRight)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    var borderHex2 = pals[4].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Color[] borderCols = borderHex.getColors();
                    Color[] border2Cols = borderHex2.getColors();

                    var newCol = (borderCols[0] + border2Cols[2] + myCols[vertNum]) / 3;

                    moveVert(vertNum, myVerts[vertNum].y, newCol);
                    borderHex.moveVert(0, borderVerts[0].y, newCol);
                    borderHex2.moveVert(2, border2Verts[2].y, newCol);
                }
                break;
            #endregion
            case 5:
                #region 5
                if (below && lowerRight)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    var borderHex2 = pals[4].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();
                    Vector3[] borderVerts = borderHex.getVerts();
                    Color[] borderCols = borderHex.getColors();
                    Color[] border2Cols = borderHex2.getColors();

                    var newCol = (borderCols[3] + border2Cols[1] + myCols[vertNum]) / 3;

                    moveVert(vertNum, myVerts[vertNum].y, newCol);
                    borderHex.moveVert(3, borderVerts[3].y, newCol);
                    borderHex2.moveVert(1, border2Verts[1].y, newCol);
                }
                break;
            #endregion
            case 6:
                var newColy = (myCols[0] + myCols[1] + myCols[2] + myCols[3] + myCols[4] + myCols[5]) / 6;
                moveVert(6, myVerts[6].y, Color.Lerp(myCols[6], newColy, 0.5f));
                yuk = newColy;
                break;
        }
    }

    public void interlopeCorner(int vertNum)
    {
        Vector3[] myVerts = getVerts();

        switch (vertNum)
        {
            case 0:
                #region 0
                if (below && lowerLeft)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[0].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[2].y + border2Verts[4].y + myVerts[0].y) / 3;

                    moveVert(0, newPoint, Color.black);
                    borderHex.moveVert(2, newPoint, Color.black);
                    borderHex2.moveVert(4, newPoint, Color.black);
                }
                else if (below)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(2, -0.25f, wetSand);
                }
                else if (lowerLeft)
                {
                    var borderHex = pals[0].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(4, -0.25f, wetSand);
                }
                else
                {
                    //This is a beach border
                    moveVert(vertNum, -0.5f, wetSand);
                }
                break;
            #endregion
            case 1:
                #region 1
                if (upperLeft && lowerLeft)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[0].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[5].y + border2Verts[3].y + myVerts[vertNum].y) / 3;
                    
                        moveVert(vertNum, newPoint, Color.black);
                        borderHex.moveVert(5, newPoint, Color.black);
                        borderHex2.moveVert(3, newPoint, Color.black);
                }
                else if (upperLeft)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(5, -0.25f, wetSand);
                }
                else if (lowerLeft)
                {
                    var borderHex = pals[0].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(3, -0.25f, wetSand);
                }
                else
                {
                    //This is a beach border
                    moveVert(vertNum, -0.5f, wetSand);
                }
                break;
            #endregion
            case 2:
                #region 2
                if (upperLeft && above)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[2].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[4].y + border2Verts[0].y + myVerts[vertNum].y) / 3;

                    moveVert(vertNum, newPoint, Color.black);
                    borderHex.moveVert(4, newPoint, Color.black);
                    borderHex2.moveVert(0, newPoint, Color.black);
                }
                else if (upperLeft)
                {
                    //This is a beach border
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(4, -0.25f, wetSand);
                }
                else if (above)
                {
                    //This is a beach border
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(0, -0.25f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 3:
                #region 3
                if (upperRight && above)
                {
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[3].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[5].y + border2Verts[1].y + myVerts[vertNum].y) / 3;

                    moveVert(vertNum, newPoint, Color.black);
                    borderHex.moveVert(5, newPoint, Color.black);
                    borderHex2.moveVert(1, newPoint, Color.black);
                }
                else if (above)
                {
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(5, -0.25f, wetSand);
                }
                else if (upperRight)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(1, -0.25f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 4:
                #region 4
                if (upperRight && lowerRight)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[4].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[0].y + border2Verts[2].y + myVerts[vertNum].y) / 3;
                    moveVert(vertNum, newPoint, Color.black);
                    borderHex.moveVert(0, newPoint, Color.black);
                    borderHex2.moveVert(2, newPoint, Color.black);
                }
                else if (upperRight)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(0, -0.25f, wetSand);
                }
                else if (lowerRight)
                {
                    var borderHex = pals[4].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(2, -0.25f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 5:
                #region 5 
                if (below && lowerRight)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    Vector3[] borderVerts = borderHex.getVerts();
                    var borderHex2 = pals[4].GetComponent<HexInfo>();
                    Vector3[] border2Verts = borderHex2.getVerts();

                    var newPoint = (borderVerts[3].y + border2Verts[1].y + myVerts[vertNum].y) / 3;

                    moveVert(vertNum, newPoint, Color.black);
                    borderHex.moveVert(3, newPoint, Color.black);
                    borderHex2.moveVert(1, newPoint, Color.black);
                }
                else if (lowerRight)
                {
                    var borderHex = pals[4].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(1, -0.25f, wetSand);
                }
                else if (below)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.25f, wetSand);
                    borderHex.moveVert(3, -0.25f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 6:
                var newPointy = (myVerts[0].y + myVerts[1].y + myVerts[2].y + myVerts[3].y + myVerts[4].y + myVerts[5].y) / 6;
                moveVert(6, newPointy, Color.black);
                break;
        }
    }
    
    public void moveVert(int vertNum, float height, Color newColour)
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

        Heights[vertNum] = height;

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

        #region vertex colours
        if (newColour != Color.black)
        {
            newColours[vertNum] = newColour;
            mesh.colors = newColours;
        }
        #endregion

        mesh.vertices = newVerts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //Don't need this for the time being
        /*DestroyImmediate(GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;*/
    }
}