using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RouteDirectionsArea : MonoBehaviour
{
    public const string OFFICIAL_ROUTE = "oficial";

    [SerializeField]
    public Camera _referenceCamera;
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



    private GameObject MapGO;
    private AbstractMap AbsMap;
    private GameObject Player;
    private DirectionsFactory _dirFactory;
    private AbstractLocationProvider _locationProvider = null;
    private Vector2d latlong;
    private bool hasInitMap;
    private bool hasInitRouteDir;
    private bool hasShowedToast;
    private Route currentRoute;
    private List<GameObject> routePoisInMap;
    //  private List<GameObject> updatedRoutePois;
    private List<string> routePoisReached;

    private void Awake()
    {
        MapGO = LocationBasedGame.transform.Find("Map").gameObject;
        Player = LocationBasedGame.transform.Find("Player").gameObject;
        _dirFactory = Directions.GetComponent<DirectionsFactory>();
        AbsMap = MapGO.GetComponent<AbstractMap>();
    }
       
    public void Start()
    {
        _referenceCamera.transform.position = new Vector3(0f, _referenceCamera.transform.position.y, _referenceCamera.transform.position.z);
        _referenceCamera.transform.SetParent(Player.transform);
        hasInitMap = false;

        if (null == _locationProvider)
        {
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }
        routePoisInMap = MapGO.GetComponent<SpawnRoutePOI>().RouteClicked(RouteDataHolder.currentRouteId);
    

        //Estado Inicial
        currentRoute = RouteDataHolder.GetRoute(RouteDataHolder.currentRouteId);


        print("PreviousScene "+ SceneHistory.PreviousScene);

        //routePoisReached = new List<string>();

        //Quando se volta 'as direcoes vindo da RA
        if ((SceneHistory.PreviousScene.Equals(SceneHistory.OnSpotARScene) || SceneHistory.PreviousScene.Equals(SceneHistory.NearbyARScene)))
        {
            if(RouteDataHolder.savedPoisReached != null)
            {
                routePoisReached = new List<string>();
                routePoisReached.AddRange(RouteDataHolder.savedPoisReached);
                //routePoisReached = RouteDataHolder.savedPoisReached;

                print("routePoisReached: " + routePoisReached.Count);

                List<GameObject> RoutePoisLeft = new List<GameObject>();

                foreach (GameObject go in routePoisInMap)
                {
                    POIMapSpecifications poi = go.transform.parent.gameObject.GetComponent<POIMapSpecifications>();

                    if (routePoisReached.Contains(poi.id))
                    {
                        poi.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                    else
                    {
                        RoutePoisLeft.Add(go);
                    }
                }

                print("RoutePoisLeft: " + RoutePoisLeft.Count);
                RouteDataHolder.updatedRoutePoisInMap = RoutePoisLeft;
                //updatedRoutePoisSaved = RouteDataHolder.updatedRoutePoisInMap;
                //print("RouteDataHolder.updatedRoutePoisInMap IN START after AR " + RouteDataHolder.updatedRoutePoisInMap.Count);
                //print("updatedRoutePoisInMap " + RouteDataHolder.updatedRoutePoisInMap[0].transform.parent.name);
                if (routePoisReached.Count != routePoisInMap.Count)
                {
                    int nextPoiOriginalIndex = routePoisInMap.IndexOf(RoutePoisLeft[0]);
                    string PoiId = currentRoute.Pois[nextPoiOriginalIndex].Id;
                    SetPoiInfo(MainDataHolder.GetPoi(PoiId), nextPoiOriginalIndex + 1);
                }
                else
                {
                    PoiInfoPanel.SetActive(false);
                    EndRoutePanel.SetActive(true);
                }
            }
            else
            {
                print("NO routePoisReached: " );

                List<GameObject> initialPoisInMap = new List<GameObject>();
                initialPoisInMap.AddRange(routePoisInMap);
                RouteDataHolder.updatedRoutePoisInMap = initialPoisInMap;
                routePoisReached = new List<string>();
                string firstPoiId = currentRoute.Pois[0].Id;
                SetPoiInfo(MainDataHolder.GetPoi(firstPoiId), 1);
            }
        }
        else//Quando se vem da pagina da Rota, ainda nao foi a cena rda RA
        {
            //Estado inicial - se nao veio de uma cena de AR, teve de vir da cena da pagina da Rota
            List<GameObject> initialPoisInMap = new List<GameObject>();
            initialPoisInMap.AddRange(routePoisInMap);
            RouteDataHolder.updatedRoutePoisInMap = initialPoisInMap;
            routePoisReached = new List<string>();
            string firstPoiId = currentRoute.Pois[0].Id;
            SetPoiInfo(MainDataHolder.GetPoi(firstPoiId), 1);
        }

        ConfirmEndRouteBtn.onClick.AddListener(EndRoute);
        EndRoutePanel.transform.Find("TerminarButton").gameObject.GetComponent<Button>().onClick.AddListener(EndRoute);
    }

    // Update is called once per frame
    void Update()
    {
        Location currLoc = _locationProvider.CurrentLocation;
        if (currLoc.IsLocationServiceEnabled && !currLoc.LatitudeLongitude.Equals(Vector2d.zero))
        {
            latlong = currLoc.LatitudeLongitude;

            if (routePoisReached.Count != routePoisInMap.Count)
            {
                UserReachedPoi();

            }
            if (!hasInitMap)
            {
                AbsMap.SetCenterLatitudeLongitude(latlong);
                AbsMap.UpdateMap(AbsMap.CenterLatitudeLongitude, 17.6f);
                hasInitMap = true;
            }

        }

        if (hasInitMap && !hasInitRouteDir) 
        {
            if(SceneHistory.PreviousScene.Equals(SceneHistory.OnSpotARScene) || SceneHistory.PreviousScene.Equals(SceneHistory.NearbyARScene))
            {
                if(RouteDataHolder.updatedRoutePoisInMap.Count > 0)
                {
                    UpdateRouteDirections(Player.transform, RouteDataHolder.updatedRoutePoisInMap);

                }
            }
            else
            {
                UpdateRouteDirections(Player.transform, routePoisInMap);
            }

        }

        if (_dirFactory.enabled && _dirFactory.GetDirectionsResponse() != null)
        {
            SetInfoThatUpdates(_dirFactory);
        }

    }

    void StateAfterAr() { 
    }
    //Raio de 10 m
    void UserReachedPoi()
    {
        List<GameObject> updatedPoisInMap = RouteDataHolder.updatedRoutePoisInMap;
        //List<GameObject> updatedPoisInMap = new List<GameObject>();
        //updatedPoisInMap.AddRange(RouteDataHolder.updatedRoutePoisInMap);
        //List<GameObject> poisAfterRemoved = new List<GameObject>();

        print("RouteDataHolder.updatedRoutePoisInMap: " + RouteDataHolder.updatedRoutePoisInMap.Count);
       // print("updatedRoutePoisInMap first elem: " + updatedPoisInMap[0].transform.parent.name);
        // yield return new WaitForSeconds(1f);
        //for (int i = updatedPoisInMap.Count - 1; i >= 0; i--)
        for (int i = 0; i < updatedPoisInMap.Count; i++)
        {
            GameObject go = updatedPoisInMap[i];
            print("GO NAME: " + go.name);
            POIMapSpecifications poi = go.transform.parent.gameObject.GetComponent<POIMapSpecifications>();

            double poiLat = double.Parse(poi.latitude, System.Globalization.CultureInfo.InvariantCulture);
            double poiLong = double.Parse(poi.longitude, System.Globalization.CultureInfo.InvariantCulture);

            if (GetDistance(latlong.x, latlong.y, poiLat, poiLong) < 0.01)
            {
                if (i == 0) //to force all POIs being visited by the right order
                {
                    //updatedPoisInMap.RemoveAll(item => item == go);
                    int currPoiIndex = routePoisInMap.IndexOf(go);
                    poi.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                    routePoisReached.Add(poi.id);
                    RouteDataHolder.savedPoisReached = routePoisReached;
                    //visitedPois[poi.id] = true;
                    //print(string.Format("currPoiIndex in all pois list: {0}, routePoisInMap.Count: {1}, id: {2}", currPoiIndex, routePoisInMap.Count, poi.id));
                    SetUpReachedPoiInfoPanel(currPoiIndex + 1, routePoisInMap.Count, poi.id);
                    updatedPoisInMap.Remove(go);
                    //poisAfterRemoved.AddRange(updatedPoisInMap);
                    //RouteDataHolder.updatedRoutePoisInMap = poisAfterRemoved;
                    print("item removed " + poi.name);
                    //print("pois left " + poisAfterRemoved[i].transform.parent.name);

                    if (updatedPoisInMap.Count > 0)
                    {
                        int nextPoiIndex = this.routePoisInMap.IndexOf(updatedPoisInMap[i]);
                        string nextPoiId = updatedPoisInMap[i].transform.parent.gameObject.GetComponent<POIMapSpecifications>().id;
                        SetPoiInfo(MainDataHolder.GetPoi(nextPoiId), nextPoiIndex + 1);
                        UpdateRouteDirections(Player.transform, updatedPoisInMap);
                        PoiInfoPanel.SetActive(false);
                        EndRoutePanel.SetActive(false);

                        ReachedPoiInfoPanel.transform.Find("ClosePanelBtn").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            PoiInfoPanel.SetActive(true);

                        });
                    }

                    if (updatedPoisInMap.Count == 0)
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
                    if (!hasShowedToast)
                    {
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
        hasInitRouteDir = true;
    }

    public void LoadArScene()
    {
        SceneManager.LoadScene("OnSpotARScene");

    }
   
    public void EndRoute()
    {
        RouteDataHolder.updatedRoutePoisInMap = null;
        RouteDataHolder.savedPoisReached = null;
        SceneManager.LoadScene("RoutePageScene");
      
    }

    public void CenterMapInPoi(Vector2d latlong, string jazId)
    {
        string auxName = "Poi-" + jazId;
        GameObject poiAux = GameObject.Find(auxName);

        _referenceCamera.transform.position = new Vector3(poiAux.transform.position.x,
                                                     _referenceCamera.transform.position.y,
                                                     poiAux.transform.position.z);
        AbsMap.SetCenterLatitudeLongitude(latlong);
        AbsMap.UpdateMap(AbsMap.CenterLatitudeLongitude, 17.5f);
        print("LATLONG: " + string.Format("{0}", AbsMap.CenterLatitudeLongitude));

    }
    void SetPoiInfo(Poi poi, int poiNr)
    {
        //Text jazId = this.transform.Find("PoiInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
        //Text personalities = this.transform.Find("PoiInfoPanel/ScrollArea/Content/PersonalityGO/PersonalityTxt-Value").gameObject.GetComponent<Text>();

        //currentJaz.text = poiNr + " - Jazigo " + poi["ID"];
        currentJaz.text = poiNr + " - Jazigo " + poi.Id;

        //print("poi[id] " + poi["id"]);
        string personalitiesString = "";
        for (int j = 0; j < poi.Personalities.Count; j++)
        {
            if (j == 0)
            {
                personalitiesString += poi.Personalities[j].Name;

            }
            else
            {
                personalitiesString += ", " + poi.Personalities[j].Name;

            }
        }
        currentJazPersonalities.text = personalitiesString;
        Davinci.get().load(poi.JazImage).into(currentJazPhoto).start();
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
