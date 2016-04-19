using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Net;
using System.Security.Cryptography;

public class twitterButton : MonoBehaviour {

    public string consumerKey, consumerSecret;
    public int tweetsToGrab;
    public List<Twitter.API.Tweet> tweets;
    public InputField usernameInput, PINInput, textInput;
    public IslandMaker IslandMaker;
    public GameObject gulley;
    public GameObject tweetCountText,islandBuildingText;
    public bool verified;
        
    bool running;
    
    //New API related things required for screenshot posting
    const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";
    const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";
    public Twitter.RequestTokenResponse m_RequestTokenResponse;
    Twitter.AccessTokenResponse m_AccessTokenResponse;

    void Start()
    {
        LoadTwitterUserInfo();
    }

    public void registerUser()
    {
        Debug.Log("Register button");
        //Open a URL with the users PIN
        StartCoroutine(Twitter.API.GetRequestToken(consumerKey, consumerSecret, this));
    }

    public void SubitPIN()
    {
        StartCoroutine(Twitter.API.GetAccessToken(consumerKey, consumerSecret, m_RequestTokenResponse.Token, PINInput.text, this.OnAccessTokenCallback));
    }

    public void PostScreenshotToTwitter(string encodedImage)
    {
        StartCoroutine(Twitter.API.PostScreenshot(encodedImage, consumerKey, consumerSecret, m_AccessTokenResponse,this));
    }

    public void postMe(string mediaID)
    {
        StartCoroutine(Twitter.API.PostTweet("HELLO", mediaID, consumerKey, consumerSecret, m_AccessTokenResponse));
    }


    #region Everything related to islanding

    //Grab some tweets (and profile info)
    public void GetTweets()
    {
        //Don't let the user press any more buttons, we got enough on our plate as is.
        GetComponentInParent<HudFader>().buttonControl(false);

        //Make sure we can't run things twice.
        if (!running)
        {
            running = true;
            //SWIGGIDY SWOOTY, IM COMING FOR THAT USER PROFILE
            Twitter.API.GetProfile(usernameInput.text, Twitter.API.GetTwitterAccessToken(consumerKey, consumerSecret), this);

            //Grab some tweets too
            Twitter.API.GetUserTimeline(usernameInput.text, Twitter.API.GetTwitterAccessToken(consumerKey, consumerSecret), tweetsToGrab, this);

            //Once we have the tweets, Make some islands!
            StartCoroutine(makeIsland());
        }
        else
            Debug.Log("Island creation process already running - please wait"); //Chill fam, fuck.
    }

    //Make some islands
    IEnumerator makeIsland()
    {
        //Tell the user we're building islands
        islandBuildingText.SetActive(true);

        //And about the number of tweets retrieved from the API call
        GameObject.Find("retrieval stats").GetComponent<Text>().enabled = true;
        GameObject.Find("retrieval stats").GetComponent<Text>().text = (tweets.Count + " tweets retrieved successfully");

        //For each tweet
        for (int i = 0; i < tweets.Count; i++)
        {
            //Update UI text
            islandBuildingText.GetComponent<Text>().text = "Building Islands..." + "\n" + (i + 1) + "/" + tweets.Count;

            //Find size + height rank
            //Also set up variables/references
            numbersToSliders nums = GameObject.Find("numbersToSliders").GetComponent<numbersToSliders>();
            IslandMaker.favs = nums.favs(tweets[i].Favs);
            IslandMaker.meshScale = nums.RTs(tweets[i].RTs);
            IslandMaker.verified = verified;
            IslandMaker.avatar = avatarImage;

            //Begin the island creation process, consult IslandMaker.cs for details
            IslandMaker.updateHexs(tweets[i].Text);
            
            //Give it a moment to make the island
            yield return new WaitForSeconds(1f);

            //Add the seagulls in
            GameObject gulls = Instantiate(gulley) as GameObject;
            gulls.GetComponent<gullMaker>().reloadGulls(tweets[i].Text);

            yield return new WaitForSeconds(.75f);

            //Take the created island, and put it somewhere else, merging the appropriate elements
            IslandMaker.mergeIsland(gulls, tweets[i]);
            yield return new WaitForSeconds(.25f);
        }

        //Now we've made all the islands, get rid of the tweet data used
        tweets.Clear();
        //Turn buttons back on
        GetComponentInParent<HudFader>().buttonControl(true);
        islandBuildingText.SetActive(false);
        running = false;

        //We done here
    }

    //Take avatar URL, find image, turn it into a texture to use as a flag later on
    static Texture2D avatarImage;
    public static IEnumerator setAvatar(string url)
    {
        url = url.Replace("\\", "");
        WWW web = new WWW(url);
        yield return web;
        avatarImage = web.texture;
        IslandMaker.avatar = web.texture;
        //GameObject.Find("Avatar").GetComponent<Image>().sprite = Sprite.Create(web.texture,new Rect(0,0,web.texture.width,web.texture.height), new Vector2(.5f,.5f));
    }

    //UI function
    public void changeTweetCount(float newCount)
    {
        tweetsToGrab = (int)(1 + Mathf.Pow((Mathf.Pow(199f, 1f / 3f) * newCount), 3));
        string temp = "Tweets to retrieve: " + tweetsToGrab;
        tweetCountText.GetComponent<Text>().text = temp;
    }
    #endregion

    void LoadTwitterUserInfo()
    {
        m_AccessTokenResponse = new Twitter.AccessTokenResponse();

        m_AccessTokenResponse.UserId = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
        m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        m_AccessTokenResponse.Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
        m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

        if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
        {
            string log = "LoadTwitterUserInfo - succeeded";
            log += "\n    UserId : " + m_AccessTokenResponse.UserId;
            log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
            log += "\n    Token : " + m_AccessTokenResponse.Token;
            log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
            print(log);
        }
    }

    void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
    {
        if (success)
        {
            string log = "OnAccessTokenCallback - succeeded";
            log += "\n    UserId : " + response.UserId;
            log += "\n    ScreenName : " + response.ScreenName;
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            print(log);

            m_AccessTokenResponse = response;

            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
        }
        else
        {
            print("OnAccessTokenCallback - failed.");
        }
    }
}
