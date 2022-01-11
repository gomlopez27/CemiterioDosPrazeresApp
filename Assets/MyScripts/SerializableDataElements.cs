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
        var RoutesList = new JSONArray();

        foreach (Route r in RoutesCol)
        {
            var routeObj = new JSONObject();
            routeObj["id"] = r.Id;
            routeObj["designation"] = r.Name;
            routeObj["description"] = r.Description;
            routeObj["code"] = r.Code;

            var routeCategoryList = new JSONArray();
            foreach (string s in r.RouteCategory)
            {
                routeCategoryList.Add(s);
            }

            routeObj["routeCategory"] = routeCategoryList;

            var personalitiesList = new JSONArray();
            foreach (Personality person in r.Personalities)
            {
                var personObj = new JSONObject();
                personObj["iri"] = person.UriId;
                personObj["artistic_name"] = person.Name;
                var birthDate = new JSONArray();
                foreach (int date in person.BirthDate)
                {
                    birthDate.Add(date);
                }
                personObj["birth_date"] = birthDate;
                var deathDate = new JSONArray();
                foreach (int date in person.DeathDate)
                {
                    deathDate.Add(date);
                }
                personObj["death_date"] = deathDate;
                personObj["description"] = person.Description;
                personObj["biography"] = person.Biography;
                personObj["imageURL"] = person.ImageUrl;

                var PoiObj = new JSONObject();
                PoiObj["cf_id"] = person.Poi.Id;
                PoiObj["construction_type"] = person.Poi.JazType;

                personObj["funeral_construction"] = PoiObj;

                personalitiesList.Add(personObj);
            }
            routeObj["personalities"] = personalitiesList;

            RoutesList.Add(routeObj);
        }
        return RoutesList;
    }
}

[System.Serializable]
public class Route
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public List<string> RouteCategory { get; set; }
    public List<Personality> Personalities { get; set; }

    /*Values not retrieved directly from json file*/
    public HashSet<string> PoisIdList { get; set; }
    public List<Poi> Pois { get; set; }

    public bool isOfficial { get; set; }


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
            poiObj["cf_id"] = p.Id;
            poiObj["latitude"] = p.Latitude;
            poiObj["longitude"] = p.Longitude;
            poiObj["road"] = p.JazLocation;
            poiObj["construction_type"] = p.JazType;
            poiObj["imageURL"] = p.JazImage; //http://localhost:8080/api/resources/image/JP%3696

            var routesCategories = new JSONArray();
            foreach (string s in p.RoutesCategory)
            {
                routesCategories.Add(s);
            }

            poiObj["percursos"] = routesCategories;

            var personList = new JSONArray();
            foreach (Personality person in p.Personalities)
            {
                var personObj = new JSONObject();
                personObj["iri"] = person.UriId;
                personObj["artistic_name"] = person.Name;
                var birthDate = new JSONArray();
                foreach (int date in person.BirthDate)
                {
                    birthDate.Add(date);
                }
                personObj["birth_date"] = birthDate;
                var deathDate = new JSONArray();
                foreach (int date in person.DeathDate)
                {
                    deathDate.Add(date);
                }
                personObj["death_date"] = deathDate;
                personObj["description"] = person.Description;
                personObj["biography"] = person.Biography;
                personObj["imageURL"] = person.ImageUrl;
                personObj["funeral_construction"] = null;

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
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string JazLocation { get; set; }
    public string JazType { get; set; }
    public string JazImage { get; set; }
    public string Description { get; set; }
    //public string imageIconPlaceholder;
    public List<string> RoutesCategory { get; set; }
    public List<Personality> Personalities { get; set; }

    /*Values not retrieved directly from json file*/
    public string FullId { get; set; }

}

[System.Serializable]
public class Personality
{
    public string UriId { get; set; }
    public string Name { get; set; }
    public List<int> BirthDate { get; set; } //year, month, day
    public List<int> DeathDate { get; set; } //year, month, day
    public string Biography { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public Poi Poi { get; set; }


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

        for (int i = 0; i < jsonPoiList.Count; i++)
        {
            Poi p = new Poi();
            string auxId = jsonPoiList[i]["cf_id"];
            string auxType = jsonPoiList[i]["construction_type"];
            p.Id = auxId;
            p.JazType = auxType;
            p.FullId = auxType + auxId;
            p.Latitude = jsonPoiList[i]["latitude"];
            p.Longitude = jsonPoiList[i]["longitude"];
            p.JazLocation = jsonPoiList[i]["road"];
            p.JazImage = MainDataHolder.URL_API + "resources/image/" + jsonPoiList[i]["imageURL"];
            p.RoutesCategory = new List<string>();

            for (int j = 0; j < jsonPoiList[i]["construction_date"].Count; j++)
            {
                p.RoutesCategory.Add(jsonPoiList[i]["construction_date"][j]);
            }

            p.Personalities = new List<Personality>();

            for (int j = 0; j < jsonPoiList[i]["personalities"].Count; j++)
            {
                JSONNode personNode = jsonPoiList[i]["personalities"][j];
                Personality person = new Personality();
                person.UriId = personNode["iri"];
                person.Name = personNode["artistic_name"];
                person.BirthDate = new List<int>();
                person.DeathDate = new List<int>();
                for (int k = 0; k < personNode["birth_date"].Count; k++)
                {
                    person.BirthDate.Add(personNode["birth_date"][k]);
                }
                for (int k = 0; k < personNode["death_date"].Count; k++)
                {
                    person.DeathDate.Add(personNode["death_date"][k]);
                }
                person.Biography = personNode["biography"];
                person.Description = personNode["description"];
                if (personNode["path_profile_picture"] != null)
                {
                    person.ImageUrl = personNode["path_profile_picture"];
                }
                else
                {
                    person.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png";

                }
                person.Poi = null;
                p.Personalities.Add(person);
            }

            PoiAux.Add(p);

            print(p.Id + " image url: " + p.JazImage);
        }
        return PoiAux;
    }

