using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class JazInformations : MonoBehaviour
{
    JSONNode AllPoiList;

    public JSONNode GetJaz(string jazId)
    {
        TextAsset jsonAllPoiList = Resources.Load<TextAsset>("MapPopularPOI");
        AllPoiList = JSON.Parse(jsonAllPoiList.ToString());

        for (int i = 0; i < AllPoiList["pois"].Count; i++)
        {
            string poiId = AllPoiList["pois"][i]["ID"];
            
            if (poiId.Equals(jazId))
            {
                return AllPoiList["pois"][i];
            }
        }
        return null;
    }

    public string GetJazLatitude(string jazId)
    {
        return GetJaz(jazId)["latitude"];
        
    }

    public string GetJazLongitude(string jazId)
    {
        return GetJaz(jazId)["longitude"];
        
    }

    public string GetJazLocation(string jazId)
    {
        return GetJaz(jazId)["jazLocation"];
    }

    public string GetJazType(string jazId)
    {
        return GetJaz(jazId)["tipoJaz"];
    }

    public string GetJazImage(string jazId)
    {
        return GetJaz(jazId)["jazImage"];
    }

    public List<string> GetJazOfficialRoutes(string jazId)
    {
        List<string> aux = new List<string>();

        for (int i = 0; i < GetJaz(jazId)["percursos"].Count; i++)
        {
            aux.Add(GetJaz(jazId)["percursos"][i]);
        }
        return aux;
    }

    public JSONNode GetPersonality(string jazId, string personId)
    {
        JSONNode jaz = GetJaz(jazId);

        for (int j = 0; j < jaz["personalidades"].Count; j++)
        {
            string personalityId = jaz["personalidades"][j]["uriId"];
            if (personalityId.Equals(personId))
            {
                return jaz["personalidades"][j];
            }

        }
            
        
        return null;
    }

    public string GetPersonalityName(string jazId, string personId)
    {
        return GetPersonality(jazId, personId)["nome"];
    }
}
