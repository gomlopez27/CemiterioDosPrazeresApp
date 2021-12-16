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
            routeObj["id"] = r.Id;
            routeObj["name"] = r.Name;
            routeObj["code"] = r.Code;

            var routeCategoryList = new JSONArray();
            foreach (string s in r.RouteCategory)
            {
                routeCategoryList.Add(s);
            }

            routeObj["routeCategory"] = routeCategoryList;
            routeObj["description"] = r.Description;

            var poisList = new JSONArray();
            foreach (Poi p in r.Pois)
            {
                var poiObj = new JSONObject();
                poiObj["id"] = p.Id;
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
    public string Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public List<string> RouteCategory { get; set; }
    public string Description { get; set; }
    public List<Poi> Pois { get; set; }

    //public static Route CreateFromJSON(string jsonString)
    //{
    //    return JsonUtility.FromJson<Route>(jsonString);
    //}
}

[System.Serializable]
public class PoiCollection
{
    public List<Poi> PoiCol = new List<Poi>();
   
    /*Converts a List of POI objects to a JSON file*/
    public JSONNode Serialize()
    {
        var Pois = new JSONObject();

        var PoisList = new JSONArray();

        foreach (Poi p in PoiCol)
        {
            var poiObj = new JSONObject();
            poiObj["ID"] = p.Id;
            poiObj["latitude"] = p.Latitude;
            poiObj["longitude"] = p.Longitude;
            poiObj["jazLocation"] = p.JazLocation;
            poiObj["tipoJaz"] = p.JazType;
            poiObj["jazImage"] = p.JazImage;
            poiObj["imageIconPlaceholder"] = p.JazImagePlaceholder;

            var routesList = new JSONArray();
            foreach (string s in p.RoutesCategory)
            {
                routesList.Add(s);
            }

            poiObj["percursos"] = routesList;

            var personList = new JSONArray();
            foreach (Personality person in p.Personalities)
            {
                var personObj = new JSONObject();
                personObj["uriId"] = person.UriId;
                personObj["nome"] = person.Name;
                personObj["description"] = person.Description;
                personObj["imageURL"] = person.ImageUrl;

                personList.Add(personObj);
            }
            poiObj["personalidades"] = personList;

            PoisList.Add(poiObj);
        }
        Pois["pois"] = PoisList;
        return Pois;
    }

    ///*Converts Json To a List of POI objects*/
    //public List<Poi> Deserialize(JSONNode jsonPoiList)
    //{
    //    List<Poi> PoiAux = new List<Poi>();

    //    // print("jsonRoutesList.Count " + jsonRoutesList["routes"].Count);
    //    JSONNode poisNode = jsonPoiList["pois"];
    //    for (int i = 0; i < poisNode.Count; i++)
    //    {
    //        Poi p = new Poi();
    //        //string auxId = poisNode[i]["ID"];
    //        //string auxType = poisNode[i]["tipoJaz"];
    //        //p.id = auxType + auxId;
    //        p.Id = poisNode[i]["ID"];
    //        p.Latitude = poisNode[i]["latitude"];
    //        p.Longitude = poisNode[i]["longitude"];
    //        p.JazLocation = poisNode[i]["jazLocation"];
    //        p.JazType = poisNode[i]["tipoJaz"];
    //        p.JazImage = poisNode[i]["jazImage"];
    //        p.JazImagePlaceholder = poisNode[i]["imageIconPlaceholder"];
    //        p.RoutesCategory = new List<string>();

    //        for (int j = 0; j < poisNode[i]["percursos"].Count; j++)
    //        {
    //            p.RoutesCategory.Add(poisNode[i]["percursos"][j]);
    //        }

    //        p.Personalities = new List<Personality>();

    //        for (int j = 0; j < poisNode[i]["personalidades"].Count; j++)
    //        {
    //            JSONNode personNode = poisNode[i]["personalidades"][j];
    //            Personality person = new Personality();
    //            person.UriId = personNode["uriId"];
    //            person.Name = personNode["nome"];
    //            person.Description = personNode["description"];
    //            person.ImageUrl = personNode["imageURL"];
    //            p.Personalities.Add(person);
    //        }

    //        PoiAux.Add(p);
    //    }
    //    return PoiAux;
    //}
}

[System.Serializable]
public class Poi
{
    public string Id { get; set; }
    //public string id;
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string JazLocation { get; set; }
    public string JazType { get; set; }
    public string JazImage { get; set; }
    public string JazImagePlaceholder { get; set; }
    public string Description { get; set; }
    //public string imageIconPlaceholder;
    public List<string> RoutesCategory { get; set; }
    public List<Personality> Personalities { get; set; }
    //public List<string> personalidades;
}

[System.Serializable]
public class Personality
{
    public string UriId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }

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

