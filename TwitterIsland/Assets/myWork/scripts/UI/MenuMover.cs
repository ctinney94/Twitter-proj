using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MenuMover : MonoBehaviour {

    public GameObject menuBit;
    string displayName, text;
	// Use this for initialization
	void Start () {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    public void updateUI(Twitter.API.Tweet newTweet, float sentiment)
    {
        StopCoroutine(revert(newTweet,sentiment));
        Text uiText = menuBit.GetComponentInChildren<Text>();
        uiText.DOFade(0, .75f);
        uiText.text = displayName+ " tweeted;\n" + "<size=16>" + text + "</size>";
        uiText.text += "\n\nSentiment: ";
        uiText.text += "\n\nSize rank: " + "<size=18>" + 15 + "</size>";
        uiText.text += "\n\nHeight rank: " + "<size=18>" + 23 + "</size>";
        menuBit.GetComponent<Image>().DOFade(0, .75f);
        StartCoroutine(revert(newTweet, sentiment));
    }

    IEnumerator revert(Twitter.API.Tweet newTweet, float sentiment)
    {
        yield return new WaitForSeconds(1f);

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
        displayName = newTweet.DisplayName;
        text = newTweet.Text;
        uiText.text = newTweet.DisplayName + " tweeted;\n" + "<size=16>" + newTweet.Text + "</size>";
        uiText.text += "\n\nSentiment: ";
        uiText.text += "\n\nSize rank: " + "<size=18>" + 15 + "</size>";
        uiText.text += "\n\nHeight rank: " + "<size=18>" + 23 + "</size>";
        menuBit.GetComponentInChildren<Text>().DOFade(1, .75f);
        menuBit.GetComponent<Image>().DOFade(.5f, .75f);

        yield return new WaitForSeconds(1.25f);
        uiText.text = newTweet.DisplayName + " tweeted;\n" + "<size=16>" + newTweet.Text + "</size>";
        uiText.text += "\n\nSentiment: " + "<size=16>" +sent+ "</size>";
        uiText.text += "\n\nSize rank: " + "<size=18>" + 15 + "</size>";
        uiText.text += "\n\nHeight rank: " + "<size=18>" + 23 + "</size>";

    }
}
