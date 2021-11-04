using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteSugestions : MonoBehaviour
{
    [SerializeField]
    GameObject SugestionsListArea;
    //[SerializeField]
    //Button NextButton;

    DataHolderRouteCreation DataHolder;
    JSONNode SugestionsList;
    private HashSet<string> selectedPoisSugestions;
    private Dictionary<string, HashSet<string>> selectedSugestions;

    private void Awake()
    {
        TextAsset jsonSugestionsList = Resources.Load<TextAsset>("Sugestions");
        SugestionsList = JSON.Parse(jsonSugestionsList.ToString());


    }

    void Start()
    {
        DataHolder = this.GetComponent<DataHolderRouteCreation>();
        selectedPoisSugestions = new HashSet<string>();
        selectedSugestions = new Dictionary<string, HashSet<string>>();

        SetUpSugestionList();
        //NextButton.onClick.AddListener(() => {
        //    DataHolder.SelectedPoisSugestions = selectedPoisSugestions;
        //});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUpSugestionList()
    {
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
                ToggleValueChanged(toggle, jazId, personId);
            });

        }
        //FavPoisListArea.SetActive(false);
        Destroy(SugestionItem);

    }

    void ToggleValueChanged(Toggle toggle, string jazId, string personId)
    {
        HashSet<string> personalities;

        if (toggle.isOn)
        {
            selectedPoisSugestions.Add(jazId);


            if (!selectedSugestions.TryGetValue(jazId, out personalities))
            {
                personalities = new HashSet<string>();
                selectedSugestions[jazId] = personalities;
            }

            personalities.Add(personId);

        }
        else
        {
            var value = selectedPoisSugestions.Contains(jazId);
            if(value)
                selectedPoisSugestions.Remove(jazId);

            if (selectedSugestions.TryGetValue(jazId, out personalities))
            {
                var hasPers = personalities.Contains(personId);

                if (hasPers)
                {
                    personalities.Remove(personId);
                }
            }

            if (personalities.Count == 0)
            {
                selectedSugestions.Remove(jazId);
            }
        }
    }

    public void SaveSugestions()
    {
        DataHolder.SelectedPoisSugestions = selectedPoisSugestions;
        DataHolder.SelectedSugestions = selectedSugestions;

    }
}
