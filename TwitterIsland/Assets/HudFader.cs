using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class HudFader : MonoBehaviour
{
    public GameObject canvas;
    Image[] images;
    Text[] texts;
    Button[] buttons;

    void Start()
    {
        images = canvas.GetComponentsInChildren<Image>();
        texts = canvas.GetComponentsInChildren<Text>();
        buttons = canvas.GetComponentsInChildren<Button>();
    }
    
    public void buttonControl(bool b)
    {
        foreach(Button butt in buttons)
        {
            butt.interactable = b;
        }
    }

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
