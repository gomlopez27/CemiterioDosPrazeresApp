using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{

    // Ctrl + K + C / U
    public List<NavButton> navButtons;
    public NavButton selectedButton;
    public List<GameObject> objectsToSwap;
    public List<Sprite> activeIcons;
    public List<Sprite> inactiveIcons;

    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    public void Subscribe(NavButton button)
    {
        if(navButtons == null)
        {
            navButtons = new List<NavButton>();
        }
        navButtons.Add(button);

    }


    public void OnNavExit(NavButton button)
    {
       
        ResetTabs();
    }

    public void OnNavSelected(NavButton button)
    {
      
        selectedButton = button;

        int index = button.transform.GetSiblingIndex() - 1;

        selectedButton.background.sprite = activeIcons[index];

        //switch (index)
        //{
        //    case 0:
        //        SceneManager.LoadScene("HomeScene");
        //        break;
        //    case 1:
        //        SceneManager.LoadScene("MapScene");
        //        break;
        //    case 2:
        //        SceneManager.LoadScene("MapScene");
        //        break;
        //    default:
        //        SceneManager.LoadScene("HomeScene");
        //        break;
        //}

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);

            }

        }
    }

    public void ResetTabs()
    {
        foreach (NavButton button in navButtons)
        {
            if (selectedButton != null && button == selectedButton) //skip over the currently selected tab
            {
                continue;
            }
            button.background.sprite = inactiveIcons[button.transform.GetSiblingIndex() - 1];

        }
    }




    //public void HomeButton()
    //{
    //    SceneManager.LoadScene("HomeScene");
    //}


    //public void ArButton()
    //{
    //    SceneManager.LoadScene("OnSpotARScene");

    //}

    //public void MapButton()
    //{
    //    SceneManager.LoadScene("MapScene");
    //}

    //public void RouteButton()
    //{
    //    SceneManager.LoadScene("RoutesScene");
    //}

    //public void SettingsButton()
    //{
    //    SceneManager.LoadScene("SettingsScene");
    //}
}
