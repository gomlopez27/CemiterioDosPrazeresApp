using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RouteCreationDataHolder
{
    public static List<string> SelectedPois;
    public static HashSet<string> SelectedPoisSugestions;
    //private List<string> selectedPersonalities;
    public static Dictionary<string, HashSet<string>> SelectedPersonallitiesPerJaz;
    public static Dictionary<string, HashSet<string>> SelectedPersonalitiesFromSugestions;

    public static List<RelatedPersonality> RelatedPersonalities;
    public static string LastedCreatedRouteCode;
    public static string RelatedPersonalitiesResponseCode;

}
