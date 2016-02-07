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
    Color sand = Color.Lerp(Color.yellow, Color.white, 0.2f);
    Color wetSand = Color.Lerp(Color.blue, Color.yellow, 0.25f);
    Color rock = Color.gray;
    Color dirt = new Color(0.96f, 0.64f, 0.38f);
    public GameObject camp;

    public GameObject[] pals = { null, null, null, null, null, null };

    void OnMouseDown()
    {
        Color centreColour = getColors()[6];
        if (centreColour == Color.Lerp(Color.black, grass, 0.8f) || centreColour == grass)
        {
            GameObject poof = Instantiate(Resources.Load("forestPoof") as GameObject);
            poof.transform.position = transform.position + getVerts()[6];
        }
        else if (centreColour == sand || centreColour == Color.Lerp(Color.white, sand, 0.75f))
        {
            GameObject poof = Instantiate(Resources.Load("sandPoof") as GameObject);
            poof.transform.position = transform.position + getVerts()[6]; ;
        }
        else if (centreColour == rock || centreColour == Color.Lerp(Color.black, dirt, 0.5f))
        {
            GameObject poof = Instantiate(Resources.Load("rockPoof") as GameObject);
            poof.transform.position = transform.position + getVerts()[6];
        }
        else if (centreColour == Color.white)
        {
            GameObject poof = Instantiate(Resources.Load("snowPoof") as GameObject);
            poof.transform.position = transform.position + getVerts()[6];
        }
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
        Vector3[] initVerts =
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

        for (int i = 0; i < initVerts.Length; i++)
            initVerts[i] = Vector3.Scale(initVerts[i], new Vector3(scale, 1, scale));
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
        //create a mesh object to pass our data into
        Mesh mesh = new Mesh();
        //add our vertices to the mesh
        mesh.vertices = initVerts;
        //add our triangles to the mesh
        mesh.triangles = triangles;
        //add out UV coordinates to the mesh
        mesh.uv = uv;

        //add a mesh filter to the GameObject the script is attached to; cache it for later
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //add a mesh renderer to the GO the script is attached to

        MeshCollider meshCol = gameObject.AddComponent<MeshCollider>();

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
        newColours = new Color[initVerts.Length];
        mesh.colors = newColours;
        #endregion
    }

    public int[] dirWeightings = { 0, 0, 0, 0, 0, 0 };
    public float avg, most, least;
    public void hexWeighter(int totalHexs)
    {
        for (int i = 0; i < pals.Length; i++)
        {
            if (pals[i] != null)
                numOfPals++;
        }

        for (int i = 0; i < pals.Length; i++)
        {
            if (pals[i] != null)
            {
                dirWeightings[i]++;//We have at least 1 pal in this direction :)

                var nextHex = getNextPals(pals[i], i);
                do
                {
                    if (nextHex != null)
                    {
                        dirWeightings[i]++;
                        nextHex = getNextPals(nextHex, i);
                    }
                } while (nextHex != null);
            }
        }
        avg = (dirWeightings[0] + dirWeightings[1] + dirWeightings[2] + dirWeightings[3] + dirWeightings[4] + dirWeightings[5]) / 6;
        for (int i = 0; i < 6; i++)
        {
            if (dirWeightings[i] > most)
                most = dirWeightings[i];
        }

        least = 1000;
        for (int i = 0; i < 6; i++)
        {
            if (dirWeightings[i] < least)
                least = dirWeightings[i];
        }
    }

    public void addHeight(float maxLeast, float heightScale, int numOfHexs)
    {
        if (numOfHexs < 12)
        {
            for (int i = 0; i < 7; i++)
            {
                moveVert(i, numOfPals * 0.5f * heightScale, sand);
            }
        }
        else
        {
            float dist = least / maxLeast;
            //% of closeness to centre
            //0 is beach
            int random = 0;
            if (Random.value > 0.8f)
            {
                random++;
            }
            if (dist < 0.25f)
            {
                for (int i = 0; i < 7; i++)
                {
                    moveVert(i, ((least / 5) * heightScale), sand);
                }
            }
            else if (dist > 0.249f && dist < 0.5f)
            {
                for (int i = 0; i < 7; i++)
                {
                    moveVert(i, ((least / 3) * heightScale) + random, grass);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    //this peaks should rise exptonentially
                    //well not quite exponentially, but it sho
                    //moveVert(i, ((least * least) * .1f) + ((avg - offset - 2) * .3f) + random, rock);

                    float startingPoint = (maxLeast / 2);

                    float hexsIn = least - startingPoint;
                    moveVert(i, (((least / 3) * heightScale) + ((hexsIn * 2) * heightScale)) + random, rock);
                }
            }
        }
    }

    public void heightColour(float maxLeast)
    {
        Vertices = getVerts();

        var ran = Random.value;
        //Add basic colours for sand, grass and the wetsand around the base of the island
        for (int i = 0; i < 7; i++)
        {

            if (Vertices[i].y > 0.5f && Vertices[i].y < 6 || (least / maxLeast) > 0.25f)
            {
                if (ran > 0.25f)
                    moveVert(i, -99, grass);
                else
                    moveVert(i, -99, Color.Lerp(Color.black, grass, 0.8f));
            }
            if (Vertices[i].y < 0.6f)
            {

                if (ran > 0.25f)
                    moveVert(i, -99, sand);
                else
                    moveVert(i, -99, Color.Lerp(Color.white, sand, 0.75f));
            }
        }

        //If gradient is mighty big, add some rocky hillside
        for (int i = 0; i < 7; i++)
        {
            int vertTocCheck = i + 3;
            if (vertTocCheck > 5)
                vertTocCheck -= 6;
            float gradient = Vertices[vertTocCheck].y - Vertices[i].y;

            if (gradient > 1f || gradient < -1f)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (ran > 0.15f)
                        moveVert(j, -99, rock);
                    else
                        moveVert(j, -99, Color.Lerp(Color.black, dirt, 0.5f));
                }
                break;
            }
        }

        //Sprinkle some now on top :)
        for (int i = 0; i < 7; i++)
        {
            if (Vertices[i].y > 10f)
                moveVert(i, -99, Color.white);
        }
    }

    public GameObject getNextPals(GameObject hex, int direction)//Returns true if you can keep going that way, and false if you can't
    {
        if (hex.GetComponent<HexInfo>().pals[direction] != null)
            return hex.GetComponent<HexInfo>().pals[direction];
        else
            return null;
    }

    public void blendCols(int vertNum)
    {
        int i1 = vertNum - 1;
        if (i1 == -1)
            i1 = 5;

        int i3 = vertNum + 2;
        if (i3 > 5)
            i3 -= 6;

        int i4 = vertNum + 4;
        if (i4 > 5)
            i4 -= 6;

        if (pals[vertNum] != null && pals[i1] != null)
        {
            var borderHex = pals[vertNum].GetComponent<HexInfo>();
            var borderHex2 = pals[i1].GetComponent<HexInfo>();


            var newCol = (borderHex.getColors()[i4] + borderHex2.getColors()[i3] + getColors()[vertNum]) / 3;
            moveVert(vertNum, -99, newCol);
            borderHex.moveVert(i4, -99, newCol);
            borderHex2.moveVert(i3, -99, newCol);
        }
        //Old code for centre vert
        //var newColy = (myCols[0] + myCols[1] + myCols[2] + myCols[3] + myCols[4] + myCols[5]) / 6;

    }

    public void interlopeCorner(int vertNum)
    {
        Vector3[] myVerts = getVerts();

        switch (vertNum)
        {
            case 0:
                #region 0
                if (pals[5] != null && pals[0] != null)
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
                else if (pals[5] != null)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(2, -0.5f, wetSand);
                }
                else if (pals[0] != null)
                {
                    var borderHex = pals[0].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(4, -0.5f, wetSand);
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
                if (pals[0] != null && pals[1] != null)
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
                else if (pals[1] != null)
                {
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(5, -0.5f, wetSand);
                }
                else if (pals[1] != null)
                {
                    var borderHex = pals[0].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(3, -0.5f, wetSand);
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
                if (pals[2] != null && pals[1] != null)
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
                else if (pals[1] != null)
                {
                    //This is a beach border
                    var borderHex = pals[1].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(4, -0.5f, wetSand);
                }
                else if (pals[2] != null)
                {
                    //This is a beach border
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(0, -0.5f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 3:
                #region 3
                if (pals[2] != null && pals[3] != null)
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
                else if (pals[2] != null)
                {
                    var borderHex = pals[2].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(5, -0.5f, wetSand);
                }
                else if (pals[3] != null)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(1, -0.5f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 4:
                #region 4
                if (pals[3] != null && pals[4] != null)
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
                else if (pals[3] != null)
                {
                    var borderHex = pals[3].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(0, -0.5f, wetSand);
                }
                else if (pals[4] != null)
                {
                    var borderHex = pals[4].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(2, -0.5f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 5:
                #region 5 
                if (pals[5] != null && pals[4] != null)
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
                else if (pals[4] != null)
                {
                    var borderHex = pals[4].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(1, -0.5f, wetSand);
                }
                else if (pals[5] != null)
                {
                    var borderHex = pals[5].GetComponent<HexInfo>();
                    moveVert(vertNum, -0.5f, wetSand);
                    borderHex.moveVert(3, -0.5f, wetSand);
                }
                else
                    moveVert(vertNum, -0.5f, wetSand);
                #endregion
                break;
            case 6:
                var newPointy = (myVerts[0].y + myVerts[1].y + myVerts[2].y + myVerts[3].y + myVerts[4].y + myVerts[5].y) / 6;
                newPointy = (newPointy + myVerts[6].y) / 2;
                moveVert(6, newPointy, Color.black);
                break;
        }
    }

    public void dirtPath(int start, float maxLeast, int length, IslandMaker caller)
    {
        Vector3[] myVerts = GetComponent<MeshFilter>().mesh.vertices;

        #region Colour starting dirt and centre point

        moveVert(start, myVerts[start].y, Color.Lerp(Color.black, dirt, 0.75f));

        int temp_ = start + 1;
        if (temp_ > 5)
            temp_ -= 6;
        int temp2_ = start + 2;
        if (temp2_ > 5)
            temp2_ -= 6;
        int temp5_ = start + 5;
        if (temp5_ > 5)
            temp5_ -= 6;

        pals[temp5_].GetComponent<HexInfo>().moveVert(temp2_, -99, Color.Lerp(Color.black, dirt, 0.75f));
        pals[temp_].GetComponent<HexInfo>().moveVert(temp5_, -99, Color.Lerp(Color.black, dirt, 0.75f));

        if (start != 5)
            moveVert(start + 1, myVerts[start + 1].y, Color.Lerp(Color.black, dirt, 0.75f));
        else
            moveVert(0, myVerts[0].y, Color.Lerp(Color.black, dirt, 0.75f));

        moveVert(6, myVerts[6].y, Color.Lerp(Color.black, dirt, 0.75f));

        #endregion
        //Now choose a random vert to go in the direction of
        //Must not be the starting point, the centre or either vertex to the sode of the starting point
        #region Choose a direction to go
        int randomDir = start, noGos = 0;
        while (randomDir == start || randomDir == start + 1 || randomDir == start - 1)
        {
            randomDir = (int)Random.Range(-0.49f, 5.49f);
            int temp = start + 2;
            if (temp > 5)
                temp -= 6;
            if (randomDir == temp)
                randomDir = start;

            //If it's too steep
            if (pals[randomDir].GetComponent<HexInfo>().Vertices[6].y > Vertices[6].y + 1)
            {
                noGos++;
                randomDir = start;
            }
            if (noGos == 3)
            {
                randomDir = 99;//You've been very naughty, time for you to get fucked up.
            }
        }
        #endregion

        if (randomDir != 99)
        {
            moveVert(randomDir, myVerts[randomDir].y, Color.Lerp(Color.black, dirt, 0.75f));
            if (randomDir != 5)
                moveVert(randomDir + 1, myVerts[randomDir + 1].y, Color.Lerp(Color.black, dirt, 0.75f));
            else
                moveVert(0, myVerts[0].y, Color.Lerp(Color.black, dirt, 0.75f));

            var nextVert = randomDir - 3;
            if (nextVert < 0)
                nextVert += 6;

            //If the nextvert Isn't like the one I was just checking for, keep going
            if ((pals[randomDir].GetComponent<HexInfo>().least / maxLeast) >= 0.25f)
            {
                length++;
                pals[randomDir].GetComponent<HexInfo>().dirtPath(nextVert, maxLeast, length, caller);
            }
            else
            {
                //The dirt path has ended
                for (int i = 0; i < 6; i++)
                {
                    moveVert(i, -99, Color.Lerp(Color.black, dirt, 0.75f));

                    int temp3_ = i + 3;
                    if (temp3_ > 5)
                        temp3_ -= 6;
                    int temp4_ = i + 4;
                    if (temp4_ > 5)
                        temp4_ -= 6;
                    pals[i].GetComponent<HexInfo>().moveVert(temp3_, -99, Color.Lerp(Color.black, dirt, 0.75f));
                    pals[i].GetComponent<HexInfo>().moveVert(temp4_, -99, Color.Lerp(Color.black, dirt, 0.75f));
                }
                //We did good! Let's celebrate with a camp!
                if (camp == null)
                    camp = Instantiate(Resources.Load("camp") as GameObject);

                //Move hole site into position
                var newPointy = (myVerts[0] + myVerts[1] + myVerts[2] + myVerts[3] + myVerts[4] + myVerts[5] + myVerts[6]) / 6;
                newPointy = (newPointy + myVerts[6]) / 2;
                camp.transform.position = newPointy + transform.position;
                camp.transform.Rotate(new Vector3(0, 150 + (60 * start), 0));

                var hut = camp.GetComponentInChildren<hut>().gameObject;
                var campfire = camp.GetComponentInChildren<campfire>().gameObject;

                int temp3 = start + 3;
                if (temp3 > 5)
                    temp3 -= 6;
                int temp4 = start + 4;
                if (temp4 > 5)
                    temp4 -= 6;

                newPointy = (myVerts[temp3] + myVerts[temp4]) / 2;
                newPointy = (newPointy * 3 + myVerts[6]) / 4;
                hut.transform.position = new Vector3(hut.transform.position.x, transform.position.y + newPointy.y, hut.transform.position.z);
                campfire.transform.position = transform.position + myVerts[6];
                
                campfire.transform.parent = hut.transform;
                caller.camps.Add(hut);

                moveVert(6, -99, dirt);
            }
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

        if (height != -99)
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

    void OnDestroy()
    {
        Destroy(camp);
    }
}