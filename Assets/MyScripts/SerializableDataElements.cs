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
                poiObj["id"] = p.id;
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
public class PoiCollection
{
    public List<Poi> PoiCol = new List<Poi>();

    public JSONNode Serialize()
    {
        var Pois = new JSONObject();

        var PoisList = new JSONArray();

        foreach (Poi p in PoiCol)
        {
            var poiObj = new JSONObject();
            poiObj["ID"] = p.id;
            poiObj["latitude"] = p.latitude;
            poiObj["longitude"] = p.longitude;
            poiObj["jazLocation"] = p.jazLocation;
            poiObj["tipoJaz"] = p.tipoJaz;
            poiObj["jazImage"] = p.jazImage;

            var routesList = new JSONArray();
            foreach (string s in p.percursos)
            {
                routesList.Add(s);
            }

            poiObj["percursos"] = routesList;

            var personList = new JSONArray();
            foreach (Personality person in p.personalidades)
            {
                var personObj = new JSONObject();
                personObj["uriId"] = person.uriId;
                personObj["nome"] = person.nome;
                personObj["description"] = person.description;
                personObj["imageURL"] = person.imageUrl;

                personList.Add(personObj);
            }
            poiObj["personalidades"] = personList;

            PoisList.Add(poiObj);
        }
        Pois["pois"] = PoisList;
        return Pois;
    }
}
[System.Serializable]
public class Poi
{
    public string id;
    public string latitude;
    public string longitude;
    public string jazLocation;
    public string tipoJaz;
    public string jazImage;
    public string description;
    //public string imageIconPlaceholder;
    public List<string> percursos;
    public List<Personality> personalidades;
    //public List<string> personalidades;

}

[System.Serializable]
public class Personality
{
    public string uriId;
    public string nome;
    public string description;
    public string imageUrl;

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

public class SerializableDataElements : MonoBehaviour
{
    public const string UNOFFICIAL_ROUTE = "ur";
    public const string OFFICIAL_ROUTE = "or";

    private string poisListFilePath;
    private string officialRoutesListFilePath;
    private string unofficialRoutesListFilePath;
    private string codesRoutesListFilePath;
    List<string> routeCodes;


    // Start is called before the first frame update
    void Start()
    {
        poisListFilePath = Application.persistentDataPath + "/PoiList.json";
        officialRoutesListFilePath = Application.persistentDataPath + "/OfficialRoutesList.json";
        unofficialRoutesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
        codesRoutesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";
        routeCodes = new List<string>();
        //CreatePoiListFromJson();
        //CreateOfficialRouteListFromJson();
        //CreateUnofficialRouteListFromJson();
        //CreateRoutesCodeListFromJson();
    }


    public List<Poi> ConvertJsonToPoiList(JSONNode jsonPoiList)
    {
        List<Poi> PoiAux = new List<Poi>();

        // print("jsonRoutesList.Count " + jsonRoutesList["routes"].Count);
        JSONNode poisNode = jsonPoiList["pois"];
        for (int i = 0; i < poisNode.Count; i++)
        {
            Poi p = new Poi();
            string auxId = poisNode[i]["ID"];
            string auxType = poisNode[i]["tipoJaz"];
            p.id = auxType + auxId;
            p.latitude = poisNode[i]["latitude"];
            p.longitude = poisNode[i]["longitude"];
            p.jazLocation = poisNode[i]["jazLocation"];
            p.tipoJaz = auxType;
            p.jazImage = poisNode[i]["jazImage"];
            p.percursos = new List<string>();

            for (int j = 0; j < poisNode[i]["percursos"].Count; j++)
            {
                p.percursos.Add(poisNode[i]["percursos"][j]);
            }

            p.personalidades = new List<Personality>();

            for (int j = 0; j < poisNode[i]["personalidades"].Count; j++)
            {
                JSONNode personNode = poisNode[i]["personalidades"][j];
                Personality person = new Personality();
                person.uriId = personNode["uriId"];
                person.nome = personNode["nome"];
                person.description = personNode["description"];
                person.imageUrl = personNode["imageURL"];
                p.personalidades.Add(person);
            }

            PoiAux.Add(p);
        }

        return PoiAux;
    }
    public List<Route> ConvertJsonToRouteList(JSONNode jsonRoutesList)
    {
        List<Route> RoutesAux = new List<Route>();

        // print("jsonRoutesList.Count " + jsonRoutesList["routes"].Count);
        for (int i = 0; i < jsonRoutesList["routes"].Count; i++)
        {
            Route route = new Route();
            route.id = jsonRoutesList["routes"][i]["id"];
            //print(jsonRoutesList["routes"][i]["id"]);
            route.name = jsonRoutesList["routes"][i]["name"];
            route.code = jsonRoutesList["routes"][i]["code"];
            route.description = jsonRoutesList["routes"][i]["description"];

            route.routeCategory = new List<string>();

            for (int j = 0; j < jsonRoutesList["routes"][i]["routeCategory"].Count; j++)
            {
                route.routeCategory.Add(jsonRoutesList["routes"][i]["routeCategory"][j]);
            }

            route.pois = new List<Poi>();
            for (int k = 0; k < jsonRoutesList["routes"][i]["pois"].Count; k++)
            {
                Poi p = new Poi();
                p.id = jsonRoutesList["routes"][i]["pois"][k]["id"];
                route.pois.Add(p);
            }

            RoutesAux.Add(route);
        }

        return RoutesAux;
    }

