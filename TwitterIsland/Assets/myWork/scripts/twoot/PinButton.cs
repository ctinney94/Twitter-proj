using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PinButton : MonoBehaviour
{
    string consumerKey, consumerSecret;
    loginButton details;

    void Start()
    {
        details = GameObject.Find("Login Button").GetComponent<loginButton>();

        consumerKey = details.consumerKey;
        consumerSecret = details.consumerSecret;
        
    }

    public void enterPin()
    {
        string m_PIN = GameObject.Find("PIN Input").GetComponent<InputField>().text;
        StartCoroutine(Twitter.API.GetAccessToken(consumerKey, consumerSecret, details.m_RequestTokenResponse.Token, m_PIN,
                       new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
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

            details.m_AccessTokenResponse = response;

            details.userID = response.UserId;
            details.ScreenName = response.ScreenName;
            details.Token = response.Token;
            details.TokenSecret = response.TokenSecret;
            details.UpdateDetails();
        }
        else
        {
            print("OnAccessTokenCallback - failed.");
        }
    }
}
