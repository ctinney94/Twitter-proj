using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {

    public int dir;
    void OnMouseDown()
    {
        Camera.main.GetComponent<cameraOrbitControls>().changeTarget(dir);
        clicked = true;
    }
    bool clicked;
    void Update()
    {
        /*if (clicked&& transform.localScale.x < 1.248f)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.25f, Time.deltaTime*5);
        else
        {
            clicked = false;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime*5);
        }*/
    }
}
