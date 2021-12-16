using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class MainDataHolder 
{
    public static List<Poi> PopularPois;
    public static List<Route> OfficialRoutes;
    public static List<Route> UnofficialRoutes;
    public static List<string> RouteCodes;
    public static AssetBundle myAssetBundle;
    public static GameObject[] augmentationsGO;
    //public static List<GameObject> PoisSpawnedOnMap;
    //public static List<GameObject> ObjectTrackers;

    public static string RemoveAccents(this string text)
    {
        StringBuilder sbReturn = new StringBuilder();
        var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
        foreach (char letter in arrayText)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }
        return sbReturn.ToString();
    }

    public static Poi GetPoi(string jazId)
    {
        Poi p = PopularPois.Find(x => x.Id == jazId);
        return p;
    }

    public static Personality GetPersonality(string jazId, string personId)
    {
        Poi poi = PopularPois.Find(x => x.Id == jazId);
        Personality p = poi.Personalities.Find(x => x.UriId == personId);
        return p;
    }
}
