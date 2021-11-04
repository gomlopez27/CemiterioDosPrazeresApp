using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RoutesCollection
{
    public List<Route> RoutesCol = new List<Route>();

    public JSONNode Serialize()
    {
        var Routes = new JSONObject();

        var RoutesList = new JSONArray();

        foreach (Route r in RoutesCol)
        {
            var routeObj = new JSONObject();
            routeObj["id"] = r.id;
            routeObj["name"] = r.name;
            routeObj["code"] = r.code;

            var routeCategoryList = new JSONArray();
            foreach (string s in r.routeCategory)
            {
                routeCategoryList.Add(s);
            }

            routeObj["routeCategory"] = routeCategoryList;
            routeObj["description"] = r.description;

            var poisList = new JSONArray();
            foreach (Poi p in r.pois)
            {
                var poiObj = new JSONObject();
                poiObj["id"] = p.ID;
                //poiObj["latitude"] = p.latitude;
                //poiObj["longitude"] = p.longitude;
                //poiObj["tipoJaz"] = p.tipoJaz;
                //var personalitiesList = new JSONArray();
                //foreach (string s in p.personalidades)
                //{
                //    personalitiesList.Add(s);
                //}
                //poiObj["personalidades"] = personalitiesList;
                //poiObj["description"] = p.description;
                //poiObj["imageURL"] = p.jazImage;

                poisList.Add(poiObj);
            }
            routeObj["pois"] = poisList;

            RoutesList.Add(routeObj);
        }
        Routes["routes"] = RoutesList;
        return Routes;
    }
}

[System.Serializable]
public class Route
{
    public string id;
    public string name;
    public string code;
    public List<string> routeCategory;
    public string description;
    public List<Poi> pois;

    //public static Route CreateFromJSON(string jsonString)
    //{
    //    return JsonUtility.FromJson<Route>(jsonString);
    //}
}

[System.Serializable]
public class Poi
{
    public string ID;
    //public string latitude;
    //public string longitude;
    ////public string jazLocation;
    //public string tipoJaz;
    //public string jazImage;
    //public string description;
    ////public string imageIconPlaceholder;
    ////public List<string> percursos;
    ////public List<Personality> personalidades;
    //public List<string> personalidades;

}

[System.Serializable]
public class Personality
{
    public string uriId;
    public string nome;
    public string description;

}

[System.Serializable]
public class RoutesCodesCollection
{
    public List<string> RoutesCodes = new List<string>();

    public JSONNode Serialize()
    {
        var codesList = new JSONArray();

        foreach (string s in RoutesCodes)
        {
            codesList.Add(s);
        }
        return codesList;
    }
}
public class SerializableRouteElements : MonoBehaviour
{
    string codesRoutesListFilePath;
    List<string> codes;
    // Start is called before the first frame update
    void Start()
    {
        codesRoutesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";
        codes = new List<string>();
        GetRoutesCodeListFromJson();
    }

    public void GetRoutesCodeListFromJson()
    {
        if (System.IO.File.Exists(codesRoutesListFilePath))
        {
            string json = File.ReadAllText(codesRoutesListFilePath);
            JSONNode CodesList = JSON.Parse(json.ToString());

            for (int i = 0; i < CodesList.Count; i++)
            {
                print(CodesList[i]);
                codes.Add(CodesList[i]);
            }
        }

    }

    public void SaveRouteCodeToJson(string code)
    {
        codes.Add(code);
        RoutesCodesCollection rc = new RoutesCodesCollection();
        rc.RoutesCodes = codes;
        string jsonToWrite = rc.Serialize().ToString(3);
        System.IO.File.WriteAllText(codesRoutesListFilePath, jsonToWrite);
    }
}
