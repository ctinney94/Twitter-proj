using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

//This script is intended to be attachewd to a canvas object
public class HudFader : MonoBehaviour
{
    public GameObject canvas;
    Image[] images;
    Text[] texts;
    Button[] buttons;

    //Collect references for each UI object parented to the canvas
    void Start()
    {
        images = canvas.GetComponentsInChildren<Image>();
        texts = canvas.GetComponentsInChildren<Text>();
        buttons = canvas.GetComponentsInChildren<Button>();
    }
    
    //Toggle the interactivity of all buttons
    public void buttonControl(bool b)
    {
        foreach(Button butt in buttons)
        {
            butt.interactable = b;
        }
    }

    //Fade UI objects in and out, based on input bool
    public void fade(bool b)
    {
        foreach (Image i in images)
        {
            i.DOFade(Convert.ToInt32(b), .75f);
        }
        foreach (Text t in texts)
        {
            t.DOFade(Convert.ToInt32(b), .75f);
            if (t.GetComponent<CircleOutline>())
            {
                if (b)
                    t.GetComponent<CircleOutline>().effectDistance = Vector2.one;
                else
                    t.GetComponent<CircleOutline>().effectDistance = Vector2.zero;
            }
        }
    }
}
