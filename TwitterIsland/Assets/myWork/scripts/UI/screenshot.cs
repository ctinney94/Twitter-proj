using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class screenshot : MonoBehaviour
{
    public Image flashImage;

    public Image[] imagesToTurnOff;
    public Text[] textToTurnOff;

    public Image screencap;
    public Image postcardTopper;
    public Text postcardTopperDATE;
    public Image[] thumbnails;

    List<Texture2D> screenshots = new List<Texture2D>();
    bool lerp;
    void Awake()
    {
        lerpScale = screencap.rectTransform.sizeDelta;
        lerpPos = screencap.rectTransform.localPosition;
    }

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
        if (lerp)
        {
            screencap.rectTransform.sizeDelta = Vector3.Lerp(screencap.rectTransform.sizeDelta, lerpScale, Time.deltaTime*3);
            screencap.rectTransform.localPosition = Vector2.Lerp(screencap.rectTransform.localPosition, lerpPos, Time.deltaTime*5);
            if (screencap.rectTransform.localPosition == lerpPos)
                lerp = false;
        }
    }

    public void takeScreenshot()
    {
        StartCoroutine(CaptureScreen());
    }

    //Display the 3 most recent screenshots

    int imageToUse = 0;
    Vector2 lerpScale;
    Vector3 lerpPos;
    IEnumerator CaptureScreen()
    {
        //Turn things off/on
        foreach (Image i in imagesToTurnOff)
        {
            i.enabled = false;
            if (i.gameObject.GetComponent<Button>())
                i.gameObject.GetComponent<Button>().enabled = false;
        }
        foreach (Text t in textToTurnOff)
        {
            t.enabled = false;
        }
        postcardTopper.enabled = true;
        postcardTopperDATE.enabled = true;

        lerp = false;
        screencap.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
        screencap.rectTransform.localPosition = Vector3.zero;
        yield return new WaitForEndOfFrame();
        //Make a new sprite using this texture 
        screencap.sprite = Sprite.Create(CaptureScreenPixels(), (new Rect(0, 0, Screen.width, Screen.height)), Vector2.zero);

        //We're done with these top bits now
        postcardTopper.enabled = false;
        postcardTopperDATE.enabled = false;

        //Make colourful border
        float borderSize = Screen.height / 25;
        //Make the original screen capture a little bit smaller
        screencap.rectTransform.sizeDelta = new Vector2(Screen.width-borderSize, Screen.height-borderSize);
        screencap.enabled = true;
        //Turn on border sprite
        screencap.GetComponentsInParent<RectTransform>()[1].sizeDelta = new Vector2(Screen.width,Screen.height);
        screencap.GetComponentsInParent<Image>()[1].enabled = true;

        //Repeat screen capture process
        yield return new WaitForEndOfFrame();
        Texture2D newScreenshot = CaptureScreenPixels();
        screencap.sprite = Sprite.Create(newScreenshot, (new Rect(0, 0, Screen.width, Screen.height)), Vector2.zero);

        //Shrink this image and put it in the middle of the screen where we can see it :)
        screencap.rectTransform.sizeDelta = new Vector2(Screen.width/2, Screen.height/2);
        
        //Turn things off/on
        foreach (Image i in imagesToTurnOff)
        {
            i.enabled = true;
            if (i.gameObject.GetComponent<Button>())
                i.gameObject.GetComponent<Button>().enabled = true;
        }
        foreach (Text t in textToTurnOff)
        {
            t.enabled = true;
        }
        screencap.GetComponentsInParent<Image>()[1].enabled = false;
        screencap.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
        
        //Flash the camera
        camFlash();

        //Post this bad boy to Twitter
        PostToTwitter(newScreenshot);

        yield return new WaitForSeconds(1);
        lerpScale = new Vector2(Screen.width / 5, Screen.height / 5);
        lerpPos = new Vector3((Screen.width/2)-((screencap.rectTransform.sizeDelta.x/5))-15, 0, 0);
        lerp = true;
    }
    
    public void PostToTwitter(Texture2D image)
    {
        GameObject.Find("TWITTER BUTTON").GetComponent<twitterButton>().PostScreenshotToTwitter(System.Convert.ToBase64String(image.EncodeToPNG()));
    }

    Texture2D CaptureScreenPixels()
    {
        Texture2D newScreenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        newScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        newScreenshot.Apply();
        return newScreenshot;
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