    public List<Route> ConvertJsonToRouteList(JSONNode jsonRoutesList)
    {
        List<Route> RoutesAux = new List<Route>();

        // print("jsonRoutesList.Count " + jsonRoutesList["routes"].Count);
        for (int i = 0; i < jsonRoutesList.Count; i++)
        {
            JSONNode r = jsonRoutesList[i];
            Route route = ConvertJsonToRoute(r);
            RoutesAux.Add(route);
        }

        return RoutesAux;
    }

    public Route ConvertJsonToRoute(JSONNode r)
    {
        Route route = new Route();
        route.Id = r["id"];
        route.Name = r["designation"];
        route.Code = r["code"];
        route.isOfficial = r["is_official"];
        route.Description = r["description"];

        route.RouteCategory = new List<string>();
        for (int j = 0; j < r["creation_date"].Count; j++)
        {
            route.RouteCategory.Add(r["creation_date"][j]);
        }

        route.Personalities = new List<Personality>();
        route.Pois = new List<Poi>();
        route.PoisIdList = new HashSet<string>();

        for (int j = 0; j < r["personalities"].Count; j++)
        {
            JSONNode personNode = r["personalities"][j];
            Personality person = new Personality();
            person.UriId = personNode["iri"];
            person.Name = personNode["artistic_name"];
            person.BirthDate = new List<int>();
            person.DeathDate = new List<int>();
            for (int k = 0; k < personNode["birth_date"].Count; k++)
            {
                person.BirthDate.Add(personNode["birth_date"][k]);
            }
            for (int k = 0; k < personNode["death_date"].Count; k++)
            {
                person.DeathDate.Add(personNode["death_date"][k]);
            }
            person.Biography = personNode["biography"];
            person.Description = personNode["description"];
            if (personNode["path_profile_picture"] != null)
            {
                person.ImageUrl = personNode["path_profile_picture"];
            }
            else
            {
                person.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png";

            }

            Poi poiOfPerson = new Poi();
            poiOfPerson.Id = personNode["funeral_construction"]["cf_id"];
            poiOfPerson.JazType = personNode["funeral_construction"]["construction_type"];
            person.Poi = poiOfPerson;

            if (route.PoisIdList.Add(poiOfPerson.Id))//hashset apenas adiciona valores unicos
            {
                Poi p = new Poi();
                p.Id = poiOfPerson.Id;
                p.JazType = poiOfPerson.JazType;
                route.Pois.Add(p); // criar lista com os pois unicos para obter os locais fisicos da rota
            }
                                      
            route.Personalities.Add(person);
        }

        //print("FROM SERIALIZABLE route.Personalities:  " + route.Personalities.Count);
        //print("FROM SERIALIZABLE route.PoisIdList:  " + route.PoisIdList.Count);
        //print("FROM SERIALIZABLE route.Pois:  " + route.Pois.Count);
        return route;
    }

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

    public void SaveRouteCodeToJson(string code)
    {
        routeCodes = new List<string>();
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

    public void SaveRouteCodeToJson2(string code)
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

    //public void SaveUpdatedPoiList(List<Poi> updatedList)
    //{
    //    PoiCollection pc = new PoiCollection();
    //    pc.PoiCol = updatedList;
    //    string jsonToWrite = pc.Serialize().ToString(3);
    //    System.IO.File.WriteAllText(poisListFilePath, jsonToWrite);
    //}

    //public void SaveUpdatedRoutesList(List<Route> updatedList, string routeType)
    //{
    //    RoutesCollection rc = new RoutesCollection();
    //    rc.RoutesCol = updatedList;
    //    string jsonToWrite = rc.Serialize().ToString(3);
    //    if (routeType.Equals(OFFICIAL_ROUTE))
    //    {
    //        System.IO.File.WriteAllText(officialRoutesListFilePath, jsonToWrite);

    //    }
    //    else
    //    {
    //        System.IO.File.WriteAllText(unofficialRoutesListFilePath, jsonToWrite);

    //    }
    //}





}
