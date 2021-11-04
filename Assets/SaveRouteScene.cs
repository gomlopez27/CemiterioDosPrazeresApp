using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveRouteScene : MonoBehaviour
{

    [SerializeField] GameObject CurrentCanvasRouteList;
    [SerializeField] GameObject CurrentCanvasGeneralInfo;
    [SerializeField] GameObject CurrentCanvasDirectionsMode;
    [SerializeField] GameObject CurrentCanvasImportRoutes;
    [SerializeField] GameObject LocationBasedGame;
    [SerializeField] GameObject Directions;
    private void Start()
    {
        if (SceneHistory.PreviousScene != null && SceneHistory.PreviousScene.Equals(SceneHistory.OnSpotARScene) )
        {
            GameObject Routes = GameObject.Find("Routes");
            //Destroy(Routes);
            GameObject go = new GameObject("CurrentRoute");
            CurrentCanvasRouteList = RouteDataHolder.CurrentCanvasRouteList;
            CurrentCanvasGeneralInfo = RouteDataHolder.CurrentCanvasGeneralInfo;
            CurrentCanvasDirectionsMode = RouteDataHolder.CurrentCanvasDirectionsMode;
            CurrentCanvasImportRoutes = RouteDataHolder.CurrentCanvasImportRoutes;
            LocationBasedGame = RouteDataHolder.LocationBasedGame;
            Directions = RouteDataHolder.Directions;
          
        }

    }
}

