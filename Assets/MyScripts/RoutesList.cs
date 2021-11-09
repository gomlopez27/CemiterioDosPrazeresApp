using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;
using Mapbox.Unity.Map;
using System.IO;



public class RoutesList : MonoBehaviour
{
    public const string OFFICIAL_ROUTE = "oficial";

    [SerializeField]
    Camera MainCamera;
    [SerializeField]
    GameObject RouteListCanvas;
    [SerializeField]
    GameObject RouteGeneralCanvas;
    //[SerializeField]
    //RouteDataHolder dataHolder;
    [SerializeField]
    GameObject Map;
    [SerializeField]
    GameObject RouteListItem;
    [SerializeField]
    GameObject EmptyRouteListItem;
    AbstractMap absMap;
    JSONNode RouteList;
    JSONNode UnofficialRoutesList;
    GameObject RouteGeneralView;
    List<JSONNode> AllRoutes;
    string routesListFilePath;
    string codesListFilePath;
    JSONNode CodesList;
    List<string> codes;
    private void Awake()
    {

        routesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
        codesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";

        if (System.IO.File.Exists(routesListFilePath))
        {
            string jsonUnofficialRoutesList = File.ReadAllText(routesListFilePath);
            UnofficialRoutesList = JSON.Parse(jsonUnofficialRoutesList.ToString());
            print("UnofficialRoutesList " + UnofficialRoutesList["routes"].Count);

        }
        if (System.IO.File.Exists(codesListFilePath))
        {
            string jsonCodesRoutesList = File.ReadAllText(codesListFilePath);
            CodesList = JSON.Parse(jsonCodesRoutesList.ToString());
            codes = new List<string>();
            for (int i = 0; i < CodesList.Count; i++)
            {
                codes.Add(CodesList[i]);
            }
        }
        TextAsset jsonRouteList = Resources.Load<TextAsset>("RoutesList");
        
        if(jsonRouteList != null){
            RouteList = JSON.Parse(jsonRouteList.ToString());
        }

    }

    public void Start()
    {
        AllRoutes = new List<JSONNode>();
 
        RouteGeneralView = RouteGeneralCanvas.transform.Find("SafeArea/GeneralInfoArea").gameObject;
        absMap = Map.GetComponent<AbstractMap>();

        if(RouteList != null)
        {
            for (int i = 0; i < RouteList["routes"].Count; i++)
            {
                JSONNode r = RouteList["routes"][i];
                AllRoutes.Add(r);
            }
        }


        if (UnofficialRoutesList != null && codes != null)
        {
            for (int i = 0; i < UnofficialRoutesList["routes"].Count; i++)
            {
                JSONNode r = UnofficialRoutesList["routes"][i];            
                if (codes.Contains(r["code"]))
                {
                    AllRoutes.Add(r);
                }
            }
        }

        ///OfficialRoutes = GetRouteListFromJson(RouteList);
       //UnofficialRoutes = GetRouteListFromJson(UnofficialRoutesList);
        //OfficialRoutes.AddRange(UnofficialRoutes);
        //AllRoutes = OfficialRoutes;
        print("count: " + AllRoutes.Count);

        SetUpRoutesList();
    }


    public void AddImportedRoute(JSONNode r)
    {
        AllRoutes.Add(r);

    }

    //List<Route> GetRouteListFromJson(JSONNode RouteList)
    //{
    //    List<Route> aux = new List<Route>();

    //    for (int i = 0; i < RouteList["routes"].Count; i++)
    //    {
    //        Route route = new Route();
    //        route.id = RouteList["routes"][i]["id"];
    //        route.name = RouteList["routes"][i]["name"];
    //        route.code = RouteList["routes"][i]["code"];
    //        route.description = RouteList["routes"][i]["description"];

    //        route.routeCategory = new List<string>();

    //        for (int j = 0; j < RouteList["routes"][i]["routeCategory"].Count; j++)
    //        {
    //            route.routeCategory.Add(RouteList["routes"][i]["routeCategory"][j]);
    //        }

    //        route.pois = new List<Poi>();
    //        for (int k = 0; k < RouteList["routes"][i]["pois"].Count; k++)
    //        {
    //            Poi p = new Poi();
    //            p.ID = RouteList["routes"][i]["pois"][k]["id"];

