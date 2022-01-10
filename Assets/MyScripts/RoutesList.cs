using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;
using Mapbox.Unity.Map;
using System.IO;
using UnityEngine.SceneManagement;

public class RoutesList : MonoBehaviour
{
    public const string OFFICIAL_ROUTE = "oficial";

    [SerializeField]
    Camera MainCamera;
    [SerializeField]
    GameObject RouteListCanvas;
    [SerializeField]
    GameObject RouteListItem;
    [SerializeField]
    GameObject EmptyRouteListItem;
    //JSONNode RouteList;
    //JSONNode UnofficialRoutesList;
    //List<JSONNode> AllRoutes;
    List<Route> AllRoutesObj;
    //string routesListFilePath;
    string codesListFilePath;
    JSONNode CodesList;
    List<string> codes;

    private void Awake()
    {
        codesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";

       

    }

    public void Start()
    {
        AllRoutesObj = new List<Route>();

        if (MainDataHolder.OfficialRoutes != null)
        {
            AllRoutesObj.AddRange(MainDataHolder.OfficialRoutes);
            print("MainDataHolder.OfficialRoutes count: " + MainDataHolder.OfficialRoutes.Count);

        }


        if (MainDataHolder.MyUnofficialRoutes != null)
        {
           AllRoutesObj.AddRange(MainDataHolder.MyUnofficialRoutes);
           print("MainDataHolder.UnofficialRoutes count: " + MainDataHolder.MyUnofficialRoutes.Count);
        }

        ///OfficialRoutes = GetRouteListFromJson(RouteList);
       //UnofficialRoutes = GetRouteListFromJson(UnofficialRoutesList);
        //OfficialRoutes.AddRange(UnofficialRoutes);
        //AllRoutes = OfficialRoutes;
        print("AllRoutesObj count: " + AllRoutesObj.Count);
        RouteDataHolder.AllRoutes = AllRoutesObj;
        SetUpRoutesList();
    }


    public void AddImportedRoute(Route r)
    {
        AllRoutesObj.Add(r);

    }

    public void SetUpRoutesList()
    {
        //GameObject RouteListItem = this.transform.GetChild(0).gameObject;

        if (AllRoutesObj.Count == 0)
        {
            RouteListItem.SetActive(false);
            EmptyRouteListItem.SetActive(true);

        }
        else
        {
            EmptyRouteListItem.SetActive(false);
            RouteListItem.SetActive(true);
            for (int i = 0; i < AllRoutesObj.Count; i++)
            {
              
                GameObject g = Instantiate(RouteListItem, this.transform);
                //string routeName = RouteList["routes"][i]["name"];
                string routeName = AllRoutesObj[i].Name;

                g.transform.Find("RouteName").GetComponent<Text>().text = routeName;

                string categoriesString = "";
                for (int j = 0; j < AllRoutesObj[i].RouteCategory.Count; j++)
                {
                    if (j == 0)
                    {
                        categoriesString += AllRoutesObj[i].RouteCategory[j];

                    }
                    else
                    {
                        categoriesString += " " + AllRoutesObj[i].RouteCategory[j];

                    }
                }
                g.transform.Find("Categories").GetComponent<Text>().text = categoriesString;

                string routeId = AllRoutesObj[i].Id;
                string routeCode = AllRoutesObj[i].Code;
                Route currRoute = AllRoutesObj[i];
                g.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    RouteDataHolder.currentRouteId = routeId;
                    SceneManager.LoadScene("RoutePageScene");

                    //ItemClicked(currRoute);
                });
                //g.GetComponent<Button>().AddEventListener(routeId, ItemClicked);
            }
           
            RouteListItem.SetActive(false);
        }
        //Destroy(RouteListItem);
    }

    //private void ItemClicked(Route currRoute)
    //{
    //    Route route = new Route();
    //    route.Id = currRoute.Id;
    //    print(route.Id);
    //    route.Name = currRoute.Name;
    //    route.Code = currRoute.Code;
    //    route.Description = currRoute.Description;

    //    route.RouteCategory = new List<string>();

    //    for (int j = 0; j < currRoute.RouteCategory.Count; j++)
    //    {
    //        route.RouteCategory.Add(currRoute.RouteCategory[j]);
    //    }

    //    route.Pois = new List<Poi>();
    //    for (int k = 0; k < currRoute.Pois.Count; k++)
    //    {
    //        Poi p = new Poi();
    //        p.Id = currRoute.Pois[k].Id;
    //        p.Latitude = currRoute.Pois[k].Latitude;
    //        p.Longitude = currRoute.Pois[k].Longitude;
    //        route.Pois.Add(p);
    //    }

    //    print("routeName " + route.Name);

    //    RouteDataHolder.currentRoute = route;
    //    print("currentRoute routeName " + RouteDataHolder.currentRoute.Name);

    //    //RouteDataHolder.jsonRouteList = RouteList;
    //    //RouteDataHolder.jsonUnofficialRoutesList = UnofficialRoutesList;


    //    SceneManager.LoadScene("RoutePageScene");


    //}


}
