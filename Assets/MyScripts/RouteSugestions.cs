using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RouteSugestions : MonoBehaviour
{
    [SerializeField]
    GameObject SugestionsListArea;
    //[SerializeField]
    //Button NextButton;
    [SerializeField]
    GameObject SugestionsLoadingArea;
    [SerializeField]
    GameObject SugestionsError;
    //DataHolderRouteCreation DataHolder;
    JSONNode SugestionsList;
    private HashSet<string> selectedPoisSugestions;
    private Dictionary<string, HashSet<string>> selectedPersonalitiesSugestions;
    private List<string> selectedPoisFromOtherLists;
    private string currentRandomPersonIRI;
    private bool hasSetUpList;


    void Start()
    {
        //DataHolder = this.GetComponent<DataHolderRouteCreation>();

        selectedPoisSugestions = new HashSet<string>();
        selectedPersonalitiesSugestions = new Dictionary<string, HashSet<string>>();

        //print("randomIRI: " + RandomPersonalityFromSelected());

        //SetUpSugestionList();
        //NextButton.onClick.AddListener(() => {
        //    DataHolder.SelectedPoisSugestions = selectedPoisSugestions;
        //});
    }

    // Update is called once per frame
    void Update()
    {
        //if (RouteCreationDataHolder.RelatedPersonalitiesResponseCode == null)
        //{
        //    SugestionsLoadingArea.SetActive(true);
        //}
        //else
        //{
        //    if (RouteCreationDataHolder.RelatedPersonalitiesResponseCode == "200" && currentRandomPersonIRI != null && !hasSetUpList)
        //    {
        //        SugestionsLoadingArea.SetActive(false);
        //        SetUpSugestionList();
        //    }
        //    else
        //    {
        //        if(currentRandomPersonIRI != null)
        //        {
        //            SugestionsLoadingArea.SetActive(false);
        //            SugestionsError.SetActive(true);
        //            currentRandomPersonIRI = null;
        //        }

        //    }
        //}


    }



    void SetUpSugestionListOld()
    {
        TextAsset jsonSugestionsList = Resources.Load<TextAsset>("Sugestions");
        SugestionsList = JSON.Parse(jsonSugestionsList.ToString());

        GameObject SugestionItem = SugestionsListArea.transform.GetChild(0).gameObject;

        for (int i = 0; i < SugestionsList["personalities"].Count; i++)
        {
            string jazId = SugestionsList["personalities"][i]["jazID"];
            string personId = SugestionsList["personalities"][i]["uriId"];
            string personName = SugestionsList["personalities"][i]["nome"];
            //string jazType = 
            GameObject g = Instantiate(SugestionItem, SugestionsListArea.transform);
            g.name = "sugestion-" + jazId;
            g.transform.Find("PersName").GetComponent<Text>().text = personName;
            g.transform.Find("PoiId").GetComponent<Text>().text = "Jazigo " + jazId;
            g.transform.Find("Sugestion").GetComponent<Text>().text = SugestionsList["personalities"][i]["sugestion"];

            Toggle toggle = g.transform.Find("Toggle2").GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChangedFromSuggestions(toggle, jazId, personId);
            });

        }
        //print("randomIRI: " + RandomPersonalityFromSelected());
        //FavPoisListArea.SetActive(false);
        Destroy(SugestionItem);

    }

    void SetUpSugestionList()
    {

        if (RouteCreationDataHolder.RelatedPersonalities == null)
        {
            SugestionsLoadingArea.SetActive(true);
        }
        else
        {
            print("SET UP LIST");
            SugestionsLoadingArea.SetActive(false);
            GameObject SugestionItem = SugestionsListArea.transform.GetChild(0).gameObject;
            DestroySugestionsList();
            SugestionItem.SetActive(true);

            for (int i = 0; i < RouteCreationDataHolder.RelatedPersonalities.Count; i++)
            {
                string jazId = RouteCreationDataHolder.RelatedPersonalities[i].Person.PoiId;
                string personId = RouteCreationDataHolder.RelatedPersonalities[i].Person.UriId;
                string personName = RouteCreationDataHolder.RelatedPersonalities[i].Person.Name;
                GameObject g = Instantiate(SugestionItem, SugestionsListArea.transform);
                g.name = "sugestion-" + jazId;
                g.transform.Find("PersName").GetComponent<Text>().text = personName;
                g.transform.Find("PoiId").GetComponent<Text>().text = "Jazigo " + jazId;
                g.transform.Find("Sugestion").GetComponent<Text>().text = RouteCreationDataHolder.RelatedPersonalities[i].Relation;

                Toggle toggle = g.transform.Find("Toggle2").GetComponent<Toggle>();

                toggle.onValueChanged.AddListener(delegate
                {
                    ToggleValueChangedFromSuggestions(toggle, jazId, personId);
                });

            }
            //print("randomIRI: " + RandomPersonalityFromSelected());
            //FavPoisListArea.SetActive(false);
            //Destroy(SugestionItem);
            SugestionItem.SetActive(false);
        }
        currentRandomPersonIRI = null;
        hasSetUpList = true;
    }
    public void DestroySugestionsList()
    {
        if (SugestionsListArea.transform.childCount > 1)
        {
            for (int i = 1; i < SugestionsListArea.transform.childCount; i++)
            {
                Destroy(SugestionsListArea.transform.GetChild(i).gameObject);
            }
        }

    }


    void ToggleValueChangedFromSuggestions(Toggle toggle, string jazId, string personId)
    {
        HashSet<string> personalities;

        if (toggle.isOn)
        {
            selectedPoisSugestions.Add(jazId);

            if (!selectedPersonalitiesSugestions.TryGetValue(jazId, out personalities))
            {
                personalities = new HashSet<string>();
                selectedPersonalitiesSugestions[jazId] = personalities;
            }

            personalities.Add(personId);

        }
        else
        {
            var value = selectedPoisSugestions.Contains(jazId);
            if (value)
                selectedPoisSugestions.Remove(jazId);

            if (selectedPersonalitiesSugestions.TryGetValue(jazId, out personalities))
            {
                var hasPers = personalities.Contains(personId);

                if (hasPers)
                {
                    personalities.Remove(personId);
                }
            }

            if (personalities.Count == 0)
            {
                selectedPersonalitiesSugestions.Remove(jazId);
            }
        }
    }



    /*Checks all the currently selected personalities and chooses one at random. Called when clicked to see the list of suggestions*/
    public void RandomPersonalityFromSelected()
    {
        SugestionsLoadingArea.SetActive(false);
        SugestionsError.SetActive(false);

        string randomIRI = "";

        selectedPoisFromOtherLists = new List<string>();

        if (RouteCreationDataHolder.SelectedPersonallitiesPerJaz != null)
        {
            foreach (KeyValuePair<string, HashSet<string>> entry in RouteCreationDataHolder.SelectedPersonallitiesPerJaz)
            {
                selectedPoisFromOtherLists.Add(entry.Key);
            }

            int indexPoi = Random.Range(0, selectedPoisFromOtherLists.Count);

            print("selectedPoisFromOtherLists.Count: " + selectedPoisFromOtherLists.Count + "     indexPoi: " + indexPoi);

            HashSet<string> personalities;
            List<string> personalitiesAux = new List<string>();
            string jazId = selectedPoisFromOtherLists[indexPoi];

            if (RouteCreationDataHolder.SelectedPersonallitiesPerJaz.TryGetValue(jazId, out personalities))
            {
                personalitiesAux.AddRange(personalities);
            }

            int indexPerson = Random.Range(0, personalitiesAux.Count);


            randomIRI = personalitiesAux[indexPerson];

            currentRandomPersonIRI = randomIRI;

            //StartCoroutine(this.GetComponent<LoadFromAPI>().GetRelatedPersonalities(currentRandomPersonIRI));
            StartCoroutine(GetRelatedPersonalities(currentRandomPersonIRI));
            hasSetUpList = false;
        }



        //return randomIRI;
        print("randomIRI: " + randomIRI);
    }

    public IEnumerator GetRelatedPersonalities(string personIRI)
    {
        RouteCreationDataHolder.RelatedPersonalitiesResponseCode = null;
        SugestionsLoadingArea.SetActive(true);
        //personalities/Q1560243/related-personalities
        UnityWebRequest www = UnityWebRequest.Get(MainDataHolder.URL_API + "/personalities/" + personIRI + "/related-personalities?onCemetery=true");

        yield return www.SendWebRequest();



        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            RouteCreationDataHolder.RelatedPersonalitiesResponseCode = www.responseCode.ToString();
            SugestionsLoadingArea.SetActive(false);
            SugestionsError.SetActive(true);
        }
        else
        {
            string jsonToWrite = www.downloadHandler.text;
            JSONNode PersonalitiesNode = JSON.Parse(jsonToWrite.ToString());
            List<RelatedPersonality> PersonalityList = this.GetComponent<SerializableDataElements>().ConvertJsonRelatedPersonalitiesList(PersonalitiesNode);

            RouteCreationDataHolder.RelatedPersonalities = PersonalityList;
            RouteCreationDataHolder.RelatedPersonalitiesResponseCode = www.responseCode.ToString();
            SugestionsLoadingArea.SetActive(false);
            SetUpSugestionList();
            //System.IO.File.WriteAllText(poisListFilePath, jsonToWrite);
        }
    }

    public void SaveSugestions()
    {
        RouteCreationDataHolder.SelectedPoisSugestions = selectedPoisSugestions;
        RouteCreationDataHolder.SelectedPersonalitiesFromSugestions = selectedPersonalitiesSugestions;
        //OnSuggestionsBtnClicked();
    }
}
