using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

//For use interacting with the twitter API
namespace Twitter
{
    public class API
    {
        //Username used in the most recent API call
        private static string currentDisplayName;

        #region Twitter API Methods

        //Authorization set-up
        //Adapted from the following source:
        //SOURCE:
        //http://www.conlanrios.com/2013/10/twitter-application-only-authentication.html
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
                Debug.Log("Retrieving access token...");
            }
            //Format string response
            string output = web.text.Replace("{\"token_type\":\"bearer\",\"access_token\":\"", "");
            output = output.Replace("\"}", "");

            return output;
        }

        //Data types used for tweet data as well as detailed time info

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

        //Grab a selection of tweets from a user
        public static void GetUserTimeline(string name, string AccessToken, int count, twitterButton caller)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            //DO AN API CALL
            //For this program, the parameters used below will likely need to be changed bar username and # of tweets to pull
            WWW web = new WWW("https://api.twitter.com/1.1/statuses/"+"user"+"_timeline.json?screen_name=" + name + "&count=" + count + "&trim_user=1" + "&include_rts=0&exclude_replies=true&contributor_details=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }

            //We have an error x(
            if (web.error != null)
            {
                //Output error to UI
                GameObject.Find("Error Text").GetComponent<Text>().enabled = true;
                GameObject.Find("Error Text").GetComponent<Text>().text = web.error;
            }
            else
            {
                //We good
                GameObject.Find("Error Text").GetComponent<Text>().text = "";

                //Find user mentions sections of tweet
                List<string> mentions = extractData(web.text, ",\"user_mentions\":", ",\"urls\":");
                //If detected, remove.
                string extractMe;
                if (ammendOutputText == null)
                    extractMe = web.text;
                else
                    extractMe = ammendOutputText;

                //Extract relevant data from web response
                List<string> dateTime = extractData(extractMe, "{\"created_at\":\"", "\",\"id\":");
                List<string> text = extractData(extractMe, ",\"text\":\"", "\",\"entities\":");
                List<string> favs = extractData(extractMe, "\"favorite_count\":", ",\"favorited\":");
                List<string> RTs = extractData(extractMe, "\"retweet_count\":", ",\"favorite_count\":");
                List<string> userID = extractData(extractMe, "\"user\":{\"id\":", "\"},\"geo\":");
                List<string> tweetID = extractData(extractMe, ",\"id\":", "\",\"text\":");

                //This is what will be used to create islands
                List<Tweet> tweets = new List<Tweet>();

                //For each detected tweet
                for (int i = 0; i < text.Count; i++)
                {
                    //Create a new tweet
                    Tweet thisTweet = new Tweet();

                    #region dateTime formatting
                    string temp = "";
                    List<string> date = new List<string>();
                    for (int k = 0; k < dateTime[i].Length; k++)
                    {
                        if (dateTime[i][k] != ' ')
                            temp += dateTime[i][k];
                        else
                        {
                            date.Add(temp);
                            temp = "";
                        }

                        if (k == dateTime[i].Length - 1)
                            date.Add(temp);
                    }
                    temp = "";
                    List<string> timeOfDay = new List<string>();
                    for (int k = 0; k < date[3].Length; k++)
                    {
                        if (date[3][k] != ':')
                            temp += date[3][k];
                        else
                        {
                            timeOfDay.Add(temp);
                            temp = "";
                        }

                        if (k == date[3].Length - 1)
                            timeOfDay.Add(temp);
                    }

                    tw_DateTime time = new tw_DateTime();
                    time.Weekday = date[0];
                    time.Month = date[1];
                    time.Day = int.Parse(date[2]);
                    time.Hour = int.Parse(timeOfDay[0]);
                    time.Minute = int.Parse(timeOfDay[1]);
                    time.Second = int.Parse(timeOfDay[2]);
                    time.Year = int.Parse(date[5]);
                    time.Offset = date[4];
                    #endregion

                    //Add data to tweet
                    thisTweet.dateTime = time;
                    thisTweet.Text = text[i];
                    thisTweet.UserID = userID[i].Substring(0, userID[i].IndexOf(",\"id_str"));
                    thisTweet.RTs = int.Parse(RTs[i]);
                    thisTweet.Favs = int.Parse(favs[i]);
                    thisTweet.ID = tweetID[i].Substring(0, tweetID[i].IndexOf(",\"id_str"));
                    thisTweet.DisplayName = currentDisplayName;

                    //Add tweet
                    tweets.Add(thisTweet);
                }
                //Send the tweet data back to the button used to call this function
                caller.tweets = tweets;
                ammendOutputText = null;
            }
        }

        //Retrieve user profile information
        public static void GetProfile(string name, string AccessToken, twitterButton caller)
        {
            //Set-up API call
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + AccessToken;

            //DO AN API CALL
            //For this program, the parameters used below will likely need to be changed bar username and # of tweets to pull
            WWW web = new WWW("https://api.twitter.com/1.1/users/show.json?screen_name=" + name+ "&include_entities=false", null, headers);

            while (!web.isDone)
            {
                Debug.Log("Processing request...");
            }

            //We have an error x(
            if (web.error != null)
            {
                //Output error to UI
                GameObject.Find("Error Text").GetComponent<Text>().enabled = true;
                GameObject.Find("Error Text").GetComponent<Text>().text = web.error;
                Debug.Log(web.error);
            }
            else
            {
                //We good
                GameObject.Find("Error Text").GetComponent<Text>().text = "";
                 
                //Extract data from web response               
                List<string> URL = extractData(web.text, ",\"profile_image_url\":\"", "\",\"profile_image_url_https\":");
                List<string> verified = extractData(web.text, ",\"verified\":", ",\"statuses_count\":");
                List<string> displayName = extractData(web.text, ",\"name\":\"", "\",\"screen_name\":");

                //Output display name
                if (displayName[0] != " ")
                    currentDisplayName = displayName[0];
                else
                    currentDisplayName = "Somebody with a non-ascii name";
                
                //Output verified status of current user
                caller.verified = Convert.ToBoolean(verified[0]);
                //Format avatar URL to retrieve full size image
                URL[0] = URL[0].Remove(URL[0].IndexOf("_normal"), 7);
                //Create a texture from the users avatar (grab from URL)
                caller.StartCoroutine(twitterButton.setAvatar(URL[0]));
            }
        }
        #endregion
        
        //Used for extracting data from API response
        public static List<string> extractData(string outputText, string start, string end)
        {
            List<int> startPos = new List<int>();
            List<int> stopPos = new List<int>();
            int i = 0;
            //Find all the position of all mentions of start string:
            while ((i=outputText.IndexOf(start,i))!=-1)
            {
                startPos.Add(i);
                i++;
            }

            i = 0;
            //Do the same for end string:
            while ((i = outputText.IndexOf(end, i)) != -1)
            {
                stopPos.Add(i);
                i++;
            }

            //Data to return
            List<string> returnMe = new List<string>();

            //If we have a different number of start and end points, something has gone wrong
            if (startPos.Count != stopPos.Count)
                startPos.Remove(startPos[startPos.Count - 1]);//Try fixing

            
            for (int j = startPos.Count-1; j>-1;j--)
            {
                string output = "";
                for (int c = startPos[j]; c < stopPos[j]; c++)
                {
                    output += outputText[c];
                }

                if (start != ",\"user_mentions\":")
                {
                    //Format output string
                    output = output.Replace(start, "");
                    //Remove emoji type things
                    output = output.Replace("\ud83c[\udf00-\udfff]", " ! ");
                    output = output.Replace("\\\"", "\"");
                    output = output.Replace("\\/", "/");
                    output = output.Replace("&amp;", "&");

                    //Attempt at emoji removal
                    List<int> EmojisOrSimilar = new List<int>();
                    i = 0;
                    while ((i = output.IndexOf("\\u", i)) != -1)
                    {
                        EmojisOrSimilar.Add(i);
                        i++;
                    }

                    for (int u = EmojisOrSimilar.Count - 1; u > -1; u--)
                    {
                        //Emoji text is typically 6 characters long
                        output = output.Remove(EmojisOrSimilar[u], 6);
                        output.Insert(EmojisOrSimilar[u], "*!*");
                    }
                }
                else if (output != "[]")
                {
                    //Remove text from original input and return
                    //Remove each section of the string STARTING AT THE END AND WORKING BACK
                    outputText = outputText.Remove(startPos[j] + 1, output.Length + start.Length - 1);
                    output = null;
                    ammendOutputText = outputText;
                }
                
                returnMe.Add(output);
            }
            return returnMe;
        }

        //Used with above function
        public static string ammendOutputText = null;        
    }
}