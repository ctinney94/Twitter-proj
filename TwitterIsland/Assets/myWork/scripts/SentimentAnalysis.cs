using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SentimentAnalysis : MonoBehaviour {
    
    public List<string> niceWords = new List<string>();
    public List<string> notSoNiceWords = new List<string>();
    public TextAsset positiveWords, negativeWords;
    public int nice = 0, nasty = 0;
    public mood moodThing;
    public TextMesh textObj;

    //Initialization
    void Start()
    {
        //Split the text files of pos/neg words into word arrays
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
    
    //Returns the input string highlighting pos/neg words in colour
    public string getFormattedText(string input)
    {
        string newText = input;

        //Check the text for nasty words
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            //Probs could've used regex here
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + "\n")
                || input.ToLower().Contains("." + notSoNiceWords[i] + "\n")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" "+notSoNiceWords[i]))
            {
                //Replace word with red text
                newText = newText.ToLower().Replace(notSoNiceWords[i], "<color=red>" + notSoNiceWords[i] + "</color>");
            }
        }
        
        //Check the text for nice words
        for (int i = 0; i < niceWords.Count; i++)
        {
            //Probs could've used regex here
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().Contains(" " + niceWords[i] + "\n")
                || input.ToLower().Contains("." + niceWords[i] + "\n")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                //Replace word with green text
                newText = newText.ToLower().Replace(niceWords[i], "<color=lime>" + niceWords[i] + "</color>");
            }
        }
        return newText;
    }

    //Return a value from -1 to 1 for the sentiment of a given string
    public float getSAValue(string input)
    {
        nasty = 0;
        nice = 0;
        
        //Check the text for nasty words
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            //Probs could've used regex here
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" " + notSoNiceWords[i]))
            {
                //Found a nasty word :(
                nasty++;
            }
        }

        //Check the text for nice words
        for (int i = 0; i < niceWords.Count; i++)
        {
            //Probs could've used regex here
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                //Found a nice word!
                nice++;
            }
        }

        //Find sentiment value from number of pos/neg words detected
        float t = nice + nasty;//Total words detected
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

    //Not currenty used, basically the same as the other functions here
    public void CheckText(string input)
    {
        string newText = input;
        
        nasty = 0;
        nice = 0;
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            //Probs could've used regex here
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + ".")
                || input.ToLower().StartsWith(notSoNiceWords[i] + " ")
                || input.ToLower().StartsWith(notSoNiceWords[i] + ",")
                || input.ToLower().EndsWith(" " + notSoNiceWords[i]))
            {
                //Replace word with red text
                newText = newText.ToLower().Replace(notSoNiceWords[i], "<color=red>" + notSoNiceWords[i] + "</color>");
                nasty++;
            }
        }
        for (int i = 0; i < niceWords.Count; i++)
        {
            //A Regex expression would've been a far more elegant solution compared to this
            //At least I can guarantee this method works.
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + ".")
                || input.ToLower().StartsWith(niceWords[i] + " ")
                || input.ToLower().StartsWith(niceWords[i] + ",")
                || input.ToLower().EndsWith(" " + niceWords[i]))
            {
                //Replace word with green text
                newText = newText.ToLower().Replace(niceWords[i], "<color=lime>" + niceWords[i] + "</color>");
                nice++;
            }
        }

        //Find sentiment value from number of pos/neg words detected
        float t = nice + nasty;//Total words detected
        float result, a = 0, b = 0;
        if (t != 0)
            t = 1 / t;
        if (nice != 0)
            a = t * nice;
        if (nasty != 0)
            b = t * nasty;
        result = a - b;
        moodThing.moodness = result;

        //Update text mesh text
        if (textObj)
            textObj.text = newText;
    }
}