using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace Twitter
{

    public class RequestTokenResponse
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }

    public class AccessTokenResponse
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string UserId { get; set; }
        public string ScreenName { get; set; }
    }

    public delegate void RequestTokenCallback(bool success, RequestTokenResponse response);
    public delegate void AccessTokenCallback(bool success, AccessTokenResponse response);
    public delegate void PostTweetCallback(bool success);
    public delegate void GetAvatarCallback(bool success);

    public class API
    {
        #region OAuth Token Methods
        // 1. Get Request-Token From Twitter
        // 2. Get PIN from User
        // 3. Get Access-Token from Twitter
        // 4. Use Accss-Token for APIs requriring OAuth 
        // Accss-Token will be always valid until the user revokes the access to your application.

        // Twitter APIs for OAuth process
        private static readonly string RequestTokenURL = "https://api.twitter.com/oauth/request_token";
        private static readonly string AuthorizationURL = "https://api.twitter.com/oauth/authenticate?oauth_token={0}";
        private static readonly string AccessTokenURL = "https://api.twitter.com/oauth/access_token";

        public static IEnumerator GetRequestToken(string consumerKey, string consumerSecret, RequestTokenCallback callback)
        {
            
            WWW web = WWWRequestToken(consumerKey, consumerSecret);

            yield return web;

            if (!string.IsNullOrEmpty(web.error))
            {
                Debug.Log(string.Format("GetRequestToken - failed. error : {0}", web.error));
                callback(false, null);
            }
            else
            {
                RequestTokenResponse response = new RequestTokenResponse
                {
                    Token = Regex.Match(web.text, @"oauth_token=([^&]+)").Groups[1].Value,
                    TokenSecret = Regex.Match(web.text, @"oauth_token_secret=([^&]+)").Groups[1].Value,
                };

                if (!string.IsNullOrEmpty(response.Token) &&
                    !string.IsNullOrEmpty(response.TokenSecret))
                {
                    callback(true, response);
                }
                else
                {
                    Debug.Log(string.Format("GetRequestToken - failed. response : {0}", web.text));

                    callback(false, null);
                }
            }
        }

        public static void OpenAuthorizationPage(string requestToken)
        {
            Application.OpenURL(string.Format(AuthorizationURL, requestToken));
        }

        public static IEnumerator GetAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin, AccessTokenCallback callback)
        {
            WWW web = WWWAccessToken(consumerKey, consumerSecret, requestToken, pin);

            yield return web;

            if (!string.IsNullOrEmpty(web.error))
            {
                Debug.Log(string.Format("GetAccessToken - failed. error : {0}", web.error));
                callback(false, null);
            }
            else
            {
                AccessTokenResponse response = new AccessTokenResponse
                {
                    Token = Regex.Match(web.text, @"oauth_token=([^&]+)").Groups[1].Value,
                    TokenSecret = Regex.Match(web.text, @"oauth_token_secret=([^&]+)").Groups[1].Value,
                    UserId = Regex.Match(web.text, @"user_id=([^&]+)").Groups[1].Value,
                    ScreenName = Regex.Match(web.text, @"screen_name=([^&]+)").Groups[1].Value
                };

                if (!string.IsNullOrEmpty(response.Token) &&
                    !string.IsNullOrEmpty(response.TokenSecret) &&
                    !string.IsNullOrEmpty(response.UserId) &&
                    !string.IsNullOrEmpty(response.ScreenName))
                {
                    callback(true, response);
                }
                else
                {
                    Debug.Log(string.Format("GetAccessToken - failed. response : {0}", web.text));

                    callback(false, null);
                }
            }
        }

        private static WWW WWWRequestToken(string consumerKey, string consumerSecret)
        {
            // Add data to the form to post.
            WWWForm form = new WWWForm();
            form.AddField("oauth_callback", "oob");

            // HTTP header
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);
            parameters.Add("oauth_callback", "oob");

            //var headers = new Hashtable();
            var headers = new Dictionary<string, string>();
            headers["Authorization"] = GetFinalOAuthHeader("POST", RequestTokenURL, parameters);

            return new WWW(RequestTokenURL, form.data, headers);
        }

        private static WWW WWWAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin)
        {
            // Need to fill body since Unity doesn't like an empty request body.
            byte[] dummmy = new byte[1];
            dummmy[0] = 0;

            // HTTP header
            var headers = new Dictionary<string,string>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);
            parameters.Add("oauth_token", requestToken);
            parameters.Add("oauth_verifier", pin);

            headers["Authorization"] = GetFinalOAuthHeader("POST", AccessTokenURL, parameters);
            
            return new WWW(AccessTokenURL, dummmy, headers);
        }

        private static string GetHeaderWithAccessToken(string httpRequestType, string apiURL, string consumerKey, string consumerSecret, AccessTokenResponse response, Dictionary<string, string> parameters)
        {
            AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);

            parameters.Add("oauth_token", response.Token);
            parameters.Add("oauth_token_secret", response.TokenSecret);

            return GetFinalOAuthHeader(httpRequestType, apiURL, parameters);
        }
        #endregion

        #region Twitter API Methods

        private const string PostTweetURL = "https://api.twitter.com/1.1/statuses/update.json";

        //GET statuses/user_timeline
        private const string GetTweetsURL = "https://api.twitter.com/1.1/statuses/user_timeline.json";

        //GET users/show
        public const string GetAccountURL = "https://api.twitter.com/1.1/users/show.json";


        public static IEnumerator PostTweet(string text, string consumerKey, string consumerSecret, AccessTokenResponse response, PostTweetCallback callback)
        {
            if (string.IsNullOrEmpty(text) || text.Length > 140)
            {
                Debug.Log(string.Format("PostTweet - text[{0}] is empty or too long.", text));

                callback(false);
            }
            else
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("status", text);

                // Add data to the form to post.
                WWWForm form = new WWWForm();
                form.AddField("status", text);
                // HTTP header
                var headers = new Dictionary<string,string>();
                headers["Authorization"] = GetHeaderWithAccessToken("POST", PostTweetURL, consumerKey, consumerSecret, response, parameters);

                WWW web = new WWW(PostTweetURL, form.data, headers);
               
                yield return web;

                if (!string.IsNullOrEmpty(web.error))
                {
					Debug.Log(string.Format("PostTweet - failed. {0}\n{1}", web.error, web.text));
					callback(false);
                }
                else
                {
                    string error = Regex.Match(web.text, @"<error>([^&]+)</error>").Groups[1].Value;

                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.Log(string.Format("PostTweet - failed. {0}", error));
                        callback(false);
                    }
                    else
                    {
                        callback(true);
                    }
                }
            }
        }
        
        public static string GetTwitterAccessToken(string consumerKey, string consumerSecret)
        {
            string URL_ENCODED_KEY_AND_SECRET = Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey + ":"+consumerSecret));

            byte[] body;
            body = Encoding.UTF8.GetBytes("grant_type=client_credentials");
            
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Basic " + URL_ENCODED_KEY_AND_SECRET;

            WWW web = new WWW("https://api.twitter.com/oauth2/token", body, headers);

            while(!web.isDone)
            {
                Debug.Log("Retrieving acess token...");
            }
            string output = web.text.Replace("{\"token_type\":\"bearer\",\"access_token\":\"", "");
            output = output.Replace("\"}", "");

            return output;
        }

        [System.Serializable]
        public class tw_DateTime
        {
            public string Weekday;
            public string Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;
            public string Offset;
            public int Year;
        }

        [System.Serializable]
        public class Tweet
        {
            public tw_DateTime dateTime;
            public string Text;
            public string ID;
            public string UserID;
            public int RTs;
            public int Favs;
        }

        public static void GetUserTimeline(string name, string AccessToken, int count, twitterButton caller)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=" + name + "&count=" + count + "&trim_user=1" + "&include_rts=0&exclude_replies=true&contributor_details=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }

            //find user mentions
            List<string> mentions = extractData(web.text, ",\"user_mentions\":", ",\"urls\":");
            //remove if true
            string extractMe;
            if (ammendOutputText == null)
                extractMe = web.text;
            else
                extractMe = ammendOutputText;

            List<string> dateTime = extractData(extractMe, "{\"created_at\":\"", "\",\"id\":");
            List<string> text = extractData(extractMe, ",\"text\":\"", "\",\"truncated\":");
            List<string> favs = extractData(extractMe, "\"favorite_count\":", ",\"entities\":");
            List<string> RTs = extractData(extractMe, "\"retweet_count\":", ",\"favorite_count\":");
            List<string> userID = extractData(extractMe, "\"user\":{\"id\":", "\"},\"geo\":");
            List<string> tweetID = extractData(extractMe, ",\"id\":", "\",\"text\":");

            List<Tweet> tweets = new List<Tweet>();

            for (int i = 0; i < text.Count; i++)
            {
                Tweet thisTweet = new Tweet();
                #region dateTime formating
                string temp = "";
                List<string> boop = new List<string>();
                for (int k = 0; k < dateTime[i].Length; k++)
                {
                    if (dateTime[i][k] != ' ')
                        temp += dateTime[i][k];
                    else
                    {
                        boop.Add(temp);
                        temp = "";
                    }

                    if (k == dateTime[i].Length - 1)
                        boop.Add(temp);
                }
                temp = "";
                List<string> doop = new List<string>();
                for (int k = 0; k < boop[3].Length; k++)
                {
                    if (boop[3][k] != ':')
                        temp += boop[3][k];
                    else
                    {
                        doop.Add(temp);
                        temp = "";
                    }

                    if (k == boop[3].Length - 1)
                        doop.Add(temp);
                }

                tw_DateTime time = new tw_DateTime();
                time.Weekday = boop[0];
                time.Month = boop[1];
                time.Day = int.Parse(boop[2]);
                time.Hour = int.Parse(doop[0]);
                time.Minute = int.Parse(doop[1]);
                time.Second = int.Parse(doop[2]);
                time.Year = int.Parse(boop[5]);
                time.Offset = boop[4];
                #endregion

                thisTweet.dateTime = time;
                thisTweet.Text = text[i];
                thisTweet.UserID = userID[i].Substring(0, userID[i].IndexOf(",\"id_str"));
                thisTweet.RTs = int.Parse(RTs[i]);
                thisTweet.Favs = int.Parse(favs[i]);
                thisTweet.ID = tweetID[i].Substring(0,tweetID[i].IndexOf(",\"id_str"));

                tweets.Add(thisTweet);
            }
            caller.tweets = tweets;
            ammendOutputText = null;
        }

        public static void GetProfile(string name, string AccessToken, twitterButton caller)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?screen_name=" + name, null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }
            Debug.Log(web.text);
            List<string> URL = extractData(web.text, ",\"profile_image_url\":\"", "\",\"profile_image_url_https\":");
            List<string> verified = extractData(web.text, ",\"verified\":", ",\"statuses_count\":");
            caller.verified = Convert.ToBoolean(verified[0]);
            Debug.Log(caller.verified);
            URL[0]=URL[0].Remove(URL[0].IndexOf("_normal"), 7);
            caller.StartCoroutine(twitterButton.setAvatar(URL[0]));
        }
        #endregion

        public static List<string> extractData(string outputText, string start)
        {
            List<int> startPos = new List<int>();
            int i = 0;
            while ((i = outputText.IndexOf(start, i)) != -1)
            {
                startPos.Add(i);
                i++;
            }
            List<string> returnMe = new List<string>();
            for (int j = startPos.Count - 2; j > -1; j--)
            {
                string output = "";
                for (int c = startPos[j]; c < startPos[j + 1]; c++)
                {
                    output += outputText[c];
                }
                output = output.Replace(start, "");
                output = output.Replace("\\n", " ");
                output = output.Replace("\\", "");

                returnMe.Add(output);
            }
            return returnMe;
        }
        public static List<string> extractData(string outputText, string start, string end)
        {
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            int i = 0;
            //Find all the position of all mentions of "text":
            while ((i=outputText.IndexOf(start,i))!=-1)
            {
                startPos.Add(i);
                i++;
            }

            i = 0;
            //Do the same for "source":
            while ((i = outputText.IndexOf(end, i)) != -1)
            {
                stopPos.Add(i);
                i++;
            }

            List<string> returnMe = new List<string>();

            //for (int j = 0; j < startPos.Count; j++)
            for (int j = startPos.Count-1; j>-1;j--)
            {
                string output = "";
                for (int c = startPos[j]; c < stopPos[j]; c++)
                {
                    output += outputText[c];
                }

                output = output.Replace(start, "");
                output = output.Replace("\ud83c[\udf00-\udfff]", " ! ");
                output = output.Replace("\\\"", "\"");
                output = output.Replace("\\/", "/");
                output = output.Replace("&amp;", "&");

                List<int> fuckyou = new List<int>();
                i = 0;
                while ((i = output.IndexOf("\\u", i)) != -1)
                {
                    fuckyou.Add(i);
                    i++;
                }

                for (int u= fuckyou.Count-1; u >-1 ; u--)
                    output = output.Remove(fuckyou[u], 6);

                if (output != "[]" && start == ",\"user_mentions\":")
                {
                    //Then remove text from original input.
                    //Remove each section of the string STARTING AT THE END AND WORKING BACK
                    outputText = outputText.Remove(startPos[j] + 1 + start.Length, output.Length - 1);
                    output = null;
                    ammendOutputText = outputText;
                }

                returnMe.Add(output);
            }
            return returnMe;
        }

        public static string ammendOutputText=null;

        #region OAuth Help Methods
        // The below help methods are modified from "WebRequestBuilder.cs" in Twitterizer(http://www.twitterizer.net/).
        // Here is its license.

        //-----------------------------------------------------------------------
        // <copyright file="WebRequestBuilder.cs" company="Patrick 'Ricky' Smith">
        //  This file is part of the Twitterizer library (http://www.twitterizer.net/)
        // 
        //  Copyright (c) 2010, Patrick "Ricky" Smith (ricky@digitally-born.com)
        //  All rights reserved.
        //  
        //  Redistribution and use in source and binary forms, with or without modification, are 
        //  permitted provided that the following conditions are met:
        // 
        //  - Redistributions of source code must retain the above copyright notice, this list 
        //    of conditions and the following disclaimer.
        //  - Redistributions in binary form must reproduce the above copyright notice, this list 
        //    of conditions and the following disclaimer in the documentation and/or other 
        //    materials provided with the distribution.
        //  - Neither the name of the Twitterizer nor the names of its contributors may be 
        //    used to endorse or promote products derived from this software without specific 
        //    prior written permission.
        // 
        //  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
        //  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
        //  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
        //  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
        //  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
        //  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
        //  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
        //  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
        //  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
        //  POSSIBILITY OF SUCH DAMAGE.
        // </copyright>
        // <author>Ricky Smith</author>
        // <summary>Provides the means of preparing and executing Anonymous and OAuth signed web requests.</summary>
        //-----------------------------------------------------------------------

        private static readonly string[] OAuthParametersToIncludeInHeader = new[]
                                                          {
                                                              "oauth_version",
                                                              "oauth_nonce",
                                                              "oauth_timestamp",
                                                              "oauth_signature_method",
                                                              "oauth_consumer_key",
                                                              "oauth_token",
                                                              "oauth_verifier"
                                                              // Leave signature omitted from the list, it is added manually
                                                              // "oauth_signature",
                                                          };

        private static readonly string[] SecretParameters = new[]
                                                                {
                                                                    "oauth_consumer_secret",
                                                                    "oauth_token_secret",
                                                                    "oauth_signature"
                                                                };

        private static void AddDefaultOAuthParams(Dictionary<string, string> parameters, string consumerKey, string consumerSecret)
        {
            parameters.Add("oauth_version", "1.0");
            parameters.Add("oauth_nonce", GenerateNonce());
            parameters.Add("oauth_timestamp", GenerateTimeStamp());
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_consumer_secret", consumerSecret);
        }

        private static string GetFinalOAuthHeader(string HTTPRequestType, string URL, Dictionary<string, string> parameters)
        {
            // Add the signature to the oauth parameters
            string signature = GenerateSignature(HTTPRequestType, URL, parameters);

            parameters.Add("oauth_signature", signature);

            StringBuilder authHeaderBuilder = new StringBuilder();
            authHeaderBuilder.AppendFormat("OAuth realm=\"{0}\"", "Twitter API");

            var sortedParameters = from p in parameters
                                   where OAuthParametersToIncludeInHeader.Contains(p.Key)
                                   orderby p.Key, UrlEncode(p.Value)
                                   select p;

            foreach (var item in sortedParameters)
            {
                authHeaderBuilder.AppendFormat(",{0}=\"{1}\"", UrlEncode(item.Key), UrlEncode(item.Value));
            }

            authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));

            return authHeaderBuilder.ToString();
        }

        private static string GenerateSignature(string httpMethod, string url, Dictionary<string, string> parameters)
        {
            var nonSecretParameters = (from p in parameters
                                       where !SecretParameters.Contains(p.Key)
                                       select p);

            // Create the base string. This is the string that will be hashed for the signature.
            string signatureBaseString = string.Format(CultureInfo.InvariantCulture,
                                                       "{0}&{1}&{2}",
                                                       httpMethod,
                                                       UrlEncode(NormalizeUrl(new Uri(url))),
                                                       UrlEncode(nonSecretParameters));

            // Create our hash key (you might say this is a password)
            string key = string.Format(CultureInfo.InvariantCulture,
                                       "{0}&{1}",
                                       UrlEncode(parameters["oauth_consumer_secret"]),
                                       parameters.ContainsKey("oauth_token_secret") ? UrlEncode(parameters["oauth_token_secret"]) : string.Empty);


            // Generate the hash
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(key));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
            return Convert.ToBase64String(signatureBytes);
        }

        private static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
        }

        private static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return new System.Random().Next(123400, int.MaxValue).ToString("X", CultureInfo.InvariantCulture);
        }

        private static string NormalizeUrl(Uri url)
        {
            string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }

            normalizedUrl += url.AbsolutePath;
            return normalizedUrl;
        }

        private static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }

        private static string UrlEncode(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder parameterString = new StringBuilder();

            var paramsSorted = from p in parameters
                               orderby p.Key, p.Value
                               select p;

            foreach (var item in paramsSorted)
            {
                if (parameterString.Length > 0)
                {
                    parameterString.Append("&");
                }

                parameterString.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}={1}",
                        UrlEncode(item.Key),
                        UrlEncode(item.Value)));
            }

            return UrlEncode(parameterString.ToString());
        }

        #endregion
    }
}