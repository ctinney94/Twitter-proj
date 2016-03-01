using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SentimentAnalysis : MonoBehaviour {

    public Text input;
    List<string> niceWords = new List<string>();
    List<string> notSoNiceWords = new List<string>();
    public int nice=0, nasty=0;
    public mood moodThing;
    // Use this for initialization
    void Start()
    {
        using (StreamReader sr = new StreamReader("Assets/Resources/negative-words.txt"))
        {
            while (!sr.EndOfStream)
            {
                notSoNiceWords.Add(sr.ReadLine());
            }
        }

        using (StreamReader sr = new StreamReader("Assets/Resources/positive-words.txt"))
        {
            while (!sr.EndOfStream)
            {
                niceWords.Add(sr.ReadLine());
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        CheckText(input.text);
	}
    void CheckText(string input)
    {
        nasty = 0;
        nice = 0;
        for (int i = 0; i < notSoNiceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + notSoNiceWords[i] + " ")
                || input.ToLower().Contains("." + notSoNiceWords[i] + " ")
                || input.ToLower().Contains(" " + notSoNiceWords[i] + "."))
            {
                nasty++;
                Debug.Log("Negative word detected - " + notSoNiceWords[i]);
            }
        }
        for (int i = 0; i < niceWords.Count; i++)
        {
            if (input.ToLower().Contains(" " + niceWords[i] + " ")
                || input.ToLower().Contains("." + niceWords[i] + " ")
                || input.ToLower().Contains(" " + niceWords[i] + "."))
            {
                nice++;
                Debug.Log("Posative word detected - " + niceWords[i]);
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
        Debug.Log("t: " + t);
        Debug.Log("a: " + a);
        Debug.Log("b: " + b);
        Debug.Log("result: " + result);
    }
}