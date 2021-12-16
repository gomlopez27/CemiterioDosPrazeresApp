using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoutePageAreaCOPY : MonoBehaviour
{
    public const string OFFICIAL_ROUTE = "oficial";

    [SerializeField]
    Camera MainCamera;
    [SerializeField]
    GameObject RouteGeneralCanvas;
    [SerializeField]
    GameObject LocationBasedGame;
    [SerializeField]
    GameObject Directions;
    [SerializeField]
    GameObject PoisRouteListArea;
    [SerializeField]
    Button StartRouteBtn;
    [SerializeField]
    Text RouteDurationTxt;
    [SerializeField]
    Text RouteDistanceTxt;
    [SerializeField]
    Text RouteTitle;
    //[SerializeField]
    //JazInformations JazInfo;
    //[SerializeField]
    //RouteDataHolder dataHolder;


    GameObject Map;
    GraphicRaycaster raycaster;
    GameObject panel1;
    DirectionsFactory _dirFactory;
    Transform[] poisOnMapTranforms;
    bool checkRouteInfo;
    //GameObject Player;
    //bool hasSetUpDirectionsRoute;
    //Text routeDistance;
    //Text routeDuration;
    // GameObject RouteDirectionsView;
    List<GameObject> routePoisInMap;
    Route currentRoute;

    void Awake()
    {

        //initialCameraTransform = MainCamera.transform;
        //InitialMapLocation = Map.GetComponent<AbstractMap>().CenterLatitudeLongitude;
        //initialMapZoom = Map.GetComponent<AbstractMap>().InitialZoom;
    }

    public void Start()
    {

        Map = LocationBasedGame.transform.Find("Map").gameObject;
       // Player = LocationBasedGame.transform.Find("Player").gameObject;
        _dirFactory = Directions.GetComponent<DirectionsFactory>();
        raycaster = RouteGeneralCanvas.GetComponent<GraphicRaycaster>();
        panel1 = this.transform.Find("Panel1").gameObject;
        //routePoisInMap = new List<GameObject>();

        if (RouteDataHolder.currentRoute.Code.Equals(OFFICIAL_ROUTE))
        {
            routePoisInMap = Map.GetComponent<SpawnRoutePOI>().RouteClicked(RouteDataHolder.currentRoute.Id, RouteDataHolder.jsonRouteList);
   
        }
        else
        {
            routePoisInMap = Map.GetComponent<SpawnRoutePOI>().RouteClicked(RouteDataHolder.currentRoute.Id, RouteDataHolder.jsonUnofficialRoutesList);
        }

        //RouteDataHolder.currentRoutePoisInMap = routePoisInMap;
        currentRoute = RouteDataHolder.currentRoute;
        RouteTitle.text = currentRoute.Name;

        SetUpDirectionsRoute();
        StartRouteBtn.onClick.AddListener(StartRoute);
    }

    // Update is called once per frame
    void Update()
    {
        if (checkRouteInfo)
        {
            if (_dirFactory.enabled && _dirFactory.GetDirectionsResponse() != null)
                /*&& _dirFactory.GetRouteDistance() != 0 && _dirFactory.GetRouteDuration() != 0*/
            {
                SetRouteInfo(_dirFactory);
            }
        }

        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                GameObject go = result.gameObject;
                //print("Clicked on: " + go.name);
                if (go.name.Equals("Panel1") /*|| go.name.Equals("Panel2")*/)
                {
                    go.SetActive(false);
                    Map.GetComponent<MapPanning>().enabled = true;
                    Map.GetComponent<MapPanning>().SetCameraInitialPosition();

                }
                if (go.name.Equals("RouteInfoPanel"))
                {
                    panel1.SetActive(true);
                    Map.GetComponent<MapPanning>().enabled = false;

                }
                //if (go.name.Equals("PoiInfoPanel"))
                //{
                //    panel2.SetActive(true);
                //    Map.GetComponent<PanZoom>().enabled = false;

                //}
            }
        }
 
    }

    public void SetUpDirectionsRoute()
    {
        //spawnedPois = routePois;
        GameObject poiItem = PoisRouteListArea.transform.GetChild(0).gameObject;

        Directions.SetActive(true);       
        _dirFactory.enabled = true;
        int size = routePoisInMap.Count;

        //print("routePoisInMap POIS: " + size);
        //print("currentRoute POIS: " + currentRoute.pois.Count);

        poisOnMapTranforms = new Transform[size];

        for (int i = 0; i < currentRoute.Pois.Count; i++)
        {
            GameObject g = Instantiate(poiItem, PoisRouteListArea.transform);
            poisOnMapTranforms[i] = routePoisInMap[i].transform;

            int poiOrder = i + 1;
            g.transform.Find("POIOrderNr").GetComponent<Text>().text = "Ponto " + poiOrder.ToString();
            Text poiName = g.transform.Find("POIName").GetComponent<Text>();
            string jazId = currentRoute.Pois[i].Id;
            Poi jaz = MainDataHolder.GetPoi(jazId);
            if(jaz.Personalities.Count > 1)
            {
                poiName.text = jaz.JazType + " " + jazId + ": Múltiplas Personalidades";

            }
            else
            {
                poiName.text = jaz.JazType + " " + jazId + ": " + jaz.Personalities[0].Name;

            }


        }
        poiItem.SetActive(false);
        _dirFactory.SetWaypoints(poisOnMapTranforms);
        _dirFactory.Start();

        checkRouteInfo = true;
        //hasSetUpDirectionsRoute = true;
    }

    void SetRouteInfo(DirectionsFactory dirFactory)
    {
        //routeDistance = this.transform.Find("RouteInfoPanel/ScrollArea/Content/DistanceGO/DistanceTxt-Value").gameObject.GetComponent<Text>();
        //routeDuration = this.transform.Find("RouteInfoPanel/ScrollArea/Content/DurationGO/DurationTxt-Value").gameObject.GetComponent<Text>();

        double distance = Math.Round(dirFactory.GetRouteDistance(), MidpointRounding.AwayFromZero);
        double duration = Math.Round(dirFactory.GetRouteDuration(), MidpointRounding.AwayFromZero);

        //print("distance: " + distance);
        //print("duration: " + duration);
        if (distance != 0 && duration != 0)
        {

            TimeSpan time = TimeSpan.FromSeconds(duration);
            double durationInMin = Math.Round(time.TotalMinutes, MidpointRounding.AwayFromZero);

            if (durationInMin < 60)
            {
                RouteDurationTxt.text = durationInMin.ToString() + " minutos";
            }
            else
            {
                double durationInHours = Math.Round(time.TotalHours, MidpointRounding.AwayFromZero);
                RouteDurationTxt.text = durationInHours.ToString() + " horas";


            }

            if (distance < 1000)
            {
                RouteDistanceTxt.text = "(" + distance.ToString() + " m)";

            }
            else
            {
                double distanceInKm = Math.Round(distance * 0.001, 2, MidpointRounding.AwayFromZero);
                RouteDistanceTxt.text = distanceInKm.ToString() + " km";

            }

            checkRouteInfo = false;

        }

        //print("Round ToEven 2.5: " + Math.Round(2.5, MidpointRounding.ToEven)); //arrendoda para baixo
        //print("Round AwayFromZero 2.565: " + Math.Round(2.565, 2, MidpointRounding.AwayFromZero));//arrendoda para cima
    }

    void StartRoute()
    {
        SceneManager.LoadScene("RouteDirectionsScene");

        //RouteGeneralCanvas.SetActive(false);
        //RouteDirectionsCanvas.SetActive(true);
        ////LocationBasedGame.SetActive(false);

        //Directions.SetActive(false);
        //_dirFactory.enabled = false;
        //_dirFactory.CancelDirections();

  
        //MainCamera.transform.position = new Vector3(0f, MainCamera.transform.position.y, MainCamera.transform.position.z);
        //Player.GetComponent<RotateWithLocationProvider>().enabled = true;
        //MainCamera.transform.SetParent(Player.transform);

        //RouteDirectionsView = RouteDirectionsCanvas.transform.Find("SafeArea/RouteDirectionsArea").gameObject;
        //RouteDirectionsView.GetComponent<RouteDirectionsArea>().enabled = true;
        ////RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetCurrentRouteIdName(currentRouteId, currentRouteName);
        ////RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetListRoutePois(spawnedPois);
        //RouteDirectionsView.GetComponent<RouteDirectionsArea>().Start();
        ////RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetUpRouteDirMode(spawnedPois);


    }


    public void BackToRoutesList()
    {
        SceneManager.LoadScene("RouteListScene");
        //for(int i = 0; i < routePois.Count; i++)
        //{
        //    routePois[i].transform.parent.gameObject.Destroy();
        //}

        //GameObject poiItem = PoisRouteListArea.transform.GetChild(0).gameObject;
        //poiItem.SetActive(true);
        //for(int i = 1; i < PoisRouteListArea.transform.childCount; i++)
        //{
        //    Destroy(PoisRouteListArea.transform.GetChild(i).gameObject);
        //}

        ////LocationBasedGame.SetActive(false);
        //SpawnRoutePoiScript.enabled = false;
        //Map.GetComponent<InitializeMapWithLocationProvider>().enabled = false;

        //Map.SetActive(false);
        //Directions.SetActive(false);

        //_dirFactory.enabled = false;
        //_dirFactory.CancelDirections();
        ////PlayerController.GetComponent<UserInRoute>().enabled = false;
    }
}