[System.Serializable]
public class FavoritesList
{
    public List<string> jazIdList = new List<string>();
}

[System.Serializable]
public class FavoritesDictionary
{
    public Dictionary<string, HashSet<string>> FavoritePersonPerJaz = new Dictionary<string, HashSet<string>>();

    public JSONNode Serialize()
    {
        var favoritesList = new JSONArray();

        foreach (var v in FavoritePersonPerJaz)
        {
            var obj = new JSONObject();

            var list = new JSONArray();
            foreach (var pers in v.Value)
            {
                list.Add(pers);
            }
            obj["jazId"] = v.Key;
            obj["personalities"] = list;

            favoritesList.Add(obj);
        }
        return favoritesList;
    }

    public void Deserialize(JSONNode node)
    {
        FavoritePersonPerJaz = new Dictionary<string, HashSet<string>>();
        var l = node.AsArray;
        foreach (var elem in l)
        {

            HashSet<string> personList = new HashSet<string>();
            string jazId = elem.Value["jazId"];
            var list = elem.Value["personalities"].AsArray;

            foreach (var personId in list)
            {
                personList.Add(personId.Value);

            }
            FavoritePersonPerJaz[elem.Key] = personList;

        }
    }
}

[System.Serializable]
public class ObjectTrackersCollection
{
    public List<ObjectTrackerData> ObjTracker = new List<ObjectTrackerData>();

    public JSONNode Serialize()
    {
        var ObjectTrackerList = new JSONArray();

        foreach (ObjectTrackerData otd in ObjTracker)
        {
            var otdObj = new JSONObject();
            otdObj["wtoUrl"] = otd.WtoUrl;
            otdObj["wtoName"] = otd.WtoName;

            var objTargetList = new JSONArray();
            foreach (ObjectTargetData objTarget in otd.ObjectTargets)
            {
                var objTargetDataObj = new JSONObject();
                objTargetDataObj["targetName"] = objTarget.TargetName;
                objTargetDataObj["augmentation"] = objTarget.Augmentation;

                objTargetList.Add(objTargetDataObj);
            }
            otdObj["targets"] = objTargetList;

            ObjectTrackerList.Add(otdObj);
        }
        return ObjectTrackerList;
    }
}

[System.Serializable]
public class ObjectTrackerData
{
    public string WtoUrl { get; set; }
    public string WtoName { get; set; }
    public List<ObjectTargetData> ObjectTargets { get; set; }
}

[System.Serializable]
public class ObjectTargetData
{
    public string TargetName { get; set; }
    public string Augmentation { get; set; }

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
            //string auxId = poisNode[i]["ID"];
            //string auxType = poisNode[i]["tipoJaz"];
            //p.id = auxType + auxId;
            p.Id = poisNode[i]["ID"];
            p.Latitude = poisNode[i]["latitude"];
            p.Longitude = poisNode[i]["longitude"];
            p.JazLocation = poisNode[i]["jazLocation"];
            p.JazType = poisNode[i]["tipoJaz"];
            p.JazImage = poisNode[i]["jazImage"];
            p.JazImagePlaceholder = poisNode[i]["imageIconPlaceholder"];
            p.RoutesCategory = new List<string>();

            for (int j = 0; j < poisNode[i]["percursos"].Count; j++)
            {
                p.RoutesCategory.Add(poisNode[i]["percursos"][j]);
            }

            p.Personalities = new List<Personality>();

            for (int j = 0; j < poisNode[i]["personalidades"].Count; j++)
            {
                JSONNode personNode = poisNode[i]["personalidades"][j];
                Personality person = new Personality();
                person.UriId = personNode["uriId"];
                person.Name = personNode["nome"];
                person.Description = personNode["description"];
                person.ImageUrl = personNode["imageURL"];
                p.Personalities.Add(person);
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
            route.Id = jsonRoutesList["routes"][i]["id"];
            //print(jsonRoutesList["routes"][i]["id"]);
            route.Name = jsonRoutesList["routes"][i]["name"];
            route.Code = jsonRoutesList["routes"][i]["code"];
            route.Description = jsonRoutesList["routes"][i]["description"];

            route.RouteCategory = new List<string>();

            for (int j = 0; j < jsonRoutesList["routes"][i]["routeCategory"].Count; j++)
            {
                route.RouteCategory.Add(jsonRoutesList["routes"][i]["routeCategory"][j]);
            }

            route.Pois = new List<Poi>();
            for (int k = 0; k < jsonRoutesList["routes"][i]["pois"].Count; k++)
            {
                Poi p = new Poi();
                p.Id = jsonRoutesList["routes"][i]["pois"][k]["id"];
                p.JazType = jsonRoutesList["routes"][i]["pois"][k]["tipoJaz"];
                route.Pois.Add(p);
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
