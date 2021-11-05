using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoutePageArea : MonoBehaviour
{
    [SerializeField]
    Camera MainCamera;
    [SerializeField]
    GameObject RouteGeneralCanvas;
    [SerializeField]
    GameObject RouteDirectionsCanvas;
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
    [SerializeField]
    JazInformations JazInfo;
    //[SerializeField]
    //RouteDataHolder dataHolder;

    string currentRouteId;
    string currentRouteName;
    //GameObject RouteInfoPanel;
    GameObject Map;
    AbstractMap AbsMap;
    SpawnRoutePOI SpawnRoutePoiScript;

    GraphicRaycaster raycaster;
    GameObject panel1;
    GameObject Player;
    //GameObject PlayerController;
    DirectionsFactory _dirFactory;
    //POIMapSpecifications[] poisOnMap;
    Transform[] poisOnMapTranforms;
    bool checkRouteInfo;
    bool hasSetUpDirectionsRoute;
    //Text routeDistance;
    //Text routeDuration;
    GameObject RouteDirectionsView;
    List<GameObject> routePois;


    void Awake()
    {

        //initialCameraTransform = MainCamera.transform;
        //InitialMapLocation = Map.GetComponent<AbstractMap>().CenterLatitudeLongitude;
        //initialMapZoom = Map.GetComponent<AbstractMap>().InitialZoom;
    }

    public void Start()
    {

        Map = LocationBasedGame.transform.Find("Map").gameObject;
        Player = LocationBasedGame.transform.Find("Player").gameObject;
        _dirFactory = Directions.GetComponent<DirectionsFactory>();
        SpawnRoutePoiScript = Map.GetComponent<SpawnRoutePOI>();
        raycaster = RouteGeneralCanvas.GetComponent<GraphicRaycaster>();
        panel1 = this.transform.Find("Panel1").gameObject;
        //poisOnMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        hasSetUpDirectionsRoute = false;

        StartRouteBtn.onClick.AddListener(StartRoute);

        //currentRouteId = dataHolder.CurrentRouteId;
        //currentRouteName = dataHolder.CurrentRouteName;
        //routePois = dataHolder.CurrentRoutePois;
        currentRouteId = RouteDataHolder.routeId;
        currentRouteName = RouteDataHolder.routeName;
        routePois = RouteDataHolder.routePois;

        RouteTitle.text = currentRouteName;
        SetCameraInitialPos();
        SetUpDirectionsRoute();
    }

    // Update is called once per frame
    void Update()
    {
        //if (poisOnMap.Length <= 0)
        //{
        //    poisOnMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        //    print("poisOnMap.Length: " + poisOnMap.Length);
        //}
        //else
        //{
        //    if (!hasSetUpDirectionsRoute)
        //    {
        //        SetUpDirectionsRoute();
        //    }

        //}

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

    //public void SetCurrentRouteInfo(string id, string name)
    //{
    //    this.currentRouteId = id;
    //    this.currentRouteName = name;
    //}



    public void SetCameraInitialPos()
    {
        Vector3 initialPos = new Vector3(0.28f, 140, -15);
        MainCamera.transform.position = initialPos;
        MainCamera.transform.SetParent(GameObject.Find("Routes").transform);
    }

    public void SetUpDirectionsRoute()
    {
        //spawnedPois = routePois;
        GameObject poiItem = PoisRouteListArea.transform.GetChild(0).gameObject;

        Directions.SetActive(true);       
        _dirFactory.enabled = true;
        int size = routePois.Count;

        print("ROUTE POIS: " + size);

        poisOnMapTranforms = new Transform[size];

        for (int i = 0; i < size; i++)
        {
            GameObject g = Instantiate(poiItem, PoisRouteListArea.transform);
            poisOnMapTranforms[i] = routePois[i].transform;
            POIMapSpecifications specifications = routePois[i].transform.parent.gameObject.GetComponent<POIMapSpecifications>();

            int poiOrder = i + 1;
            g.transform.Find("POIOrderNr").GetComponent<Text>().text = "Ponto " + poiOrder.ToString();
            Text poiName = g.transform.Find("POIName").GetComponent<Text>();
            JSONNode jaz = JazInfo.GetJaz(specifications.id);
            if(jaz["personalidades"].Count > 1)
            {
                poiName.text = jaz["tipoJaz"] + " " + specifications.id + ": Múltiplas Personalidades";

            }
            else
            {
                poiName.text = jaz["tipoJaz"] + " " + specifications.id + ": " + jaz["personalidades"][0]["nome"];

            }


        }
        poiItem.SetActive(false);
        _dirFactory.SetWaypoints(poisOnMapTranforms);
        _dirFactory.Start();

        checkRouteInfo = true;
        hasSetUpDirectionsRoute = true;
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
        RouteGeneralCanvas.SetActive(false);
        RouteDirectionsCanvas.SetActive(true);
        //LocationBasedGame.SetActive(false);

        Directions.SetActive(false);
        _dirFactory.enabled = false;
        _dirFactory.CancelDirections();

  
        MainCamera.transform.position = new Vector3(0f, MainCamera.transform.position.y, MainCamera.transform.position.z);
        Player.GetComponent<RotateWithLocationProvider>().enabled = true;
        MainCamera.transform.SetParent(Player.transform);

        RouteDirectionsView = RouteDirectionsCanvas.transform.Find("SafeArea/RouteDirectionsArea").gameObject;
        RouteDirectionsView.GetComponent<RouteDirectionsArea>().enabled = true;
        //RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetCurrentRouteIdName(currentRouteId, currentRouteName);
        //RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetListRoutePois(spawnedPois);
        RouteDirectionsView.GetComponent<RouteDirectionsArea>().Start();
        //RouteDirectionsView.GetComponent<RouteDirectionsArea>().SetUpRouteDirMode(spawnedPois);

        


        //LBGRouteView.SetActive(true);
        //GameObject MapGO = LBGRouteView.transform.Find("Map").gameObject;
        //MapGO.SetActive(true);
        //MapGO.GetComponent<AbstractMap>().enabled = true;
        //MapGO.GetComponent<AbstractMap>().InitializeOnStart = true;

        //MapGO.GetComponent<AbstractMap>().Initialize(Conversions.StringToLatLon(38.713655 + "," + -9.171207), 18);
        //MapGO.GetComponent<SpawnRoutePOI>().enabled = true;
        //List<GameObject> routePois = MapGO.GetComponent<SpawnRoutePOI>().RouteClicked(currentRouteId);


    }


    public void BackToRoutesList()
    {

        for(int i = 0; i < routePois.Count; i++)
        {
            routePois[i].transform.parent.gameObject.Destroy();
        }
      
        GameObject poiItem = PoisRouteListArea.transform.GetChild(0).gameObject;
        poiItem.SetActive(true);
        for(int i = 1; i < PoisRouteListArea.transform.childCount; i++)
        {
            Destroy(PoisRouteListArea.transform.GetChild(i).gameObject);
        }

        //LocationBasedGame.SetActive(false);
        SpawnRoutePoiScript.enabled = false;
        Map.GetComponent<InitializeMapWithLocationProvider>().enabled = false;

        Map.SetActive(false);
        Directions.SetActive(false);

        _dirFactory.enabled = false;
        _dirFactory.CancelDirections();
        //PlayerController.GetComponent<UserInRoute>().enabled = false;
    }
}
