using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class screenshot : MonoBehaviour
{
    public Image flashImage;
    public Button ScreenshotButton;
    public Image screencap;
    public Image postcardTopper;
    public Text postcardTopperDATE;

    Texture2D newScreenshot;

    // Update is called once per frame
    void Update()
    {
        if (flashImage.enabled)
        {
            Color currentCol = flashImage.color;
            currentCol.a = Mathf.Lerp(flashImage.color.a, 0, Time.deltaTime);
            flashImage.color = currentCol;
            if (currentCol.a == 0)
            {
                flashImage.enabled = false;
            }
        }

    }
    
    public void takeScreenshot()
    {
        StartCoroutine(CaptureScreen());
    }

    IEnumerator CaptureScreen()
    {
        //Turn things off/on
        ScreenshotButton.GetComponent<Image>().enabled = false;
        ScreenshotButton.GetComponentInChildren<Text>().enabled = false;
        postcardTopper.enabled = true;
        postcardTopperDATE.enabled = true;

        yield return new WaitForEndOfFrame();
        //Get a screenshot as a texture
        newScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        newScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        newScreenshot.Apply();

        //Make a new sprite using this texture 
        screencap.sprite = Sprite.Create(newScreenshot, (new Rect(0, 0, Screen.width, Screen.height)), Vector2.zero);
        float borderSize = Screen.height / 25;
        
        //screencap.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width-borderSize, Screen.height-borderSize);
        screencap.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Screen.height / 2);

        //Make colourful border
        screencap.GetComponentsInParent<RectTransform>()[1].sizeDelta = new Vector2(Screen.width / 2 + borderSize, Screen.height / 2 + borderSize);
        //screencap.GetComponentsInParent<RectTransform>()[1].sizeDelta = new Vector2(Screen.width,Screen.height);
        screencap.GetComponentsInParent<Image>()[1].enabled = true;
        screencap.enabled = true;
        #region work on me
        /*
        //Repeat process
        yield return new WaitForEndOfFrame();
        //Get a screenshot as a texture
        newScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        newScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        newScreenshot.Apply();
        
        screencap.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width/2, Screen.height/2);

        //Make WHITE border
        screencap.GetComponentsInParent<RectTransform>()[1].sizeDelta = new Vector2(Screen.width/2+borderSize, Screen.height/2+borderSize);
        Sprite sp = screencap.GetComponentsInParent<Image>()[1].sprite;
        screencap.GetComponentsInParent<Image>()[1].sprite = null;
        screencap.GetComponentsInParent<Image>()[1].enabled = true;
        screencap.enabled = true;
        */

        screencap.GetComponentsInParent<RectTransform>()[1].localRotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
        #endregion
        //Turn things off/on
        postcardTopper.enabled = false;
        postcardTopperDATE.enabled = false;
        ScreenshotButton.GetComponent<Image>().enabled = true;
        ScreenshotButton.GetComponentInChildren<Text>().enabled = true;

        //Flash the camera
        camFlash();

        //Post this bad boy to Twitter
        //PostToTwitter();
    }
    
    public void PostToTwitter()
    {
        GameObject.Find("TWITTER BUTTON").GetComponent<twitterButton>().PostScreenshotToTwitter(System.Convert.ToBase64String(newScreenshot.EncodeToPNG()));
    }

    void camFlash()
    {
        //Turn alpha to 1, so it can be faded out in Update()
        Color currentCol = flashImage.color;
        currentCol.a = 1;
        flashImage.enabled = true;
        flashImage.color = currentCol;
    }
}
