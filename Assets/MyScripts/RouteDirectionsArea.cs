using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Utils;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteDirectionsArea : MonoBehaviour
{

    [SerializeField]
    public Camera _referenceCamera;
    [SerializeField]
    GameObject RouteGeneralCanvas;
    [SerializeField]
    GameObject RouteDirectionsCanvas;
    [SerializeField]
    GameObject LocationBasedGame;
    [SerializeField]
    GameObject Directions;
    [SerializeField]
    Text currentJazDuration;
    [SerializeField]
    Text currentJazDistance;
    [SerializeField]
    Text currentJaz;
    [SerializeField]
    Text currentJazPersonalities;
    [SerializeField]
    Image currentJazPhoto;
    [SerializeField]
    Text instructionsText;
    [SerializeField]
    Button ConfirmEndRouteBtn;
    //[SerializeField]
    //RouteDataHolder dataHolder;
    [SerializeField]
    GameObject ReachedPoiInfoPanel;
    [SerializeField]
    GameObject PoiInfoPanel;
    [SerializeField]
    GameObject EndRoutePanel;
    [SerializeField]
    GameObject ConfirmEndRoutePanel;
    [SerializeField]
    Button ARButton;
    [SerializeField]
    GameObject ToastMsg;    
    [SerializeField]
    JazInformations JazInfo;

    string currentRouteId;
    string currentRouteName;
    List<GameObject> routePois;
    List<GameObject> updatedRoutePois;
    GameObject MapGO;
    AbstractMap _map;
    //Vector3 initialCameraPos;
    //Button NextPoiBtn;
    //Button PrevPoiBtn;
    Text PoiNrTxt;
    GameObject PlayerController;
    GameObject Player;
    int indexNext = 0;
    JSONNode RouteList;
    JSONNode PoiInRoute;
    DirectionsFactory _dirFactory;
    AbstractLocationProvider _locationProvider = null;
    Vector2d latlong;
    bool hasInitMap;
    bool hasInitRouteDir;
    bool hasShowedToast;
    //Dictionary<string, bool> visitedPois;
    GameObject RouteGeneralView;

    private void Awake()
    {

        TextAsset json = Resources.Load<TextAsset>("RoutesList");
        RouteList = JSON.Parse(json.ToString());
    }

       
    public void Start()
    {
        if (null == _locationProvider)
        {
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }

        //NextPoiBtn = this.transform.Find("PoiInfoPanel/ScrollArea/Content/ButtonGO/NextButton").gameObject.GetComponent<Button>();
        //PrevPoiBtn = this.transform.Find("PoiInfoPanel/ScrollArea/Content/ButtonGO/PrevButton").gameObject.GetComponent<Button>();
        //NextPoiBtn.onClick.AddListener(HandleNextPoi);
        //PrevPoiBtn.onClick.AddListener(HandlePrevPoi);
        //EndRouteBtn = this.transform.Find("PoiInfoPanel/ScrollArea/Content/ButtonGO/EndButton").gameObject.GetComponent<Button>();
        //initialCameraPos = _referenceCamera.transform.position;
        MapGO = LocationBasedGame.transform.Find("Map").gameObject;
        PlayerController = LocationBasedGame.transform.Find("PlayerCotroller").gameObject;
        Player = LocationBasedGame.transform.Find("Player").gameObject;
        _dirFactory = Directions.GetComponent<DirectionsFactory>();
        _map = MapGO.GetComponent<AbstractMap>();
        PoiNrTxt = this.transform.Find("PoiInfoPanel/ScrollArea/Content/ButtonGO/PoiNr").gameObject.GetComponent<Text>();
        ConfirmEndRouteBtn.onClick.AddListener(EndRoute);
        EndRoutePanel.transform.Find("TerminarButton").gameObject.GetComponent<Button>().onClick.AddListener(EndRoute);
        RouteGeneralView = RouteGeneralCanvas.transform.Find("SafeArea/GeneralInfoArea").gameObject;


        //currentRouteId = dataHolder.CurrentRouteId;
        //currentRouteName = dataHolder.CurrentRouteName;
        //routePois = dataHolder.CurrentRoutePois;
        currentRouteId = RouteDataHolder.routeId;
        currentRouteName = RouteDataHolder.routeName;
        routePois = RouteDataHolder.routePois;

        hasInitMap = false;

        //visitedPois = new Dictionary<string, bool>();
        updatedRoutePois = new List<GameObject>();
        updatedRoutePois.AddRange(routePois);
        string firstPoiId = "";

        foreach (GameObject go in routePois)
        {
            POIMapSpecifications poi = go.transform.parent.gameObject.GetComponent<POIMapSpecifications>();
            //visitedPois.Add(poi.id, false);
            if(routePois.IndexOf(go)== 0)
            {
                firstPoiId = poi.GetId();
                break;
            }

        }

        SetPoiInfo(JazInfo.GetJaz(firstPoiId), 1);

        //StartCoroutine(UserReachedPoi());

    }

    // Update is called once per frame
    void Update()
    {
        Location currLoc = _locationProvider.CurrentLocation;
        if (currLoc.IsLocationServiceEnabled && !currLoc.LatitudeLongitude.Equals(Vector2d.zero))
        {
            latlong = currLoc.LatitudeLongitude;
           UserReachedPoi();

            if (!hasInitMap)
            {
                _map.SetCenterLatitudeLongitude(latlong);
                _map.UpdateMap(_map.CenterLatitudeLongitude, 18f);
                hasInitMap = true;
            }

        }

        
        if (hasInitMap && !hasInitRouteDir)
        {

            UpdateRouteDirections(Player.transform, routePois);
            hasInitRouteDir = true;
        }

        if (_dirFactory.enabled && _dirFactory.GetDirectionsResponse() != null)
        {
            SetInfoThatUpdates(_dirFactory);
        }

        RouteDataHolder.CurrentCanvasGeneralInfo = RouteGeneralCanvas;
        RouteDataHolder.CurrentCanvasDirectionsMode = RouteDirectionsCanvas;
        RouteDataHolder.LocationBasedGame = LocationBasedGame;
        RouteDataHolder.Directions = Directions;

    }




    //Raio de 10 m
    void UserReachedPoi()
    {
        // yield return new WaitForSeconds(1f);
        for (int i = updatedRoutePois.Count - 1; i >= 0; i--)
        {
            GameObject go = updatedRoutePois[i];
            POIMapSpecifications poi = go.transform.parent.gameObject.GetComponent<POIMapSpecifications>();
            double poiLat = double.Parse(poi.latitude, System.Globalization.CultureInfo.InvariantCulture);
            double poiLong = double.Parse(poi.longitude, System.Globalization.CultureInfo.InvariantCulture);

            if (GetDistance(latlong.x, latlong.y, poiLat, poiLong) < 0.01 )
            {
                if (i == 0) //to force all POIs being visited by the right order
                {
                    updatedRoutePois.RemoveAll(item => item == go);
                    print("item removed " + poi.name);
                    poi.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                    //visitedPois[poi.id] = true;
                    int currPoiIndex = routePois.IndexOf(go);
                    SetUpReachedPoiInfoPanel(currPoiIndex + 1, routePois.Count, poi.id);

                    if (updatedRoutePois.Count > 0)
                    {
                        int nextPoiIndex = routePois.IndexOf(updatedRoutePois[i]);
                        string nextPoiId = updatedRoutePois[i].transform.parent.gameObject.GetComponent<POIMapSpecifications>().id;
                        SetPoiInfo(JazInfo.GetJaz(nextPoiId), nextPoiIndex + 1);
                        UpdateRouteDirections(Player.transform, updatedRoutePois);
                        PoiInfoPanel.SetActive(false);
                        EndRoutePanel.SetActive(false);

                        ReachedPoiInfoPanel.transform.Find("ClosePanelBtn").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            PoiInfoPanel.SetActive(true);

                        });
                    }

                    if (updatedRoutePois.Count == 0)
                    {
                        PoiInfoPanel.SetActive(false);
                        EndRoutePanel.SetActive(false);

                        ReachedPoiInfoPanel.transform.Find("ClosePanelBtn").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            EndRoutePanel.SetActive(true);

                        });
                    }

                }
                else
                {
                    print("Index: " + i);
                    if (!hasShowedToast) { 
                        StartCoroutine(ShowToastMsg());
                    }
                    
                }
            }

        }
    }

    IEnumerator ShowToastMsg()
    {
        ToastMsg.SetActive(true);
        yield return new WaitForSeconds(5);
        ToastMsg.SetActive(false);
        hasShowedToast = true;

    }
    void SetUpReachedPoiInfoPanel(int poiNr, int totalPois, string poiId)
    {
        ARButton.GetComponent<Animator>().SetTrigger("StartAnim");
        int poisLeft = totalPois - poiNr;
        string auxText1 = "Parabéns! Chegou ao " + poiNr + "º ponto do percurso, faltam " + (totalPois - poiNr) + ".";
        string auxText2 = "Parabéns! Chegou ao " + poiNr + "º e último ponto do percurso.";
        ReachedPoiInfoPanel.SetActive(true);
        ReachedPoiInfoPanel.transform.Find("Title").GetComponent<Text>().text = poiNr + " - Jazigo " + poiId.ToString();
        
        if(poisLeft > 0)
        {
            ReachedPoiInfoPanel.transform.Find("PoisLeftText").GetComponent<Text>().text = auxText1;
        }
        else
        {
            ReachedPoiInfoPanel.transform.Find("PoisLeftText").GetComponent<Text>().text = auxText2;

        }

    }

    //public void SetCurrentRouteIdName(string id, string name)
    //{
    //    this.currentRouteId = id;
    //    this.currentRouteName = name;
    //}

    //public void SetListRoutePois(List<GameObject> list)
    //{
    //    routePois = list;
    //}

    public void UpdateRouteDirections(Transform playerPos, List<GameObject> pois)
    {

        //MapGO.GetComponent<SpawnRoutePOI>().enabled = true;
        //List<GameObject> routePois = MapGO.GetComponent<SpawnRoutePOI>().RouteClicked(currentRouteId);
        Directions.SetActive(true);
        _dirFactory.enabled = true;
        int size = pois.Count;
        Transform [] poisOnMapTranforms = new Transform[size+1];

        print("POIS left: " + size); 
        poisOnMapTranforms[0] = playerPos;

        for (int i = 0; i < size; i++)
        {
            poisOnMapTranforms[i+1] = pois[i].transform;

        }
        _dirFactory.SetWaypoints(poisOnMapTranforms);
        _dirFactory.Start();
    }

    public void EndRoute()
    {
        ConfirmEndRoutePanel.SetActive(false);
        PlayerController.GetComponent<UserInRoute>().enabled = false;
        Player.GetComponent<RotateWithLocationProvider>().enabled = false;
        Player.transform.rotation = new Quaternion(0,0,0,0);
        RouteGeneralCanvas.SetActive(true);
        RouteDirectionsCanvas.SetActive(false);
        LocationBasedGame.SetActive(true);
        MapGO.SetActive(true);
        _map.enabled = true;
        _map.gameObject.GetComponent<SpawnRoutePOI>().enabled = true;
        
        _map.SetCenterLatitudeLongitude(RouteDataHolder.mapLocation);
        _map.UpdateMap(_map.CenterLatitudeLongitude, RouteDataHolder.mapZoom);

        RouteGeneralView.GetComponent<RoutePageArea>().enabled = true;
        RouteGeneralView.GetComponent<RoutePageArea>().Start();

        //RouteGeneralView.GetComponent<RoutePageArea>().SetUpDirectionsRoute(routePois);
        //RouteGeneralView.GetComponent<RoutePageArea>().SetCurrentRouteInfo(currentRouteId, currentRouteName);

    }

    public void CenterMapInPoi(Vector2d latlong, string jazId)
    {
        string auxName = "Poi-" + jazId;
        GameObject poiAux = GameObject.Find(auxName);

        _referenceCamera.transform.position = new Vector3(poiAux.transform.position.x,
                                                     _referenceCamera.transform.position.y,
                                                     poiAux.transform.position.z);
        _map.SetCenterLatitudeLongitude(latlong);
        _map.UpdateMap(_map.CenterLatitudeLongitude, 17.5f);
        print("LATLONG: " + string.Format("{0}", _map.CenterLatitudeLongitude));

    }

    //JSONNode GetPoiInformation(string currentRoute, string id)
    //{
    //    for (int i = 0; i < RouteList["routes"].Count; i++)
    //    {
    //        if (RouteList["routes"][i]["id"].Equals(currentRoute))
    //        {
    //            int POIsCount = RouteList["routes"][i]["pois"].Count;

    //            for (int j = 0; j < POIsCount; j++)
    //            {
    //                string poiId = RouteList["routes"][i]["pois"][j]["id"];
    //                if (poiId.Equals(id))
    //                {
    //                    PoiInRoute = RouteList["routes"][i]["pois"][j];
    //                }

    //            }
    //        }
    //    }
    //    return PoiInRoute;
    //}

    void SetPoiInfo(JSONNode poi, int poiNr)
    {
        //Text jazId = this.transform.Find("PoiInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
        //Text personalities = this.transform.Find("PoiInfoPanel/ScrollArea/Content/PersonalityGO/PersonalityTxt-Value").gameObject.GetComponent<Text>();

        currentJaz.text = poiNr + " - Jazigo " + poi["ID"];

        //print("poi[id] " + poi["id"]);
        string personalitiesString = "";
        for (int j = 0; j < poi["personalidades"].Count; j++)
        {
            if (j == 0)
            {
                personalitiesString += poi["personalidades"][j]["nome"];

            }
            else
            {
                personalitiesString += ", " + poi["personalidades"][j]["nome"];

            }
        }

        currentJazPersonalities.text = personalitiesString;

        Davinci.get().load(poi["jazImage"]).into(currentJazPhoto).start();

        
        //currentJazDuration
    }

    //public void SetInfoThatUpdates(double distanceToNextPoi, double durationToNextPoi, string instruction)
    public void SetInfoThatUpdates(DirectionsFactory dirFactory)
    {
        string instruction = dirFactory.GetDirectionsResponse().Routes[0].Legs[0].Steps[0].Maneuver.Instruction;
        instructionsText.text = instruction;

        //double distanceToNextPoi = dirFactory.GetDirectionsResponse().Routes[0].Legs[0].Distance;
        //double durationToNextPoi = dirFactory.GetDirectionsResponse().Routes[0].Legs[0].Duration;
        double distanceToNextPoi =  Math.Round(dirFactory.GetDirectionsResponse().Routes[0].Legs[0].Distance, MidpointRounding.AwayFromZero);
        double durationToNextPoi = Math.Round(dirFactory.GetDirectionsResponse().Routes[0].Legs[0].Duration, MidpointRounding.AwayFromZero);


        TimeSpan time = TimeSpan.FromSeconds(durationToNextPoi);
        double durationInMin = Math.Round(time.TotalMinutes, MidpointRounding.AwayFromZero);

        if(durationToNextPoi < 60)
        {
            currentJazDuration.text = " < 1 minuto";

        }
        else if (durationInMin < 60)
        {
            currentJazDuration.text = durationInMin.ToString() + " minutos";
        }
        else
        {
            double durationInHours = Math.Round(time.TotalHours, MidpointRounding.AwayFromZero);
            currentJazDuration.text = durationInHours.ToString() + " horas";
        }

        if (distanceToNextPoi < 1000)
        {
            currentJazDistance.text = "(" + distanceToNextPoi.ToString() + " m)";

        }
        else
        {
            double distanceInKm = Math.Round(distanceToNextPoi * 0.001, 2, MidpointRounding.AwayFromZero);
            currentJazDistance.text = distanceInKm.ToString() + " km";

        }
    }


    

    //calculate the distance between two coordinates, in KM
    double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    double ToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }

}
