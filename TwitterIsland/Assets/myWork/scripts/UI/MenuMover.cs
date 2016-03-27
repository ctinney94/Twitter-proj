using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MenuMover : MonoBehaviour {

    public GameObject menuBit;
    public GameObject sizeGO, heightGO;
    string m_displayName, m_text;
    float m_size, m_height;
    float lerpSize, lerpHeight;
    bool lerp;

	void Start ()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    void Update()
    {
        if (menuBit.GetComponent<Image>().color.a > .4 && lerp)
        {
            lerpHeight = Mathf.Lerp(lerpHeight, m_height, Time.deltaTime*2);
            lerpSize = Mathf.Lerp(lerpSize, m_size, Time.deltaTime*2);

            heightGO.GetComponentInChildren<Text>().text = "" + Mathf.FloorToInt(lerpHeight);
            heightGO.GetComponent<Slider>().value = lerpHeight - Mathf.FloorToInt(lerpHeight);
            sizeGO.GetComponentInChildren<Text>().text = "" + Mathf.FloorToInt(lerpSize);
            sizeGO.GetComponent<Slider>().value = lerpSize - Mathf.FloorToInt(lerpSize);
        }
        else if (menuBit.GetComponent<Image>().color.a < .05f)
        {
            heightGO.GetComponentInChildren<Text>().text = "0";
            heightGO.GetComponent<Slider>().value = 0;
            sizeGO.GetComponentInChildren<Text>().text = "0";
            sizeGO.GetComponent<Slider>().value = 0;
        }
    }

    public void updateUI(Twitter.API.Tweet newTweet, float sentiment, float height, float size)
    {
        lerpSize = 0;
        lerpHeight = 0;
        lerp = false;
        m_height = height;
        m_size = size;

        StopCoroutine(revert(newTweet,sentiment));
        Text uiText = menuBit.GetComponentInChildren<Text>();
        uiText.DOFade(0, .75f);
        uiText.text = m_displayName+ " tweeted;\n" + "<size=16>" + m_text + "</size>";
        uiText.text += "\n\nSentiment: ";
        menuBit.GetComponent<Image>().DOFade(0, .75f);
        menuBit.GetComponentsInChildren<Text>()[0].DOFade(0, .75f);
        menuBit.GetComponentsInChildren<Text>()[2].DOFade(0, .75f);
        StartCoroutine(revert(newTweet, sentiment));

        Image[] images = sizeGO.GetComponentsInChildren<Image>();
        foreach(Image i in images)
        {
            i.DOFade(0, .75f);
        }
        images = heightGO.GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            i.DOFade(0, .75f);
        }

        heightGO.GetComponentInChildren<Text>().DOFade(0, .75f);
        sizeGO.GetComponentInChildren<Text>().DOFade(0, .75f);
    }

    IEnumerator revert(Twitter.API.Tweet newTweet, float sentiment)
    {
        yield return new WaitForSeconds(1f);
        lerp = true;
        Color newCol = Color.white;
        if (sentiment != 0 && sentiment > 0)
            newCol = Color.Lerp(Color.white, Color.green, sentiment);
        else if (sentiment != 0)
            newCol = Color.Lerp(Color.white, Color.red, Mathf.Abs(sentiment));

        var rgbString = string.Format("#{0:X2}{1:X2}{2:X2}",
            (int)(newCol.r * 255),
            (int)(newCol.g * 255),
            (int)(newCol.b * 255));

        string sent = "<color=" + rgbString + ">";
        if (Mathf.Abs(sentiment) > .9f)
        {
            sent += "Overwhelmingly ";
        }
        else if (Mathf.Abs(sentiment) > .65f)
        {
            sent += "Mostly ";
        }
        else if (Mathf.Abs(sentiment) > .3f)
        {
            sent += "Somewhat ";
        }
        else if (Mathf.Abs(sentiment) > .1f)
        {
            sent += "Mildly ";
        }

        if (sentiment > 0)
            sent += "positive</color>";
        else if (sentiment == 0)
            sent += "Neutral</color>";
        else
            sent += "negative</color>";

        Text uiText = menuBit.GetComponentInChildren<Text>();
        m_displayName = newTweet.DisplayName;
        m_text = newTweet.Text;
        uiText.text = newTweet.DisplayName + " tweeted;\n" + "<size=16>" + newTweet.Text + "</size>";
        uiText.text += "\n\nSentiment: ";
        menuBit.GetComponentsInChildren<Text>()[0].DOFade(1, .75f);
        menuBit.GetComponentsInChildren<Text>()[2].DOFade(1, .75f);
        menuBit.GetComponent<Image>().DOFade(.5f, .75f);

        Image[] images = sizeGO.GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            i.DOFade(1, .75f);
        }
        images = heightGO.GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            i.DOFade(1, .75f);
        }

        heightGO.GetComponentInChildren<Text>().DOFade(1, .75f);
        sizeGO.GetComponentInChildren<Text>().DOFade(1, .75f);

        yield return new WaitForSeconds(1.25f);
        uiText.text = newTweet.DisplayName + " tweeted;\n" + "<size=16>" + newTweet.Text + "</size>";
        uiText.text += "\n\nSentiment: " + "<size=16>" +sent+ "</size>";

    }
}
