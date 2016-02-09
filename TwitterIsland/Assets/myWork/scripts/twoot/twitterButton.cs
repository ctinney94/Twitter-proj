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

    loginButton details;
    public int count;
    public List<Twitter.API.Tweet> tweets;
    public InputField usernameInput;
    public IslandMaker IslandMaker;

    // Use this for initialization
    void Start()
    {
        details = GameObject.Find("Login Button").GetComponent<loginButton>();
    }

    public void postTweet()
    {
        string m_Tweet = GameObject.Find("Tweet Input").GetComponent<InputField>().text;

        StartCoroutine(Twitter.API.PostTweet(m_Tweet, details.consumerKey, details.consumerSecret, details.m_AccessTokenResponse,
                           new Twitter.PostTweetCallback(this.OnPostTweet)));
    }

    void OnPostTweet(bool success)
    {
        print("OnPostTweet - " + (success ? "succedded." : "failed."));
    }

    public void GetTweets()
    {
        if (usernameInput.text != null)
        {
            Twitter.API.GetUserTimeline(usernameInput.text, Twitter.API.GetTwitterAccessToken(details.consumerKey, details.consumerSecret), count, this);
            Twitter.API.GetProfile(usernameInput.text, Twitter.API.GetTwitterAccessToken(details.consumerKey, details.consumerSecret), this);
        }
        else
        {
            Twitter.API.GetUserTimeline(details.ScreenName, Twitter.API.GetTwitterAccessToken(details.consumerKey, details.consumerSecret), count, this);
            Twitter.API.GetProfile(details.ScreenName, Twitter.API.GetTwitterAccessToken(details.consumerKey, details.consumerSecret), this);
        }

        //Once we have the tweets, do islands!
        //for each tweet
        StartCoroutine(makeIsland());
    }

    IEnumerator makeIsland()
    {
        for (int i = 0; i < tweets.Count; i++)
        {
            numbersToSliders nums = GameObject.Find("numbersToSliders").GetComponent<numbersToSliders>();         
            IslandMaker.favs = nums.favs(tweets[i].Favs);
            IslandMaker.meshScale = nums.RTs(tweets[i].RTs);
            IslandMaker.UpdatePentMesh(null);

            IslandMaker.avatar = avatarImage;
            IslandMaker.updateHexs(tweets[i].Text);
            yield return new WaitForSeconds(.75f);

            GameObject gulls = Instantiate(Resources.Load("Gulley!")) as GameObject;
            gulls.GetComponent<gullMaker>().reloadGulls(tweets[i].Text);

            yield return new WaitForSeconds(0.5f);
            IslandMaker.mergeIsland(gulls,tweets[i]);
        }
    }
    static Texture2D avatarImage;
    public static IEnumerator setAvatar(string url)
    {
        WWW web = new WWW(url);
        yield return web;
        avatarImage = web.texture;
        IslandMaker.avatar = web.texture;
        //GameObject.Find("Avatar").GetComponent<Image>().sprite = Sprite.Create(web.texture,new Rect(0,0,web.texture.width,web.texture.height), new Vector2(.5f,.5f));
    }

    public class TwitAuthenticateResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }
}
