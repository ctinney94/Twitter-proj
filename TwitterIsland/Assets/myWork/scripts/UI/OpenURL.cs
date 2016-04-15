using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour {

	public void openThisPage(string URL)
    {
        Application.OpenURL(URL);
    }
}
