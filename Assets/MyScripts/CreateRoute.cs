using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateRoute : MonoBehaviour
{
    [SerializeField]
    GameObject CreateRoutePage;
    [SerializeField]
    GameObject SaveRoutePage;
    [SerializeField]
    GameObject AllPoisListArea;
    [SerializeField]
    GameObject FavPoisListArea;
    [SerializeField]
    Button NextButton;
    [SerializeField]
    GameObject ToastMsg;
    [SerializeField]
    GameObject TopText;

    DataHolderRouteCreation DataHolder;
    //JSONNode AllPoiList;
    JSONNode FavPoiList;
    private string filePath;
    private HashSet<string> selectedPois;
    //private HashSet<string> selectedPersonallities;
    private Dictionary<string, Toggle> PoiItems;
    private Dictionary<string, Toggle> FavItems;
    private Dictionary<string, HashSet<string>> selectedPersonallitiesPerJaz;
    private List<string> selectedPoiItems;
    private List<string> selectedFavItems;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/FavoritesList.json";
        print(filePath);
        //TextAsset jsonAllPoiList = Resources.Load<TextAsset>("MapPopularPOI");
        //AllPoiList = JSON.Parse(jsonAllPoiList.ToString());
    }

    void Start()
    {
        DataHolder = this.GetComponent<DataHolderRouteCreation>();
        selectedPois = new HashSet<string>();
        //selectedPersonallities = new HashSet<string>();
        PoiItems = new Dictionary<string, Toggle>();
        FavItems = new Dictionary<string, Toggle>();
        selectedPoiItems = new List<string>();
        selectedFavItems = new List<string>();
        selectedPersonallitiesPerJaz = new Dictionary<string, HashSet<string>>();

        List<string> auxSelectedPois = new List<string>();
        //List<string> auxSelectedPersonalities = new List<string>();
        SetUpPoisList();
        SetUpFavoritesList();

        FavPoisListArea.SetActive(false);
        
        NextButton.onClick.AddListener(()=> {
            this.GetComponent<RouteSugestions>().SaveSugestions();

            int totalSelected = selectedPois.Count + DataHolder.SelectedPoisSugestions.Count;
            
            if (totalSelected > 1)
            {
                CreateRoutePage.SetActive(false);
                SaveRoutePage.SetActive(true);
                this.GetComponent<SaveCreatedRoute>().enabled = true;
                auxSelectedPois.AddRange(selectedPois);
                Dictionary<string, HashSet<string>> auxSelectedPersonalities = new Dictionary<string, HashSet<string>>(selectedPersonallitiesPerJaz);
                DataHolder.SelectedPois = auxSelectedPois;
                DataHolder.SelectedPersonallitiesPerJaz = auxSelectedPersonalities;
            }
            else
            {
                StartCoroutine(ShowToastMsg());
            }
        });
    }

    void SetUpPoisList()
    {
        GameObject AllPoisListItem = AllPoisListArea.transform.GetChild(0).gameObject;
        TopText.SetActive(true);

        for (int i = 0; i < MainDataHolder.PopularPois.Count; i++)
        {
            string jazId = MainDataHolder.PopularPois[i].Id;
            string jazType = MainDataHolder.PopularPois[i].JazType;
            GameObject g = Instantiate(AllPoisListItem, AllPoisListArea.transform);
            g.name = "item-" + jazId;
            Toggle jazToggle = g.transform.Find("Info/Toggle").GetComponent<Toggle>();
            Button openBtn = g.transform.Find("Info/Button-Open").GetComponent<Button>();
            GameObject PersonalityListItem = g.transform.Find("Content").transform.Find("Item").gameObject;

            /*Multiplas personalidades*/
            if (MainDataHolder.PopularPois[i].Personalities.Count > 1)
            {
                g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": Múltiplas Personalidades";
                jazToggle.gameObject.SetActive(false);
                openBtn.gameObject.SetActive(true);
                PersonalityListItem.SetActive(true);

                for(int k = 0; k < MainDataHolder.PopularPois[i].Personalities.Count; k++)
                {
                    string personName = MainDataHolder.PopularPois[i].Personalities[k].Name;

                    string personId = MainDataHolder.PopularPois[i].Personalities[k].UriId;
                    GameObject gPers = Instantiate(PersonalityListItem, g.transform.Find("Content").transform);
                    //gPers.name = "pers-" + personId;
                    gPers.name = "item-" + jazId + "-" + personId;
                    gPers.transform.Find("PersName").GetComponent<Text>().text = personName;
                    Toggle persToggle = gPers.transform.Find("Toggle").GetComponent<Toggle>();
                    PoiItems.Add(gPers.name, persToggle);
                    persToggle.onValueChanged.AddListener(delegate
                    {
                        ToggleValueChanged(persToggle, jazId);
                        PersonalityToggleValueChanged(persToggle, jazId, personId);
                        if (persToggle.isOn)
                        {
                            //PoiItems.Add(gPers.name, persToggle);
                            selectedPoiItems.Add(gPers.name);
                        }
                        else
                        {
                            //Toggle t;
                            //if (PoiItems.TryGetValue(gPers.name, out t))
                            //{
                            //    PoiItems.Remove(gPers.name);
                            //}
                            if (selectedPoiItems.Contains(gPers.name))
                            {
                                selectedPoiItems.Remove(gPers.name);
                            }
                        }
                    });
                }
                PersonalityListItem.SetActive(false);
            }
            else  /*Uma personalidade*/
            {
                string personName = MainDataHolder.PopularPois[i].Personalities[0].Name;
                string personId = MainDataHolder.PopularPois[i].Personalities[0].UriId;
                g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": " + personName;
                g.name = "item-" + jazId + "-" + personId;
                jazToggle.gameObject.SetActive(true);
                openBtn.gameObject.SetActive(false);
                PersonalityListItem.SetActive(false);
                PoiItems.Add(g.name, jazToggle);

                jazToggle.onValueChanged.AddListener(delegate
                {
                    ToggleValueChanged(jazToggle, jazId);
                    PersonalityToggleValueChanged(jazToggle, jazId, personId);
                    if (jazToggle.isOn)
                    {
                        //PoiItems.Add(g.name, jazToggle);
                        selectedPoiItems.Add(g.name);

                    }
                    else
                    {
                        //Toggle t;
                        //if (PoiItems.TryGetValue(g.name, out t))
                        //{
                        //    PoiItems.Remove(g.name);
                        //}
                        if (selectedPoiItems.Contains(g.name))
                        {
                            selectedPoiItems.Remove(g.name);
                        }
                    }
                });

            }
        }
        //FavPoisListArea.SetActive(false);
        Destroy(AllPoisListItem);

    }

    void SetUpFavoritesList()
    {
        GameObject FavPoisListItem = FavPoisListArea.transform.GetChild(0).gameObject;
        GameObject EmptyListItem = FavPoisListArea.transform.GetChild(1).gameObject;
        //GameObject PersonalityListItem = PersonalitiesListArea.transform.GetChild(0).gameObject;

        if (!System.IO.File.Exists(filePath))
        {
            FavPoisListItem.SetActive(false);
            TopText.SetActive(false);
            EmptyListItem.SetActive(true);
        }
        else
        {
            string jsonFavPoisListArea = File.ReadAllText(filePath);
            FavPoiList = JSON.Parse(jsonFavPoisListArea.ToString());

            if (FavPoiList.Count == 0)
            {
                FavPoisListItem.SetActive(false);
                TopText.SetActive(false);
                EmptyListItem.SetActive(true);
                //GameObject g2 = Instantiate(FavPoisListItem, FavPoisListArea.transform);
                //g2.name = "empty";
                //g2.transform.Find("Info/PoiName").GetComponent<Text>().text = "Lista vazia.";
                //AllPoisListItem.transform.Find("Toggle").GetComponent<Toggle>().gameObject.SetActive(false);
            }

            EmptyListItem.SetActive(false);
            TopText.SetActive(true);

            for (int i = 0; i < FavPoiList.Count; i++)
            {
                string jazId = FavPoiList[i]["jazId"];
                //string jazType = this.GetComponent<JazInformations>().GetJazType(jazId);
                string jazType = MainDataHolder.GetPoi(jazId).JazType;
                GameObject g = Instantiate(FavPoisListItem, FavPoisListArea.transform);
                g.name = "item-" + jazId;
                Toggle jazToggle = g.transform.Find("Info/Toggle").GetComponent<Toggle>();
                Button openBtn = g.transform.Find("Info/Button-Open").GetComponent<Button>();
                GameObject PersonalityListItem = g.transform.Find("Content").transform.Find("Item").gameObject;

                /*Multiplas personalidades*/
                if (FavPoiList[i]["personalities"].Count > 1)
                {
                    g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": Múltiplas Personalidades";
                    jazToggle.gameObject.SetActive(false);
                    openBtn.gameObject.SetActive(true);
                    PersonalityListItem.SetActive(true);

                    for (int k = 0; k < FavPoiList[i]["personalities"].Count; k++)
                    {
                        string personId = FavPoiList[i]["personalities"][k];
                        //string personName = this.GetComponent<JazInformations>().GetPersonalityName(jazId, personId);
                        string personName = MainDataHolder.GetPersonality(jazId, personId).Name;
                        GameObject gPers = Instantiate(PersonalityListItem, g.transform.Find("Content").transform);
                        //gPers.name = "pers-" + personId;
                        gPers.name = "item-" + jazId + "-" + personId;
                        gPers.transform.Find("PersName").GetComponent<Text>().text = personName;
                        Toggle persToggle = gPers.transform.Find("Toggle").GetComponent<Toggle>();
                        FavItems.Add(gPers.name, persToggle);

                        persToggle.onValueChanged.AddListener(delegate
                        {
                            ToggleValueChanged(persToggle, jazId);
                            PersonalityToggleValueChanged(persToggle, jazId, personId);
                            if (persToggle.isOn)
                            {
                                //FavItems.Add(gPers.name, persToggle);
                                selectedFavItems.Add(gPers.name);

                            }
                            else
                            {
                                //Toggle t;
                                //if (FavItems.TryGetValue(gPers.name, out t))
                                //{
                                //    FavItems.Remove(gPers.name);
                                //}
                                if (selectedFavItems.Contains(gPers.name))
                                {
                                    selectedFavItems.Remove(gPers.name);
                                }
                            }
                        });

                    }

                    PersonalityListItem.SetActive(false);
                }
                else /*Uma personalidade*/
                {
                    string personId = FavPoiList[i]["personalities"][0];
                    //string personName = this.GetComponent<JazInformations>().GetPersonalityName(jazId, personId);
                    string personName = MainDataHolder.GetPersonality(jazId, personId).Name;
                    g.name = "item-" + jazId + "-" + personId;

                    g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": " + personName;
                    jazToggle.gameObject.SetActive(true);
                    openBtn.gameObject.SetActive(false);
                    PersonalityListItem.SetActive(false);
                    FavItems.Add(g.name, jazToggle);

                    jazToggle.onValueChanged.AddListener(delegate
                    {
                        ToggleValueChanged(jazToggle, jazId);
                        PersonalityToggleValueChanged(jazToggle, jazId, personId);
                        if (jazToggle.isOn)
                        {
                            //FavItems.Add(g.name, jazToggle);
                            selectedFavItems.Add(g.name);

                        }
                        else
                        {
                            //Toggle t;
                            //if (FavItems.TryGetValue(g.name, out t))
                            //{
                            //    FavItems.Remove(g.name);
                            //}
                            if (selectedFavItems.Contains(g.name))
                            {
                                selectedFavItems.Remove(g.name);
                            }
                        }
                    });
                }
            }

            Destroy(FavPoisListItem);
        }

    }

    /*Atualiza os toggles list de POIS em concordacia com os toogles checked na lista dos FAVS*/
    public void CheckSamePoiToggles()
    {
        foreach (var p in PoiItems)
        {
           foreach (string s in selectedFavItems)
            {
                if (p.Key.Equals(s))
                {
                    p.Value.isOn = true;
                    print("parent: " + p.Value.gameObject.transform.parent.name);
                }
            }
        }


    }

    /*Atualiza os toggles list de FAVS em concordacia com os toogles checked na lista dos POIS*/
    public void CheckSameFavToggles()
    {
        foreach (var f in FavItems)
        {
            foreach(string s in selectedPoiItems)
            {
                if (f.Key.Equals(s))
                {
                    f.Value.isOn = true;
                }
            }

        }

    }

    void ToggleValueChanged(Toggle toggle, string toggleTxt)
    {
        print(toggleTxt + " " + toggle.isOn);
        if (toggle.isOn)
        {
            selectedPois.Add(toggleTxt);
        }
        else
        {
            var value = selectedPois.Contains(toggleTxt);
            if (value)
            {
                selectedPois.Remove(toggleTxt);
            }

        }

    }

    void PersonalityToggleValueChanged(Toggle toggle, string jazId, string personId)
    {
        HashSet<string> personalities;

        if (toggle.isOn)
        {
            //selectedPersonallities.Add(personId);
            //print(personId + " " + toggle.isOn);


            if (!selectedPersonallitiesPerJaz.TryGetValue(jazId, out personalities))
            {
                personalities = new HashSet<string>();
                selectedPersonallitiesPerJaz[jazId] = personalities;
            }

            personalities.Add(personId);

        }
        else
        {
            //var value = selectedPersonallities.Contains(personId);
            //if (value)
            //{
            //    selectedPersonallities.Remove(personId);
            //}

            if (selectedPersonallitiesPerJaz.TryGetValue(jazId, out personalities))
            {
                var value = personalities.Contains(personId);

                if (value)
                {
                    personalities.Remove(personId);
                }
            }

            if (personalities.Count == 0)
            {
                selectedPersonallitiesPerJaz.Remove(jazId);
            }

        }

    }

    IEnumerator ShowToastMsg()
    {
        ToastMsg.SetActive(true);
        yield return new WaitForSeconds(3);
        ToastMsg.SetActive(false);

    }
   
    IEnumerator PoiChoicesAnd()
    {
        byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        using (UnityWebRequest www = UnityWebRequest.Put("https://www.my-server.com/upload", myData))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }

  
}
