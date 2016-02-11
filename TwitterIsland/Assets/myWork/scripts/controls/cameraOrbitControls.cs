
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//SOURCE:
//http://wiki.unity3d.com/index.php?title=MouseOrbitZoom

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class cameraOrbitControls : MonoBehaviour
{
    public List<GameObject> islands = new List<GameObject>();
    public int currentIsland = 0;

    public Transform target;
    public Vector3 targetOffset;
    public float distance = 0;
    float maxDistance = 150;
    float minDistance = 0.6f;
    float xSpeed = 120f;
    float ySpeed = 120;
    int yMinLimit = 5;
    int yMaxLimit = 80;
    int zoomRate = 100;
    float panSpeed = 0.5f;
    float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    void Start() { Init(); }
    void OnEnable() { Init(); }
    
    public Vector3 newTarget;

    public void changeTarget(int direction)
    {
        if (direction > 0)
        {
            if (currentIsland != islands.Count)
            {
                newTarget = islands[currentIsland].transform.position;
                currentIsland += direction;
            }
        }
        else
        {
            if (currentIsland >1)
            {
                currentIsland += direction;
                newTarget = islands[currentIsland-1].transform.position;
            }
            else
            {
                currentIsland = 0;
                newTarget = Vector3.zero;
            }
        }
        if (islands.Count > 0 && currentIsland > 0)
        {
            GameObject.Find("WorldLight").GetComponent<lighting>().newShadowStrength = islands[currentIsland - 1].GetComponent<finishedIsland>().blackness;
        }
    }

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position;
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance + 10;

        //be sure to grab the current rotations as starting points.
        position = transform.position - (transform.forward * distance); ;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        //Lerp target
        target.position = Vector3.Lerp(target.position, newTarget, Time.deltaTime*5);
        if (target.position == newTarget)
            target.position = newTarget;

        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(1))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.25f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if (Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation f
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(1))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.F))
            target.transform.position = Vector3.zero;

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        //Do the same for rotating
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;

        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;

        /*if (Input.GetKey(KeyCode.W))
        {
            target.Translate(Vector3.up * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            target.Translate(Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            target.Translate(Vector3.down * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            target.Translate(Vector3.right * Time.deltaTime);
        }*/
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

/*using UnityEngine;
using System.Collections;

//SOURCE:
//http://wiki.unity3d.com/index.php?title=MouseOrbitImproved#Code_C.23

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class cameraOrbitControls : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody rigidbody;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance -Mathf.Pow(Input.GetAxis("Mouse ScrollWheel")*10,3), distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}*/
