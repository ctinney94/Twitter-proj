using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class loginButton : MonoBehaviour {

    public string consumerKey, consumerSecret;
    public string userID, ScreenName, Token, TokenSecret;
    int casey;
    public Twitter.AccessTokenResponse m_AccessTokenResponse;
    public Twitter.RequestTokenResponse m_RequestTokenResponse;


    const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";
    const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";

    // Use this for initialization
    void Start () {
        LoadTwitterUserInfo();
        UpdateText();
	}

    void LoadTwitterUserInfo()
    {
        m_AccessTokenResponse = new Twitter.AccessTokenResponse();

        m_AccessTokenResponse.UserId = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
        m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        m_AccessTokenResponse.Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
        m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

        userID = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
        ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
        TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);


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
            Debug.Log(log);
        }
    }

    public void UpdateDetails()
    {
        PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, m_AccessTokenResponse.UserId);
        PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, m_AccessTokenResponse.ScreenName);
        PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, m_AccessTokenResponse.Token);
        PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, m_AccessTokenResponse.TokenSecret);
    }

    // Update is called once per frame
    public void UpdateText()
    {
        if (string.IsNullOrEmpty(consumerKey) ||string.IsNullOrEmpty(consumerSecret))
        {
            GetComponentInChildren<Text>().text = "You need to register your game or application first.\n Click this button, register and fill CONSUMER_KEY and CONSUMER_SECRET of Demo game object.";
            casey = 1;
        }
        else
        {
            if (!string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName))
            {
                GetComponentInChildren<Text>().text = "Logged In: "+m_AccessTokenResponse.ScreenName + "\nClick to register with a different account";
            }

            else
            {
                GetComponentInChildren<Text>().text = "You need to register your game or application first.";
            }
            casey = 0;
        }
    }

    public void doThings()
    {
        switch (casey)
        {
            case 1:
                Application.OpenURL("http://dev.twitter.com/apps/new");
                break;
            case 0:
                StartCoroutine(Twitter.API.GetRequestToken(consumerKey, consumerSecret,
                    new Twitter.RequestTokenCallback(this.OnRequestTokenCallback)));
                break;
            default:
                break;
        }
    }


    void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response)
    {
        if (success)
        {
            string log = "OnRequestTokenCallback - succeeded";
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            print(log);

            m_RequestTokenResponse = response;

            Twitter.API.OpenAuthorizationPage(response.Token);
        }
        else
        {
            print("OnRequestTokenCallback - failed.");
        }
    }
}
