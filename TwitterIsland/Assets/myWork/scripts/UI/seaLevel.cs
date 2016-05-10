using UnityEngine;
using UnityEngine.UI;

//Used to raise of lower the position of the sea
public class seaLevel : MonoBehaviour {
    public Slider theSlider;
	public void changeSeaLevel(float newHeight)
    {
        newHeight += -0.475f;
        Vector3 pos = transform.position;
        pos.y = newHeight;
        transform.position = pos;
    }
    public void Reset()
    {
        theSlider.value = 0;
        transform.position = Vector3.zero;
    }
}
