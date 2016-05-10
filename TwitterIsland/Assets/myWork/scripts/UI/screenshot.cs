using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class screenshot : MonoBehaviour
{
    public Image flashImage,galleryBackground;

    public Image[] imagesToTurnOff;
    public Text[] textToTurnOff;

    public List<KeyValuePair<string,bool>> mediaIDs = new List<KeyValuePair<string, bool>>();

    public Image screencap, postcardTopper,dummyBackground,stamp;
    public Text postcardTopperDATE, currentImageText, postCardAddress,outputText;
    public GameObject FinalThingBeforeUpload;
    public GameObject[] postcardButtons;
    public InputField statusInput;
    public Button galleryViewButton, galleryExitButton, leftBtn, rightBtn;
    public Image displayScreenshots;
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
        galleryViewButton.interactable = (screenshots.Count > 0) ? true : false;
        rightBtn.interactable = (screenshots.Count > 1 && currentImage  < screenshots.Count-1) ? true:false;

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
            if (Vector3.Distance(screencap.rectTransform.localPosition,lerpPos) < 0.2f)
            {
                lerp = false;
                screencap.DOFade(0, 1);
            }
        }
    }

    public void takeScreenshot()
    {
        StopCoroutine(CaptureScreen());
        StartCoroutine(CaptureScreen());
    }

    //Display the 3 most recent screenshots
    #region Gallery view
    public void enterGallery()
    {
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
        galleryExitButton.gameObject.SetActive(true);
        galleryBackground.gameObject.SetActive(true);
        galleryBackground.DOFade(.5f, 1);

        displayScreenshots.sprite = Sprite.Create(screenshots[currentImage], new Rect(0, 0, screenshots[currentImage].width, screenshots[currentImage].height), Vector2.zero);
        displayScreenshots.rectTransform.sizeDelta = new Vector2(screenshots[currentImage].width / 2, screenshots[currentImage].height / 2);
        currentImageText.text = (currentImage + 1) + "/" + screenshots.Count;
    }

    public void setUsernameTextOnPostcard()
    {
        postCardAddress.text = "@" + twitterButton.instance.userName;        
        postCardAddress.text += "\nTwitter.com\n";
        postCardAddress.text += "<size=55>199.59.150.39</size>";
    }

    int currentImage = 0;
    public void changeImage(int dir)
    {
        currentImage += dir;
        currentImage = (currentImage < 0) ? 0 : currentImage;
        currentImage = (currentImage == screenshots.Count) ? screenshots.Count - 1 : currentImage;
        leftBtn.interactable = (currentImage > 0) ? true : false;
        rightBtn.interactable = (currentImage < screenshots.Count-1) ? true : false;

        currentImageText.text = (currentImage+1) + "/" +screenshots.Count;

        displayScreenshots.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-2.5f, 2.5f));
        displayScreenshots.rectTransform.localPosition = Vector3.zero;
        displayScreenshots.rectTransform.sizeDelta = new Vector2(screenshots[currentImage].width / 2, screenshots[currentImage].height / 2);
        displayScreenshots.sprite = Sprite.Create(screenshots[currentImage], new Rect(0, 0, screenshots[currentImage].width, screenshots[currentImage].height), Vector2.zero);
    }

    public void ExitGallery()
    {
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
        galleryExitButton.gameObject.SetActive(false);
        galleryBackground.gameObject.SetActive(false);
        dummyBackground.gameObject.SetActive(true);
        dummyBackground.DOFade(0f, 1);
        galleryBackground.DOFade(0f, 0);
        Invoke("disableDummyImage", 1);
    }

    void disableDummyImage()
    {
        dummyBackground.gameObject.SetActive(false);
        dummyBackground.DOFade(0.5f, 0);
    }
    #endregion


    public void postThePostcard(bool skipBack)
    {
        StopCoroutine("capturePostCardBack");
        StartCoroutine(capturePostCardBack(skipBack));
    }

    IEnumerator capturePostCardBack(bool skip)
    {
        mediaIDs.Clear();
        if (!skip)
        {
            //Turn things off/on
            foreach (GameObject i in postcardButtons)
            {
                i.SetActive(false);
            }

            stamp.enabled = true;
            yield return new WaitForEndOfFrame();
            Texture2D postcardBack = CaptureScreenPixels();
            stamp.enabled = false;

            //Turn things off/on
            foreach (GameObject i in postcardButtons)
            {
                i.SetActive(true);
            }
            EncodeAndUpload(postcardBack,true);
        }
        EncodeAndUpload(screenshots[currentImage],false);
        FinalThingBeforeUpload.SetActive(true);
    }

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
        postcardTopperDATE.text = "";

        if (System.DateTime.Now.Day < 10)
            postcardTopperDATE.text += "0";

        postcardTopperDATE.text += System.DateTime.Now.Day + ".";

        if (System.DateTime.Now.Month < 10)
            postcardTopperDATE.text += "0";

        //Wow fucking magic numbers, this method of formatting the date will be obsolete in 84 years
        postcardTopperDATE.text += System.DateTime.Now.Month + "." + (System.DateTime.Now.Year - 2000);

        screencap.DOKill();
        lerp = false;
        screencap.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
        screencap.rectTransform.localPosition = Vector3.zero;
        yield return new WaitForEndOfFrame();
        //Make a new sprite using this texture 
        screencap.sprite = Sprite.Create(CaptureScreenPixels(), (new Rect(0, 0, Screen.width, Screen.height)), Vector2.zero);
        screencap.DOFade(1, 0);
        screencap.DOComplete();

        //We're done with these top bits now
        postcardTopper.enabled = false;
        postcardTopperDATE.enabled = false;

        //Make colourful border
        float borderSize = Screen.height / 25;
        //Make the original screen capture a little bit smaller
        screencap.rectTransform.sizeDelta = new Vector2(Screen.width - borderSize, Screen.height - borderSize);
        screencap.enabled = true;
        //Turn on border sprite
        screencap.GetComponentsInParent<RectTransform>()[1].sizeDelta = new Vector2(Screen.width, Screen.height);
        screencap.GetComponentsInParent<Image>()[1].enabled = true;

        //Repeat screen capture process
        yield return new WaitForEndOfFrame();
        Texture2D newScreenshot = CaptureScreenPixels();
        screencap.sprite = Sprite.Create(newScreenshot, (new Rect(0, 0, Screen.width, Screen.height)), Vector2.zero);

        //Shrink this image and put it in the middle of the screen where we can see it :)
        screencap.rectTransform.sizeDelta = new Vector2(Screen.width / 2, Screen.height / 2);

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

        screenshots.Add(newScreenshot);

        //Post this bad boy to Twitter
        //PostToTwitter(newScreenshot);

        yield return new WaitForSeconds(1);
        lerpScale = new Vector2(Screen.width / 5, Screen.height / 5);
        lerpPos = new Vector3((Screen.width / 2) - ((screencap.rectTransform.sizeDelta.x / 5)) - 15, 0, 0);
        lerp = true;

    }

    void EncodeAndUpload(Texture2D image,bool isBack)
    {
        System.Convert.ToBase64String(image.EncodeToPNG());
        twitterButton.instance.PostScreenshotToTwitter(System.Convert.ToBase64String(image.EncodeToPNG()),this,isBack);
    }

    public void postThisTotwitter()
    {
        outputText.color = Color.white;
        outputText.text = "Uploading to Twitter...";
        if (mediaIDs.Count > 1)
        {
            //If you're trying to post two of the same thing for some bizzare reason
            if (mediaIDs[0].Value == mediaIDs[1].Value)
                twitterButton.instance.postMe(statusInput.text, mediaIDs[0].Key, outputText);
            else if (mediaIDs[0].Value && !mediaIDs[1].Value)
                twitterButton.instance.postMe(statusInput.text, mediaIDs[1].Key + "," + mediaIDs[0].Key, outputText);
            else
                twitterButton.instance.postMe(statusInput.text, mediaIDs[0].Key + "," + mediaIDs[1].Key, outputText);
        }
        else
        {
            twitterButton.instance.postMe(statusInput.text, mediaIDs[0].Key,outputText);
        }
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