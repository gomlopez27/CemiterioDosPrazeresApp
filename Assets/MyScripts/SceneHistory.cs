using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHistory : MonoBehaviour
{
  
    public const string OnSpotARScene = "OnSpotARScene";
    public const string NearbyARScene = "NearbyARScene";
    public const string RoutesScene = "RoutesScene";
    public static string PreviousScene { get; private set; }
    public static string PreviousSceneToAr{ get; private set; }
    public static GameObject[] CurrentGameObjects { get; private set; }

    private void OnDestroy()
    {
        PreviousScene = gameObject.scene.name;
    }

    private void Start()
    {
        Debug.Log("PreviousScene: " + SceneHistory.PreviousScene);

        if (gameObject.scene.name.Equals(OnSpotARScene))
        {
            PreviousSceneToAr = PreviousScene;
        }

        Debug.Log("PreviousSceneToAr: " + SceneHistory.PreviousSceneToAr);

    }

    public void BackButtonAR()
    {
        if (SceneHistory.PreviousScene == null)
        {
            SceneManager.LoadScene("Home");

        }
        else if (gameObject.scene.name.Equals(NearbyARScene))
        {
            SceneManager.LoadScene(SceneHistory.PreviousSceneToAr);

        }
        else
        {
            SceneManager.LoadScene(SceneHistory.PreviousScene);
        }
    }

}
