using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MenuMover : MonoBehaviour {

    public GameObject menuBit;
    public GameObject sizeGO, heightGO;
    string m_displayName, m_text,timeAgo;
    float m_size, m_height;
    float lerpSize, lerpHeight;
    bool lerp;

	void Start ()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    void Update()
    {
        #region Slider lerping
        if (menuBit.activeSelf)
        {
            if (menuBit.GetComponent<Image>().color.a > .4 && lerp)
            {
                lerpHeight = Mathf.Lerp(lerpHeight, m_height, Time.deltaTime * 2);
                lerpSize = Mathf.Lerp(lerpSize, m_size, Time.deltaTime * 2);

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
        #endregion
    }

    public void updateUI(Twitter.API.Tweet newTweet, float sentiment, float height, float size)
    {
        StopCoroutine(revert(newTweet,sentiment));

        menuBit.SetActive(true);
        menuBit.GetComponentInChildren<Text>().text = m_displayName + " tweeted;\n" + "<size=20>" + m_text + "</size>"+
            "<size=24>\n\n<i>" + timeAgo + " </i>ago</size>"+
            "\n\nSentiment: " + "<size=16>       </size>";

        lerpSize = 0;
        lerpHeight = 0;
        lerp = false;
        m_height = height;
        m_size = size;
        m_displayName = newTweet.DisplayName;
        m_text = newTweet.Text.Replace("\\n", '\n' + "");

        //Calculate time between tweet post and now
        #region time between now and post
        int MonthInt=0;
        switch (newTweet.dateTime.Month)
        {
            case "Jan":
                MonthInt = 1;
                break;
            case "Feb":
                MonthInt = 2;
                break;
            case "Mar":
                MonthInt = 3;
                break;
            case "Apr":
                MonthInt = 4;
                break;
            case "May":
                MonthInt = 5;
                break;
            case "Jun":
                MonthInt = 6;
                break;
            case "Jul":
                MonthInt = 7;
                break;
            case "Aug":
                MonthInt = 8;
                break;
            case "Sep":
                MonthInt = 9;
                break;
            case "Oct":
                MonthInt = 10;
                break;
            case "Nov":
                MonthInt = 11;
                break;
            case "Dec":
                MonthInt = 12;
                break;
        }

        float difference = (((System.DateTime.Now.Year - 1) * 365)
            + ((System.DateTime.Now.Month - 1) * 30)
            + System.DateTime.Now.Day)
            -
            (((newTweet.dateTime.Year - 1) * 365)
            + ((MonthInt - 1) * 30)
            + newTweet.dateTime.Day);

        if (difference == 0)
        {
            //Posted today!
            float hours = System.DateTime.Now.Hour - newTweet.dateTime.Hour;
            if (hours >= 22)
                timeAgo = "almost a day";
            else if (hours >= 5)
                timeAgo = hours+" hours";
            else if (hours >= 3)
                timeAgo = "a few hours";
            else if (hours >= 2)
                timeAgo = "a couple hours";
            else if (hours >= 1)
                timeAgo = "about an hour";
            else
            {
                float mins = ((System.DateTime.Now.Hour * 60) + System.DateTime.Now.Minute)
                    -
                    (newTweet.dateTime.Hour * 60 + newTweet.dateTime.Minute);
                if (mins > 54)
                    timeAgo = "about an hour";
                else if (mins > 5)
                    timeAgo = mins+" minutes";
                else if (mins >= 3)
                    timeAgo = "a few minutes";
                else if (mins >= 2)
                    timeAgo = "a couple minutes";
                else if (mins >= 1)
                    timeAgo = "about a minute";
                else
                    timeAgo = "less than a minute";

            }
        }
        else
        {
            if (difference == 1)
            {
                //This must be yesturday
                //Hours between now, and when the tweet was posted
                float h = System.DateTime.Now.Hour + (24 - newTweet.dateTime.Hour);
                if (h > 24)
                    timeAgo = "about a day";
                else
                {
                    //How many hours ago?
                    if (h >1)
                    timeAgo = h + " hours";
                    else
                    {
                        float minutes = System.DateTime.Now.Minute - newTweet.dateTime.Minute;
                    }
                }
            }
            else
            {
                //how many YEARS
                if (difference / 365 > 5)
                    timeAgo = (int)(difference / 365 )+" years";
                else if (difference / 365 >= 3)
                    timeAgo = "a few years";
                else if (difference / 365 >= 2)
                    timeAgo = "a couple years";
                else if (difference / 365 > 1)
                    timeAgo = "over a year";
                else
                {
                    //it's month time baby
                    if (difference / 30 > 11)
                        timeAgo = "almost a year";
                    else if (difference / 30 > 5)
                        timeAgo = (int)(difference / 30) + " months";
                    else if (difference / 30 >= 3)
                        timeAgo = "a few months";
                    else if (difference / 30 >= 2)
                        timeAgo = "a couple months";
                    else if (difference / 30 > 1)
                        timeAgo = "over a month";
                    else
                    {
                        //WE NEED TO GO DEEPER
                        //WEEKS
                        if (difference / 7 >= 3)
                            timeAgo = "a few weeks";
                        else if (difference / 7 >= 2)
                            timeAgo = "a couple weeks";
                        else if (difference / 7 > 1)
                            timeAgo = "over a week";
                        else
                            timeAgo = difference + " days";
                    }
                }
            }
        }
        #endregion

        #region  Fade out everything
        menuBit.GetComponentInChildren<Text>().DOFade(0, .75f);
        menuBit.GetComponent<Image>().DOFade(0, .75f);
        menuBit.GetComponentsInChildren<Text>()[0].DOFade(0, .75f);
        menuBit.GetComponentsInChildren<Text>()[2].DOFade(0, .75f);

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
        #endregion

        StartCoroutine(revert(newTweet, sentiment));
    }

    IEnumerator revert(Twitter.API.Tweet newTweet, float sentiment)
    {
        yield return new WaitForSeconds(1f);
        lerp = true;

        #region Sentiment text creation
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
        #endregion

        Text uiText = menuBit.GetComponentInChildren<Text>();
        uiText.text = m_displayName + " tweeted;\n" + "<size=20>" + m_text + "</size>" +
            "<size=24>\n\n<i>" + timeAgo + " </i>ago</size>" +
            "\n\nSentiment: " + "<size=16>       </size>";

        #region  Fade in the everything
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
        #endregion

        yield return new WaitForSeconds(1.25f);
        uiText.text = m_displayName + " tweeted;\n" + "<size=20>" + m_text + "</size>" +
            "<size=24>\n\n<i>" + timeAgo + " </i>ago</size>" +
            "\n\nSentiment: " + "<size=16>" + sent + "</size>";

    }
}
