using UnityEngine;
using System.Collections;

public class pauseManager : MonoBehaviour
{

    public GameObject allTheCanvases, pointer, pauseScreen;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !twitterButton.instance.running)
            {
                pauseUnpause(allTheCanvases.activeSelf);
            }
        }
    }

    public void pauseUnpause(bool b)
    {
        if (b)
        {
            Time.timeScale = 0;
            allTheCanvases.SetActive(false);
            pointer.SetActive(false);
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            allTheCanvases.SetActive(true);
            pointer.SetActive(true);
            pauseScreen.SetActive(false);
        }
    }
    public void exit()
    {
        Application.Quit();
    }
}
