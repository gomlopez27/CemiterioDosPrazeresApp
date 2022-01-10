using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class SpawnRoutePOI : MonoBehaviour
{
    public const string OFFICIAL_ROUTE = "oficial";


    [SerializeField]
    AbstractMap _map;
    [SerializeField]
    float _spawnScale = 100f;
    [SerializeField]
    GameObject _markerPrefab;
    //[SerializeField]
    //JazInformations JazInfo;

    Vector2d[] _locations;
    List<GameObject> _spawnedObjects;
    string[] currentRoutePois;

    //Vector3 initialCameraPos;
    //Vector2d initialLocation;
    //Transform[] _spawnedObjectsTranforms;
    //JSONNode RouteList;
    //JSONNode PoiInRoute;
    //JSONNode UnofficialRoutesList;
    //GameObject RouteArea;
    //GameObject Directions;
    //DirectionsFactory dirFactory;
    //Text numberOfPoi;
    //Text routeDistance;
    //Text routeDuration;
    //bool checkRouteInfo;
    //GameObject RouteInfoPanel;
    //GameObject PoiInfoPanel;
    //string currentRoute;
    //int indexNext = 0;
    //Button StartRouteBtn;
    //Button EndRouteBtn;
    //Button NextPoiBtn;
    //Button PrevPoiBtn;
    //Text PoiNrTxt;
    //GameObject PlayerController;
    //List<GameObject> spawnedPois;
    //List<JSONNode> AllRoutes;

    private void Awake()
    {
        //string allRoutesListFilePath = Application.persistentDataPath + "/AllRoutesList.json";
        //if (System.IO.File.Exists(allRoutesListFilePath))
        //{
        //    string json = File.ReadAllText(allRoutesListFilePath);
        //    RouteList = JSON.Parse(json.ToString());
        //}
        //else
        //{ }
            //TextAsset json = Resources.Load<TextAsset>("RoutesList");
            //RouteList = JSON.Parse(json.ToString());

        
        //string routesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
        //if (System.IO.File.Exists(routesListFilePath))
        //{
        //    string jsonUnofficialRoutesList = File.ReadAllText(routesListFilePath);
        //    UnofficialRoutesList = JSON.Parse(jsonUnofficialRoutesList.ToString());
        //    print("UnofficialRoutesList " + UnofficialRoutesList["routes"].Count);

        //}

    }

    private void Start()
    {
        //spawnedPois = new List<GameObject>();
        //AllRoutes = new List<JSONNode>();
    }

    private void Update()
    {
        int count = _spawnedObjects.Count;

        for (int i = 0; i < count; i++)
        {
            var spawnedObject = _spawnedObjects[i];

            var location = _locations[i];
            spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, 1,
                spawnedObject.transform.localPosition.z);

            //_spawnedObjectsTranforms[i] = spawnedObject.transform;
            spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
        }
         

        //if (checkRouteInfo)
        //{
        //    if (dirFactory.GetDirectionsResponse() != null)
        //    {
        //        SetRouteInfo(dirFactory);
        //        checkRouteInfo = false;
        //    }
        //}

    }


    public List<GameObject> RouteClicked(string routeId, JSONNode rList)
    {
        _spawnedObjects = new List<GameObject>();
        //initialLocation = _map.CenterLatitudeLongitude;

        for (int i = 0; i < rList["routes"].Count; i++)
        {
            if (rList["routes"][i]["id"].Equals(routeId))
            {
                //currentRoute = routeId;
                int POIsCount = rList["routes"][i]["pois"].Count;

                //numberOfPoi = RouteArea.transform.Find("RouteInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
                //numberOfPoi.text = POIsCount.ToString();

                currentRoutePois = new string[POIsCount];
                _locations = new Vector2d[POIsCount];

                for (int j = 0; j < POIsCount; j++)
                {
                    string poiId = rList["routes"][i]["pois"][j]["id"];
                    //string lat = rList["routes"][i]["pois"][j]["latitude"];
                    //string lng = rList["routes"][i]["pois"][j]["longitude"];
                    Poi poi = MainDataHolder.GetPoi(poiId);
                    string lat = poi.Latitude.ToString();
                    string lng = poi.Longitude.ToString();
                    string location = lat + "," + lng;
                    print(poiId + " " + location);

                    _locations[j] = Conversions.StringToLatLon(location);
                    currentRoutePois[j] = poiId;
                    GameObject POILocation = new GameObject("Poi-" + poiId);
                    POIMapSpecifications specifications = POILocation.AddComponent<POIMapSpecifications>();
                    specifications.SetVariables(poiId, lat, lng);


                    TextMesh poiNumber = _markerPrefab.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
                    poiNumber.text = (j + 1).ToString();
                    GameObject thisPOI = Instantiate(_markerPrefab, POILocation.transform);

                    thisPOI.transform.localPosition = _map.GeoToWorldPosition(_locations[j], true);
                    thisPOI.transform.localPosition = new Vector3(thisPOI.transform.localPosition.x, 1,
                        thisPOI.transform.localPosition.z);

                    thisPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                    _spawnedObjects.Add(thisPOI);
                    //aux.Add(thisPOI);

                }
                break;
            }
        }

        //spawnedPois = _spawnedObjects;
        return _spawnedObjects;
    }

    //public List<GameObject> RouteClicked(string routeId)
    //{
    //    Route route = RouteDataHolder.GetRoute(RouteDataHolder.currentRouteId);
    //    _spawnedObjects = new List<GameObject>();

    //    List<string> auxPoisIdList = new List<string>(route.PoisId);

    //    int POIsCount = auxPoisIdList.Count;

    //    //numberOfPoi = RouteArea.transform.Find("RouteInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
    //    //numberOfPoi.text = POIsCount.ToString();

    //    currentRoutePois = new string[POIsCount];
    //    _locations = new Vector2d[POIsCount];

    //    for (int i = 0; i < POIsCount; i++)
    //    {

    //        string poiId = auxPoisIdList[i];
    //        //string lat = rList["routes"][i]["pois"][j]["latitude"];
    //        //string lng = rList["routes"][i]["pois"][j]["longitude"];
    //        Poi poi = MainDataHolder.GetPoi(poiId);
    //        string lat = poi.Latitude.ToString();
    //        string lng = poi.Longitude.ToString();
    //        string location = lat + "," + lng;
    //        print(poiId + " " + location);

    //        _locations[i] = Conversions.StringToLatLon(location);
    //        currentRoutePois[i] = poiId;
    //        GameObject POILocation = new GameObject("Poi-" + poiId);
    //        POIMapSpecifications specifications = POILocation.AddComponent<POIMapSpecifications>();
    //        specifications.SetVariables(poiId, lat, lng);


    //        TextMesh poiNumber = _markerPrefab.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
    //        poiNumber.text = (i + 1).ToString();
    //        GameObject thisPOI = Instantiate(_markerPrefab, POILocation.transform);

    //        thisPOI.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
    //        thisPOI.transform.localPosition = new Vector3(thisPOI.transform.localPosition.x, 1,
    //            thisPOI.transform.localPosition.z);

    //        thisPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
    //        _spawnedObjects.Add(thisPOI);
    //        //aux.Add(thisPOI);

    //    }

    //    return _spawnedObjects;
    //}

    public List<GameObject> RouteClicked(string routeId)
    {
        Route route = RouteDataHolder.GetRoute(RouteDataHolder.currentRouteId);
        _spawnedObjects = new List<GameObject>();

        int POIsCount = route.Pois.Count;

        //numberOfPoi = RouteArea.transform.Find("RouteInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
        //numberOfPoi.text = POIsCount.ToString();

        currentRoutePois = new string[POIsCount];
        _locations = new Vector2d[POIsCount];

        for (int i = 0; i < POIsCount; i++)
        {
            string poiId = route.Pois[i].Id;
            //string lat = rList["routes"][i]["pois"][j]["latitude"];
            //string lng = rList["routes"][i]["pois"][j]["longitude"];
            Poi poi = MainDataHolder.GetPoi(poiId);
            string lat = poi.Latitude.ToString();
            string lng = poi.Longitude.ToString();
            string location = lat + "," + lng;
            print(poiId + " " + location);

            _locations[i] = Conversions.StringToLatLon(location);
            currentRoutePois[i] = poiId;
            GameObject POILocation = new GameObject("Poi-" + poiId);
            POIMapSpecifications specifications = POILocation.AddComponent<POIMapSpecifications>();
            specifications.SetVariables(poiId, lat, lng);


            TextMesh poiNumber = _markerPrefab.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
            poiNumber.text = (i + 1).ToString();
            GameObject thisPOI = Instantiate(_markerPrefab, POILocation.transform);

            thisPOI.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            thisPOI.transform.localPosition = new Vector3(thisPOI.transform.localPosition.x, 1,
                thisPOI.transform.localPosition.z);

            thisPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            _spawnedObjects.Add(thisPOI);
            //aux.Add(thisPOI);

        }

        return _spawnedObjects;
    }

    //public List<GameObject> GetSpawnedPois(JSONNode rList, string routeId)
    //{
    //    List<GameObject> aux = new List<GameObject>();
    //    print("ROUTE POI COUNT " + rList["routes"].Count);
    //    for (int i = 0; i < rList["routes"].Count; i++)
    //    {
    //        if (rList["routes"][i]["id"].Equals(routeId))
    //        {
    //            currentRoute = routeId;
    //            int POIsCount = rList["routes"][i]["pois"].Count;

    //            //numberOfPoi = RouteArea.transform.Find("RouteInfoPanel/ScrollArea/Content/PoiGO/POINrTxt-Value").gameObject.GetComponent<Text>();
    //            //numberOfPoi.text = POIsCount.ToString();

    //            currentRoutePois = new string[POIsCount];
    //            _locations = new Vector2d[POIsCount];

    //            for (int j = 0; j < POIsCount; j++)
    //            {
    //                string poiId = rList["routes"][i]["pois"][j]["id"];
    //                //string lat = rList["routes"][i]["pois"][j]["latitude"];
    //                //string lng = rList["routes"][i]["pois"][j]["longitude"];
    //                Poi poi = MainDataHolder.GetPoi(poiId);
    //                string lat = poi.Latitude.ToString();
    //                string lng = poi.Longitude.ToString();
    //                string location = lat + "," + lng;
    //                print(poiId + " " + location);

    //                _locations[j] = Conversions.StringToLatLon(location);
    //                currentRoutePois[j] = poiId;
    //                GameObject POILocation = new GameObject("Poi-" + poiId);
    //                POIMapSpecifications specifications = POILocation.AddComponent<POIMapSpecifications>();
    //                specifications.SetVariables(poiId, lat, lng);


    //                TextMesh poiNumber = _markerPrefab.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
    //                poiNumber.text = (j + 1).ToString();
    //                GameObject thisPOI = Instantiate(_markerPrefab, POILocation.transform);

    //                thisPOI.transform.localPosition = _map.GeoToWorldPosition(_locations[j], true);
    //                thisPOI.transform.localPosition = new Vector3(thisPOI.transform.localPosition.x, 1,
    //                    thisPOI.transform.localPosition.z);

    //                thisPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
    //                //_spawnedObjects.Add(thisPOI);
    //                aux.Add(thisPOI);

    //            }
    //            break;
    //        }
    //    }
    //    return aux;
    //}

}
