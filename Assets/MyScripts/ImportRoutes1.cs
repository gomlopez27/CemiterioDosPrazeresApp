using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImportRoutes1 : MonoBehaviour
{
    [SerializeField]
    GameObject ImportRouteGO;
    [SerializeField]
    GameObject ConfirmPanel;
    [SerializeField]
    InputField inputCode;
    [SerializeField]
    Button confirmImportBtn;
    [SerializeField]
    GameObject LoadingPanel;
    [SerializeField]
    GameObject RouteListGO;
    [SerializeField]
    GameObject ToastMsgWrongCode;

    private string code;
    private string unofficialRoutesListFilePath;
    private JSONNode RouteNode;
    private JSONNode UnofficialRoutes;
    private List<Route> Routes;

    //void Start()
    //{
    //    RouteListGO.GetComponent<RoutesListCOPY>().enabled = false;
    //    unofficialRoutesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
    //    print(unofficialRoutesListFilePath);
    //    ConfirmPanel.SetActive(true);
    //    LoadingPanel.SetActive(false);

    //    print("From static class UnofficialRoutes.Count: " + MainDataHolder.MyUnofficialRoutes.Count);
    //    print("From static class UnofficialRoutes 1ª rota: " + MainDataHolder.MyUnofficialRoutes[0].Name);
    //}

    //void Update()
    //{
    //    //RouteDataHolder.CurrentCanvasImportRoutes = ImportRouteGO;

    //}

    //public void SetCode(string s)
    //{
    //    code = s;
    //}

    //public void Clear()
    //{
    //    inputCode.text = "";
    //}

    //public void Import()
    //{
    //    StartCoroutine(GetImportedRoute(code));
    //}

    //void ConvertJsonToList(JSONNode jsonRoutesList)
    //{
    //    print("jsonRoutesList.Count " + jsonRoutesList["routes"].Count);
    //    for (int i = 0; i < jsonRoutesList["routes"].Count; i++)
    //    {
    //        Route route = new Route();
    //        route.Id = jsonRoutesList["routes"][i]["id"];
    //        print(jsonRoutesList["routes"][i]["id"]);
    //        route.Name = jsonRoutesList["routes"][i]["name"];
    //        route.Code = jsonRoutesList["routes"][i]["code"];
    //        route.Description = jsonRoutesList["routes"][i]["description"];

    //        route.RouteCategory = new List<string>();

    //        for (int j = 0; j < jsonRoutesList["routes"][i]["routeCategory"].Count; j++)
    //        {
    //            route.RouteCategory.Add(jsonRoutesList["routes"][i]["routeCategory"][j]);
    //        }

    //        //route.Pois = new List<Poi>();
    //        //for (int k = 0; k < jsonRoutesList["routes"][i]["pois"].Count; k++)
    //        //{
    //        //    Poi p = new Poi();
    //        //    p.Id = jsonRoutesList["routes"][i]["pois"][k]["id"];
    //        //    route.Pois.Add(p);
    //        //}

    //        Routes.Add(route);
    //    }
    //}

    //void AddFirstUnnoficialRoute(JSONNode r)
    //{
    //    print("AddFirstUnnoficialRoute");
    //    Route route = new Route();
    //    route.Id = r["id"];
    //    print(r["id"]);
    //    route.Name = r["name"];
    //    route.Code = r["code"];
    //    route.Description = r["description"];

    //    route.RouteCategory = new List<string>();

    //    for (int j = 0; j < r["routeCategory"].Count; j++)
    //    {
    //        route.RouteCategory.Add(r["routeCategory"][j]);
    //    }

    //    //route.Pois = new List<Poi>();
    //    //for (int k = 0; k < r["pois"].Count; k++)
    //    //{
    //    //    Poi p = new Poi();
    //    //    p.Id = r["pois"][k]["id"];


    //    //    route.Pois.Add(p);
    //    //}
    //    Routes.Add(route);
    //    //return route;
    //}

    //IEnumerator GetImportedRoute(string code)
    //{
    //    string codeFilePath = Application.persistentDataPath + "/" + code + ".json";
    //    Routes = new List<Route>();
    //    if (System.IO.File.Exists(codeFilePath))
    //    {
    //        ConfirmPanel.SetActive(false);
    //        LoadingPanel.SetActive(true);
    //        print(codeFilePath);
    //        string json = File.ReadAllText(codeFilePath);
    //        RouteNode = JSON.Parse(json.ToString());

    //        if (System.IO.File.Exists(unofficialRoutesListFilePath))
    //        {
    //            string jsonUnoRoutes = File.ReadAllText(unofficialRoutesListFilePath);
    //            UnofficialRoutes = JSON.Parse(jsonUnoRoutes.ToString());
    //            ConvertJsonToList(UnofficialRoutes);
    //            AddFirstUnnoficialRoute(RouteNode);
    //            //print("UnofficialRoutes count " + UnofficialRoutes.Count);
    //            //UnofficialRoutes.Add(RouteNode);
    //            //print("UnofficialRoutes count after add " + UnofficialRoutes.Count +  " " + RouteNode["name"]);
    //            //ConvertJsonToList(UnofficialRoutes);
    //        }
    //        else
    //        {
    //            AddFirstUnnoficialRoute(RouteNode);
    //        }

    //        RouteListGO.GetComponent<RoutesListCOPY>().AddImportedRoute(RouteNode);
    //        print("List<Route> Routes: " + Routes.Count);
    //        foreach (Route r in Routes)
    //        {
    //            print(r.Name);
    //        }
    //        RoutesCollection rc = new RoutesCollection();
    //        rc.RoutesCol = Routes;
    //        string jsonToWrite = rc.Serialize().ToString(3);
    //        System.IO.File.WriteAllText(unofficialRoutesListFilePath, jsonToWrite);
    //        this.GetComponent<SerializableDataElements>().SaveRouteCodeToJson(RouteNode["code"]);
    //        yield return new WaitForSeconds(3);
    //        this.GetComponent<LoadScenes>().LoadRouteListScene();
    //        //LoadingPanel.SetActive(false);
    //        //ImportRouteGO.SetActive(false);
    //        //RouteListGO.GetComponent<RoutesList>().enabled = true;

    //        //for (int i = 2; i < RouteListGO.transform.childCount; i++)
    //        //{
    //        //    Destroy(RouteListGO.transform.GetChild(i).gameObject);
    //        //}
    //        //RouteListGO.GetComponent<RoutesList>().SetUpRoutesList();
    //    }
    //    else
    //    {
    //        print("Código Não existe");
    //        ToastMsgWrongCode.SetActive(true);
    //        yield return new WaitForSeconds(1.5f);
    //        ToastMsgWrongCode.SetActive(false);
    //    }
    //}

    ////IEnumerator GetImportedRoute(string code)
    ////{
    ////    string codeFilePath = Application.persistentDataPath + "/" + code + ".json";
    ////    Routes = new List<Route>();
    ////    if (System.IO.File.Exists(codeFilePath))
    ////    {
    ////        ConfirmPanel.SetActive(false);
    ////        LoadingPanel.SetActive(true);
    ////        print(codeFilePath);


    ////        Routes = MainDataHolder.UnofficialRoutes;
    ////        Route r = AddFirstUnnoficialRoute(RouteNode);
    ////        Routes.Add(r);
    ////        MainDataHolder.UnofficialRoutes = Routes;


    ////        RouteListGO.GetComponent<RoutesList>().AddImportedRoute(RouteNode);

    ////        this.GetComponent<SerializableDataElements>().SaveUpdatedRoutesList(Routes, "ur");
    ////        this.GetComponent<SerializableDataElements>().SaveRouteCodeToJson(RouteNode["code"]);
    ////        yield return new WaitForSeconds(3);
    ////        SceneManager.LoadScene("RoutesScene");

    ////    }
    ////    else
    ////    {
    ////        print("Código Não existe");
    ////        ToastMsgWrongCode.SetActive(true);
    ////        yield return new WaitForSeconds(1.5f);
    ////        ToastMsgWrongCode.SetActive(false);
    ////    }
    ////}
}
