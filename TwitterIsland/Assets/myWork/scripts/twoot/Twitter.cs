using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

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

    public class API
    {
        private static string currentDisplayName;
        
        #region Twitter API Methods
        
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
            public string DisplayName;
            public int RTs;
            public int Favs;
        }

        public static void GetUserTimeline(string name, string AccessToken, int count, twitterButton caller)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/statuses/"+"user"+"_timeline.json?screen_name=" + name + "&count=" + count + "&trim_user=1" + "&include_rts=0&exclude_replies=true&contributor_details=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }

            if (web.error != null)
            {
                GameObject.Find("Error Text").GetComponent<Text>().enabled = true;
                GameObject.Find("Error Text").GetComponent<Text>().text = web.error;
            }
            else
            {
                GameObject.Find("Error Text").GetComponent<Text>().text = "";

                //find user mentions
                List<string> mentions = extractData(web.text, ",\"user_mentions\":", ",\"urls\":");
                //remove if true
                string extractMe;
                if (ammendOutputText == null)
                    extractMe = web.text;
                else
                    extractMe = ammendOutputText;

                List<string> dateTime = extractData(extractMe, "{\"created_at\":\"", "\",\"id\":");
                List<string> text = extractData(extractMe, ",\"text\":\"", "\",\"entities\":");
                List<string> favs = extractData(extractMe, "\"favorite_count\":", ",\"favorited\":");
                List<string> RTs = extractData(extractMe, "\"retweet_count\":", ",\"favorite_count\":");
                List<string> userID = extractData(extractMe, "\"user\":{\"id\":", "\"},\"geo\":");

                //I don't actually have a reason why I need this tweet ID
                //Would it be that awful if I didn't include it?
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
                    thisTweet.ID = tweetID[i].Substring(0, tweetID[i].IndexOf(",\"id_str"));
                    thisTweet.DisplayName = currentDisplayName;

                    tweets.Add(thisTweet);
                }
                caller.tweets = tweets;
                ammendOutputText = null;
            }
        }

        public static void GetProfile(string name, string AccessToken, twitterButton caller)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?screen_name=" + name+ "&include_entities=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }
            if (web.error != null)
            {
                GameObject.Find("Error Text").GetComponent<Text>().enabled = true;
                GameObject.Find("Error Text").GetComponent<Text>().text = web.error;
                Debug.Log(web.error);
            }
            else
            {
                GameObject.Find("Error Text").GetComponent<Text>().text = "";
                                
                List<string> URL = extractData(web.text, ",\"profile_image_url\":\"", "\",\"profile_image_url_https\":");
                List<string> verified = extractData(web.text, ",\"verified\":", ",\"statuses_count\":");
                List<string> displayName = extractData(web.text, ",\"name\":\"", "\",\"screen_name\":");

                if (displayName[0] != " ")
                    currentDisplayName = displayName[0];
                else
                    currentDisplayName = "Somebody with a non-ascii name";
                
                caller.verified = Convert.ToBoolean(verified[0]);
                URL[0] = URL[0].Remove(URL[0].IndexOf("_normal"), 7);
                caller.StartCoroutine(twitterButton.setAvatar(URL[0]));
            }
        }
        #endregion
        
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

            if (startPos.Count != stopPos.Count)
                startPos.Remove(startPos[startPos.Count - 1]);

            //for (int j = 0; j < startPos.Count; j++)
            for (int j = startPos.Count-1; j>-1;j--)
            {
                string output = "";
                for (int c = startPos[j]; c < stopPos[j]; c++)
                {
                    output += outputText[c];
                }

                #region the whole user mentions bit
                if (start != ",\"user_mentions\":")
                {
                    output = output.Replace(start, "");
                    output = output.Replace("\ud83c[\udf00-\udfff]", " ! ");
                    output = output.Replace("\\\"", "\"");
                    output = output.Replace("\\/", "/");
                    output = output.Replace("&amp;", "&");

                    List<int> EmojisOrSimilar = new List<int>();
                    i = 0;
                    while ((i = output.IndexOf("\\u", i)) != -1)
                    {
                        EmojisOrSimilar.Add(i);
                        i++;
                    }

                    for (int u = EmojisOrSimilar.Count - 1; u > -1; u--)
                    {
                        output = output.Remove(EmojisOrSimilar[u], 6);
                        output.Insert(EmojisOrSimilar[u], "*!*");
                    }
                }
                if (output != "[]" && start == ",\"user_mentions\":")
                {
                    //Then remove text from original input.
                    //Remove each section of the string STARTING AT THE END AND WORKING BACK
                    outputText = outputText.Remove(startPos[j] + 1, output.Length + start.Length - 1);
                    output = null;
                    ammendOutputText = outputText;
                }
                #endregion
                
                returnMe.Add(output);
            }
            return returnMe;
        }

        public static string ammendOutputText = null;
        
    }
}