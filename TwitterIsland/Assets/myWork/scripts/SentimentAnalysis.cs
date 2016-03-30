using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SentimentAnalysis : MonoBehaviour {

    //public Text input;
    public List<string> niceWords = new List<string>();
    public List<string> notSoNiceWords = new List<string>();
    public TextAsset positiveWords, negativeWords;
    public int nice = 0, nasty = 0;
    public mood moodThing;
    public TextMesh textObj;

    // Use this for initialization
    void Start()
    {
        string[] words = Regex.Split(positiveWords.text, "\n|\r|\r\n");

        foreach (string s in words)
        {
            if (s !="")
            niceWords.Add(s);
        }

        words = Regex.Split(negativeWords.text, "\n|\r|\r\n");

        foreach (string s in words)
        {
            if (s!="")
            notSoNiceWords.Add(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //CheckText(input.text);
	}

    public string getFormattedText(string input)
    {
        string newText = input;
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + "\n")
                || input.ToLower().Contains("." + notSoNiceWords[i] + "\n")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" "+notSoNiceWords[i]))
            {
                newText = newText.ToLower().Replace(notSoNiceWords[i], "<color=red>" + notSoNiceWords[i] + "</color>");
            }
        }
        
        for (int i = 0; i < niceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().Contains(" " + niceWords[i] + "\n")
                || input.ToLower().Contains("." + niceWords[i] + "\n")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                newText = newText.ToLower().Replace(niceWords[i], "<color=lime>" + niceWords[i] + "</color>");
            }
        }
        return newText;
    }
    public float getSAValue(string input)
    {
        nasty = 0;
        nice = 0;
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" " + notSoNiceWords[i]))
            {
                Debug.Log("Nasty word " +notSoNiceWords[i]);
                nasty++;
            }
        }
        for (int i = 0; i < niceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                Debug.Log("Nice word " + niceWords[i]);
                nice++;
            }
        }
        float t = nice + nasty;
        float result, a = 0, b = 0;
        if (t != 0)
            t = 1 / t;
        if (nice != 0)
            a = t * nice;
        if (nasty != 0)
            b = t * nasty;
        result = a - b;
        return result;
    }

    public void CheckText(string input)
    {
        string newText = input;

        /*char[] delimChars = { ' ', '\n', ',', '.', ';', '#' };
        string[] words = input.Split(delimChars);
        foreach (string s in words)
            Debug.Log(s);*/

        nasty = 0;
        nice = 0;
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" " + notSoNiceWords[i]))
            {
                Debug.Log(newText);
                Debug.Log(notSoNiceWords[i]);
                newText = newText.ToLower().Replace(notSoNiceWords[i], "<color=red>" + notSoNiceWords[i] + "</color>");
                Debug.Log(newText);
                nasty++;
                Debug.Log(notSoNiceWords[i] + " detected");
            }
        }
        for (int i = 0; i < niceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                newText = newText.ToLower().Replace(niceWords[i], "<color=lime>" + niceWords[i] + "</color>");
                nice++;
                Debug.Log(niceWords[i] + " detected");
            }
        }
        float t = nice + nasty;
        float result, a = 0, b = 0;
        if (t != 0)
            t = 1 / t;
        if (nice != 0)
            a = t * nice;
        if (nasty != 0)
            b = t * nasty;
        result = a - b;
        moodThing.moodness = result;

        if (textObj)
            textObj.text = newText;
    }
}