    //            route.pois.Add(p);
    //        }
    //        aux.Add(route);
    //    }
    //    print("Routes list count " + aux.Count);
    //    return aux;
    //}

    public void SetUpRoutesList()
    {
        //GameObject RouteListItem = this.transform.GetChild(0).gameObject;

        if (AllRoutes.Count == 0)
        {
            RouteListItem.SetActive(false);
            EmptyRouteListItem.SetActive(true);

        }
        else
        {
            EmptyRouteListItem.SetActive(false);
            RouteListItem.SetActive(true);
            for (int i = 0; i < AllRoutes.Count; i++)
            {
              
                GameObject g = Instantiate(RouteListItem, this.transform);
                //string routeName = RouteList["routes"][i]["name"];
                string routeName = AllRoutes[i]["name"];

                g.transform.Find("RouteName").GetComponent<Text>().text = routeName;

                string categoriesString = "";
                for (int j = 0; j < AllRoutes[i]["routeCategory"].Count; j++)
                {
                    if (j == 0)
                    {
                        categoriesString += AllRoutes[i]["routeCategory"][j];

                    }
                    else
                    {
                        categoriesString += " " + AllRoutes[i]["routeCategory"][j];

                    }
                }
                g.transform.Find("Categories").GetComponent<Text>().text = categoriesString;

                string routeId = AllRoutes[i]["id"];
                string routeCode = AllRoutes[i]["code"];

                g.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    ItemClicked(routeId, routeName, routeCode);
                });
                //g.GetComponent<Button>().AddEventListener(routeId, ItemClicked);
            }
           
            RouteListItem.SetActive(false);
        }
        //Destroy(RouteListItem);
    }

    private void ItemClicked(string routeId, string routeName, string routeCode)
    {
        print(routeId);
        RouteListCanvas.SetActive(false);
        RouteDataHolder.CurrentCanvasRouteList = RouteListCanvas;
        RouteGeneralCanvas.SetActive(true);
        //LocationBasedGame.SetActive(true);
        Map.SetActive(true);
        //map.enabled = true;
        Map.GetComponent<InitializeMapWithLocationProvider>().enabled = true;
        Map.GetComponent<SpawnRoutePOI>().enabled = true;
        List<GameObject> routePois = new List<GameObject>();

        if (routeCode.Equals(OFFICIAL_ROUTE))
        {
             routePois = absMap.GetComponent<SpawnRoutePOI>().RouteClicked(routeId, RouteList);
        }
        else
        {
           routePois = absMap.GetComponent<SpawnRoutePOI>().RouteClicked(routeId, UnofficialRoutesList);
        }


        //dataHolder.InitialCameraTransform = MainCamera.transform;

        //dataHolder.InitialMapLocation = Map.GetComponent<InitializeMapWithLocationProvider>().GetInitialMapLocation();
        //dataHolder.InitialMapZoom = Map.GetComponent<InitializeMapWithLocationProvider>().GetInitialMapZoom();
        //dataHolder.CurrentRouteId = routeId;
        //dataHolder.CurrentRouteName = routeName;
        //dataHolder.CurrentRoutePois = routePois;

        RouteDataHolder.mapLocation = Map.GetComponent<InitializeMapWithLocationProvider>().GetInitialMapLocation();
        RouteDataHolder.mapZoom = Map.GetComponent<InitializeMapWithLocationProvider>().GetInitialMapZoom();
        RouteDataHolder.routeId = routeId;
        RouteDataHolder.routeName = routeName;
        RouteDataHolder.routePois = routePois;

        RouteGeneralView.GetComponent<RoutePageArea>().enabled = true;
        RouteGeneralView.GetComponent<RoutePageArea>().Start();
        //RouteGeneralView.GetComponent<RoutePageArea>().SetUpDirectionsRoute(/*routePois*/);

        //RouteGeneralView.GetComponent<RoutePageArea>().SetCurrentRouteInfo(routeId, routeName);



        //RouteGeneralView.GetComponent<RoutePageArea>().SetCameraInitialPos();
        //RouteGeneralView.GetComponent<RoutePageArea>().SetUpDirectionsRoute();
        //map.GetComponent<SpawnRoutePOI>().SetUpDirectionsRoute();



    }


}
