/*using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{

    #region awake
    public void Awake()
    {
        //get the flat hexagons size; we use this to space out the hexagons and chunks
        GetHexProperties();
        //generate the chunks of the world
        GenerateMap();
    }
    #endregion

    #region fields
    public Mesh flatHexagonSharedMesh;
    public float hexRadiusSize;

    //hexInstances
    [HideInInspector]
    public Vector3 hexExt;
    [HideInInspector]
    public Vector3 hexSize;
    [HideInInspector]
    public Vector3 hexCenter;
    [HideInInspector]
    public GameObject chunkHolder;

    public Texture2D terrainTexture;

    int xSectors;
    int zSectors;

    public HexChunk[,] hexChunks;

    public Vector2 mapSize;
    public int chunkSize;
    #endregion

    #region GetHexProperties
    private void GetHexProperties()
    {
        //Creates mesh to calculate bounds
        GameObject inst = new GameObject("Bounds Set Up: Flat");
        //add mesh filter to our temp object
        inst.AddComponent<MeshFilter>();
        //add a renderer to our temp object
        inst.AddComponent<MeshRenderer>();
        //add a mesh collider to our temp object; this is for determining dimensions cheaply and easily
        inst.AddComponent<MeshCollider>();
        //reset the position to global zero
        inst.transform.position = Vector3.zero;
        //reset all rotation
        inst.transform.rotation = Quaternion.identity;


        Vector3[] vertices;
        int[] triangles;

        float floorLevel = 0;
        //positions vertices of the hexagon to make a normal hexagon
        vertices = new Vector3[]
        {
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(3+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(3+0.5)/5)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(2+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(2+0.5)/5)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(1+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(1+0.5)/5)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(0+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(0+0.5)/5)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(5+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(5+0.5)/5)))),
            new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(4+0.5)/5))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(4+0.5)/5))))
        };

        //triangles connecting the verts
        triangles = new int[]
        {
            1,5,0,
            1,4,5,
            1,2,4,
            2,3,4
        };

        Vector2[] uv;
        uv = new Vector2[]
        {
            new Vector2(0,0.25f),
            new Vector2(0,0.75f),
            new Vector2(0.5f,1),
            new Vector2(1,0.75f),
            new Vector2(1,0.25f),
            new Vector2(0.5f,0),
         };

        //create new mesh to hold the data for the flat hexagon
        flatHexagonSharedMesh = new Mesh();
        //assign verts
        flatHexagonSharedMesh.vertices = vertices;
        //assign triangles
        flatHexagonSharedMesh.triangles = triangles;
        //assign uv
        flatHexagonSharedMesh.uv = uv;
        //set temp gameObject's mesh to the flat hexagon mesh
        inst.GetComponent<MeshFilter>().mesh = flatHexagonSharedMesh;
        //make object play nicely with lighting
        inst.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //set mesh collider's mesh to the flat hexagon
        inst.GetComponent<MeshCollider>().sharedMesh = flatHexagonSharedMesh;

        //calculate the extents of the flat hexagon

        hexExt = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.extents.x, inst.gameObject.GetComponent<Collider>().bounds.extents.y, inst.gameObject.GetComponent<Collider>().bounds.extents.z);
        hexExt = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.extents.x, inst.gameObject.GetComponent<Collider>().bounds.extents.y, inst.gameObject.GetComponent<Collider>().bounds.extents.z);
        //calculate the size of the flat hexagon
        hexSize = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.size.x, inst.gameObject.GetComponent<Collider>().bounds.size.y, inst.gameObject.GetComponent<Collider>().bounds.size.z);
        //calculate the center of the flat hexagon
        hexCenter = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.center.x, inst.gameObject.GetComponent<Collider>().bounds.center.y, inst.gameObject.GetComponent<Collider>().bounds.center.z);
        //destroy the temp object that we used to calculate the flat hexagon's size
        Destroy(inst);
    }
    #endregion

    #region GenerateMap
    void GenerateMap()
    {
        //determine number of chunks
        xSectors = Mathf.CeilToInt(mapSize.x / chunkSize);
        zSectors = Mathf.CeilToInt(mapSize.y / chunkSize);
        hexChunks = new HexChunk[xSectors, zSectors];

        //Cycle through all the chunks
        for (int x = 0; x < xSectors; x++)
        {
            for (int z = 0; z < zSectors; z++)
            {
                //create the new chunk
                hexChunks[x, z] = NewChunk(x, z);
                //set the position of the new chunk
                hexChunks[x, z].gameObject.transform.position = new Vector3(x * (chunkSize * hexSize.x), 0f, (z * (chunkSize * hexSize.z) * (.75f)));
                //set hex size for hexagon positioning
                hexChunks[x, z].hexSize = hexSize;
                //set the number of hexagons for the chunk to generate
                hexChunks[x, z].SetSize(chunkSize, chunkSize);
                //the width interval of the chunk
                hexChunks[x, z].xSector = x;
                //set the height interval of the chunk
                hexChunks[x, z].ySector = z;
                //assign the world manager(this)
                hexChunks[x, z].worldManager = this;
            }
        }
    }
    #endregion

    #region NewChunk

    #endregion
}

public class HexChunk
{
    #region fields
    [SerializeField]
    public HexInfo[,] hexArray;
    public int xSize;
    public int ySize;
    public Vector3 hexSize;

    //set by world master
    public int xSector;
    public int ySector;
    public WorldManager worldManager;

    private MeshFilter filter;
    private new BoxCollider collider;
    #endregion

    public void SetSize(int x, int y)
    {
        xSize = x;
        ySize = y;
    }

    public void OnDestroy()
    {
        //Destroy(renderer.material);
    }

    public void AllocateHexArray()
    {
        hexArray = new HexInfo[xSize, ySize];
    }

    public void Begin()
    {
        GenerateChunk();
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < ySize; z++)
            {
                if (hexArray[x, z] != null)
                {
                    hexArray[x, z].parentChunk = this;
                    hexArray[x, z].Start();
                }
                else
                {
                    print("null hexagon found in memory");
                }
            }
        }
        Combine();
    }
}*/