using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolderRouteCreation : MonoBehaviour
{
    private List<string> selectedPois;
    private HashSet<string> selectedPoisSugestions;
    //private List<string> selectedPersonalities;
    private Dictionary<string, HashSet<string>> selectedPersonallitiesPerJaz;
    private Dictionary<string, HashSet<string>> selectedSugestions;

    public List<string> SelectedPois
    {
        get { return selectedPois; }
        set { selectedPois = value; }

    }

    public HashSet<string> SelectedPoisSugestions
    {
        get { return selectedPoisSugestions; }
        set { selectedPoisSugestions = value; }
    }

    public Dictionary<string, HashSet<string>> SelectedSugestions
    {
        get { return selectedSugestions; }
        set { selectedSugestions = value; }
    }


    public Dictionary<string, HashSet<string>> SelectedPersonallitiesPerJaz
    {
        get { return selectedPersonallitiesPerJaz; }
        set { selectedPersonallitiesPerJaz = value; }
    }

}
