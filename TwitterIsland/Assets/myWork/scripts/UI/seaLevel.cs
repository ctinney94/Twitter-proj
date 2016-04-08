using UnityEngine;
using System.Collections;

//Used to raise of lower the position of the sea
public class seaLevel : MonoBehaviour {

	public void changeSeaLevel(float newHeight)
    {
        newHeight += -0.475f;
        Vector3 pos = transform.position;
        pos.y = newHeight;
        transform.position = pos;
    }
}
