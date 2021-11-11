using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{

    //void Start()
    //{
    //    DontDestroyOnLoad(this.gameObject);  //Allow this object to persist between scene changes
    //}

    public void LoadHomeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void OnSpotAR()
    {
        SceneManager.LoadScene("OnSpotARScene");
    }

    public void NearbyAR()
    {
        SceneManager.LoadScene("NearbyARScene");
    }

    public void BackToArMain()
    {
        SceneManager.LoadScene("HomeScene");
        
        Debug.Log("GetActiveScene.name" + SceneManager.GetActiveScene().name);

    }

    public void LoadMapScene()
    {
        SceneManager.LoadScene("CemeteryMapScene");
    }

    public void LoadRoutesScene()
    {
        SceneManager.LoadScene("RoutesScene");

    }

    public void LoadRouteListScene()
    {
        SceneManager.LoadScene("RouteListScene");

    }

    public void LoadCreateRoutesScene()
    {
        SceneManager.LoadScene("CreateRoutesScene");

    }

    public void LoadImportRoutesScene()
    {
        SceneManager.LoadScene("ImportRoutesScene");

    }

    public void LoadStartRoutesScene()
    {
        SceneManager.LoadScene("StartRoutesScene");

    }

    //    public void LoadProfileScene()
    //    {
    //        SceneManager.LoadScene();
    //    }
    //
}
