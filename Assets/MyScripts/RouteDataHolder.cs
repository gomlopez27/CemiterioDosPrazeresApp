using Mapbox.Utils;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public static class RouteDataHolder 
{
    public static Transform cameraTransform;
    public static Vector2d mapLocation;
    public static int mapZoom;
    public static string routeId;
    public static string routeName;
    public static List<GameObject> routePois;

    public static GameObject CurrentCanvasRouteList;
    public static GameObject CurrentCanvasGeneralInfo;
    public static GameObject CurrentCanvasDirectionsMode;
    public static GameObject CurrentCanvasImportRoutes;
    public static GameObject LocationBasedGame;
    public static GameObject Directions;

    public static Route currentRoute;
    public static JSONNode jsonRouteList;
    public static JSONNode jsonUnofficialRoutesList;

    //State during route diretions
    public static List<GameObject> updatedRoutePoisInMap;
    public static List<string> savedPoisReached;

    public static List<Route> AllRoutes;
    public static string currentRouteCode;
    public static List<Poi> currentRoutePois;

    public static Route GetRoute(string routeCode)
    {
        Route r = AllRoutes.Find(x => x.Code == routeCode);
        return r;
    }

    //public static Route GetRoute(string routeId)
    //{
    //    Route r = AllRoutes.Find(x => x.Id == routeId);
    //    return r;
    //}


    //public Transform InitialCameraTransform
    //{
    //    get { return cameraTransform; }
    //    set { cameraTransform = value; }
    //}
    //public Vector2d InitialMapLocation {
    //    get { return mapLocation; }
    //    set { mapLocation = value; }
    //}
    //public int InitialMapZoom
    //{
    //    get { return mapZoom; }
    //    set { mapZoom = value; }
    //}
    //public string CurrentRouteId
    //{
    //    get { return routeId; }
    //    set { routeId = value; }
    //}
    //public string CurrentRouteName
    //{
    //    get { return routeName; }
    //    set { routeName = value; }
    //}
    //public List<GameObject> CurrentRoutePois
    //{
    //    get { return routePois; }
    //    set { routePois = value; }
    //}



}
