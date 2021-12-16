using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;
using Mapbox.Unity.Map;
using System.IO;
using UnityEngine.SceneManagement;

public class RoutesListCOPY : MonoBehaviour
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
    JSONNode RouteList;
    JSONNode UnofficialRoutesList;
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
                JSONNode currRoute = AllRoutes[i];
                g.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    ItemClicked(currRoute);
                });
                //g.GetComponent<Button>().AddEventListener(routeId, ItemClicked);
            }
           
            RouteListItem.SetActive(false);
        }
        //Destroy(RouteListItem);
    }

    private void ItemClicked(JSONNode routeJson)
    {
        Route route = new Route();
        route.Id = routeJson["id"];
        print(route.Id);
        route.Name = routeJson["name"];
        route.Code = routeJson["code"];
        route.Description = routeJson["description"];

        route.RouteCategory = new List<string>();

        for (int j = 0; j < routeJson["routeCategory"].Count; j++)
        {
            route.RouteCategory.Add(routeJson["routeCategory"][j]);
        }

        route.Pois = new List<Poi>();
        for (int k = 0; k < routeJson["pois"].Count; k++)
        {
            Poi p = new Poi();
            p.Id = routeJson["pois"][k]["id"];
            p.Latitude = routeJson["pois"][k]["latitude"];
            p.Longitude = routeJson["pois"][k]["longitude"];
            route.Pois.Add(p);
        }

        print("routeName " + route.Name);

        RouteDataHolder.currentRoute = route;
        print("currentRoute routeName " + RouteDataHolder.currentRoute.Name);

        RouteDataHolder.jsonRouteList = RouteList;
        RouteDataHolder.jsonUnofficialRoutesList = UnofficialRoutesList;


        SceneManager.LoadScene("RoutePageScene");


    }


}
