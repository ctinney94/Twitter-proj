using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class maker : MonoBehaviour {
    
    public float[] vowelCount = { 0, 0, 0, 0, 0 };
    
    // Use this for initialization
    void Start ()
    {
    }
    
    // Update is called once per frame
    void Update ()
    {
        var input = gameObject.GetComponent<InputField>();
        Debug.Log(input.text);

        string inputText = input.text;
        inputText = inputText.ToLower();
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

        for (int i = 0; i < inputText.Length; i++)
        {
            for (int v = 0; v < 4; v++)
            {
                if (inputText[i] == vowels[v])
                {
                    vowelCount[v] =+1.0f;
                }
            }
        }
    }
}
