using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class pauseManager : MonoBehaviour
{
    public static pauseManager instance;

    int cursorIndex = 0;
    public GameObject allTheCanvases, pointer, pauseScreen;
    public GameObject[] menuItems;
    public Color highlightColour;

    public bool allowCursorMove;

    void Start()
    {
        instance = this;
    }
    void Update()
    {
        #region Enter/exit pause mode
        if (Camera.main)
        {
            if (allTheCanvases.activeSelf)
            {
                if (!twitterButton.instance.running)
                {
                    if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)))
                    {
                        pauseUnpause(allTheCanvases.activeSelf);
                    }
                }
            }
            else
            {
                if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)))
                {
                    pauseUnpause(allTheCanvases.activeSelf);
                }
            }
        }
        #endregion

        #region Change menu selection item
        if (!allTheCanvases.activeSelf)
        {
            if (Mathf.Abs(Input.GetAxis("JoypadZoom")) > .0f)
            {
                Debug.Log(Input.GetAxis("JoypadZoom"));
                changeCursorIndex(cursorIndex +((Input.GetAxis("JoypadZoom") > 0) ? -1 : 1));
                allowCursorMove = false;
            }
            else if (Mathf.Abs(Input.GetAxis("Up/Down")) > .2f)
            {
                changeCursorIndex(cursorIndex + ((Input.GetAxis("Up/Down") > 0) ? -1 : 1));
                allowCursorMove = false;
            }
            else
            {
                allowCursorMove = true;
            }
            switch (cursorIndex)
            {
                case 0:
                    menuItems[cursorIndex].GetComponent<Text>().color = highlightColour;
                    menuItems[1].GetComponent<Image>().color = Color.white;
                    menuItems[2].GetComponent<Text>().color = Color.white;
                    break;
                case 1:
                    menuItems[0].GetComponent<Text>().color = Color.white;
                    menuItems[1].GetComponent<Image>().color = highlightColour;
                    menuItems[2].GetComponent<Text>().color = Color.white;
                    break;
                case 2:
                    menuItems[0].GetComponent<Text>().color = Color.white;
                    menuItems[1].GetComponent<Image>().color = Color.white;
                    menuItems[cursorIndex].GetComponent<Text>().color = highlightColour;
                    break;
            }
        }
        #endregion

        if (!allTheCanvases.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                if (cursorIndex != 1)
                {
                    menuItems[cursorIndex].GetComponentInParent<Button>().onClick.Invoke();
                }
            }
            if (cursorIndex == 1 && (Mathf.Abs(Input.GetAxis("Left/Right")) > .1f))
            {
                menuItems[1].GetComponentInParent<Slider>().value += Input.GetAxis("Left/Right")*.05f;
            }
            if (cursorIndex == 1 && (Mathf.Abs(Input.GetAxis("SwitchIsland")) > .1f))
            {
                menuItems[1].GetComponentInParent<Slider>().value += Input.GetAxis("SwitchIsland") * .05f;
            }
        }
    }

    public void changeCursorIndex(int newIndex)
    {
        if (allowCursorMove)
        {
            if (newIndex > menuItems.Length - 1)
                newIndex = 0;
            if (newIndex < 0)
                newIndex = menuItems.Length - 1;

            cursorIndex = newIndex;
        }
    }

    public void pauseUnpause(bool b)
    {
        allowCursorMove = b;
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