    //public bool CreatePoiListFromJson(string filePath)
    //{
    //    if (System.IO.File.Exists(filePath))
    //    {
    //        string poisJsonString = File.ReadAllText(filePath);
    //        JSONNode PoisNode = JSON.Parse(poisJsonString.ToString());
    //        List<Poi> PoisList = ConvertJsonToPoiList(PoisNode);
    //        MainDataHolder.PopularPois = PoisList;
    //        print("MainDataHolder.PopularPois: " + MainDataHolder.PopularPois.Count);
    //        return true;

    //    }
    //    print("poisListFilePath not found");
    //    return false;
    //}

    //public bool CreateOfficialRouteListFromJson(string filePath)
    //{
    //    if (System.IO.File.Exists(filePath))
    //    {
    //        string jsonOffiRoutes = File.ReadAllText(filePath);
    //        JSONNode OfficialRoutesJson = JSON.Parse(jsonOffiRoutes.ToString());
    //        List<Route> OfficialRoutes = ConvertJsonToRouteList(OfficialRoutesJson);
    //        MainDataHolder.OfficialRoutes = OfficialRoutes;
    //        return true;
    //    }

    //    print("officialRoutesListFilePath not found");
    //    return false;
    //}

    //public bool CreateUnofficialRouteListFromJson(string filePath)
    //{
    //    if (System.IO.File.Exists(filePath))
    //    {
    //        string jsonUnoRoutes = File.ReadAllText(filePath);
    //        JSONNode UnofficialRoutesJson = JSON.Parse(jsonUnoRoutes.ToString());
    //        List<Route> UnofficialRoutes = ConvertJsonToRouteList(UnofficialRoutesJson);
    //        MainDataHolder.UnofficialRoutes = UnofficialRoutes;
    //        return true;
    //    }
    //    print("unofficialRoutesListFilePath not found");
    //    return false;
    //}

    public void CreateRoutesCodeListFromJson(string filePath)
    {
        List<string> codes = new List<string>();

        if (System.IO.File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JSONNode CodesList = JSON.Parse(json.ToString());

            for (int i = 0; i < CodesList.Count; i++)
            {
                codes.Add(CodesList[i]);
            }
            routeCodes = codes;
            MainDataHolder.RouteCodes = codes;
        }
    }

    /*Old method, depois apagar e trocar pelo debaixo*/
    public void SaveRouteCodeToJson(string code)
    {
        if (System.IO.File.Exists(codesRoutesListFilePath))
        {
            string json = File.ReadAllText(codesRoutesListFilePath);
            JSONNode CodesList = JSON.Parse(json.ToString());

            for (int i = 0; i < CodesList.Count; i++)
            {
                routeCodes.Add(CodesList[i]);
            }
        }
        routeCodes.Add(code);
        RoutesCodesCollection rc = new RoutesCodesCollection();
        rc.RoutesCodes = routeCodes;
        string jsonToWrite = rc.Serialize().ToString(3);
        System.IO.File.WriteAllText(codesRoutesListFilePath, jsonToWrite);
    }

    public void SaveUpdatedRouteCodeList(List<string> updatedList)
    {
        RoutesCodesCollection rc = new RoutesCodesCollection();
        rc.RoutesCodes = updatedList;
        string jsonToWrite = rc.Serialize().ToString(3);
        System.IO.File.WriteAllText(codesRoutesListFilePath, jsonToWrite);
    }
    
    public void SaveUpdatedPoiList(List<Poi> updatedList)
    {
        PoiCollection pc = new PoiCollection();
        pc.PoiCol = updatedList;
        string jsonToWrite = pc.Serialize().ToString(3);
        System.IO.File.WriteAllText(poisListFilePath, jsonToWrite);
    }

    public void SaveUpdatedRoutesList(List<Route> updatedList, string routeType)
    {
        RoutesCollection rc = new RoutesCollection();
        rc.RoutesCol = updatedList;
        string jsonToWrite = rc.Serialize().ToString(3);
        if (routeType.Equals(OFFICIAL_ROUTE))
        {
            System.IO.File.WriteAllText(officialRoutesListFilePath, jsonToWrite);

        }
        else
        {
            System.IO.File.WriteAllText(unofficialRoutesListFilePath, jsonToWrite);

        }
    }

}
