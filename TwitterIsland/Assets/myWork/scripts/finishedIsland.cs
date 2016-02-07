using UnityEngine;
using System.Collections;

public class finishedIsland : MonoBehaviour
{

    Color grass = Color.Lerp(Color.green, Color.black, 0.5f);
    Color sand = Color.Lerp(Color.yellow, Color.white, 0.2f);
    Color rock = Color.gray;
    Color dirt = new Color(0.96f, 0.64f, 0.38f);

    public int islandIndex;

    void OnMouseDown()
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<cameraOrbitControls>().newTarget = transform.position;
            Camera.main.GetComponent<cameraOrbitControls>().currentIsland = islandIndex;
        }
        //I tried adding the poofs back in as a thing.
        //Turns out looking through every single vertex of a big island is kinda time consuming, and slows the program right down.
        
        /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Color centreColour = GetComponent<MeshFilter>().mesh.colors[NearestVertexTo(hit.point)];

            if (centreColour == Color.Lerp(Color.black, grass, 0.8f) || centreColour == grass)
            {
                GameObject poof = Instantiate(Resources.Load("forestPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == sand || centreColour == Color.Lerp(Color.white, sand, 0.75f))
            {
                GameObject poof = Instantiate(Resources.Load("sandPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == rock || centreColour == Color.Lerp(Color.black, dirt, 0.5f))
            {
                GameObject poof = Instantiate(Resources.Load("rockPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else if (centreColour == Color.white)
            {
                GameObject poof = Instantiate(Resources.Load("snowPoof") as GameObject);
                poof.transform.position = hit.point;
            }
            else
            {
                GameObject poof = Instantiate(Resources.Load("rockPoof") as GameObject);
                poof.transform.position = hit.point;
            }
        }*/
    }

    public int NearestVertexTo(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);
        
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        float minDistanceSqr = Mathf.Infinity;
        int nearestVertex=0;
        // scan all vertices to find nearest
        /*for (int i=0; i < mesh.vertexCount;i++)
        {
            //Find difference between 2 points
            Vector3 diff = point - mesh.vertices[i];

            //Find this difference in vectors as a singular float value
            float distSqr = diff.sqrMagnitude;

            //If this the lowest distance EVAR
            if (distSqr < minDistanceSqr)
            {
                //New lowest distance is current
                minDistanceSqr = distSqr;
                nearestVertex = i;

                if (minDistanceSqr < .5f)
                {
                    i = mesh.vertexCount - 1;
                }
            }
        }*/

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            float distance = Vector3.Distance(point, mesh.vertices[i]);
            if (distance < .5f)
            {
                nearestVertex = i;
                i = mesh.vertexCount - 1;
            }
        }
        return nearestVertex;
    }
}
