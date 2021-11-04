using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SaveCreatedRoute : MonoBehaviour
{
    public const string UNOFFICIAL_ROUTE_ID = "ur";

    [SerializeField]
    InputField InputRouteName;
    [SerializeField]
    InputField InputRouteDescription;
    [SerializeField]
    GameObject SelectedPoisListArea;
    [SerializeField]
    GameObject ToastMsg;
    [SerializeField]
    GameObject LoadingPanel;
    [SerializeField]
    LoadScenes Scenes;
    [SerializeField]
    Button CreatedBtn;

    DataHolderRouteCreation DataHolder;
    List<string> finalChoicesPois;
    List<string> finalChoicesPersonalities;
    string routeName;
    string routeDescription;
    JSONNode AllPoiList;
    JSONNode UnofficialRoutesList;
    //RoutesCollection rc;
    List<Route> UnofficialRoutesObjList;
    //TextAsset jsonAllRoutesList;
    string unofficialRoutesListFilePath;


    private void Awake()
    {
        TextAsset jsonAllPoiList = Resources.Load<TextAsset>("MapPopularPOI");
        AllPoiList = JSON.Parse(jsonAllPoiList.ToString());

        //jsonAllRoutesList = Resources.Load<TextAsset>("RoutesList");
        //AllRoutesList = JSON.Parse(jsonAllRoutesList.ToString());

      
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadingPanel.SetActive(false);
        unofficialRoutesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";

        //if (!System.IO.File.Exists(routesListFilePath))
        //{
        //    System.IO.File.WriteAllText(routesListFilePath, jsonAllRoutesList.text);
        //}
        print(unofficialRoutesListFilePath);

        DataHolder = this.GetComponent<DataHolderRouteCreation>();
        finalChoicesPois = new List<string>();
        finalChoicesPersonalities = new List<string>();
        AddSugestionsToFinalChoices();


        //Lista com apenas os ids de todos os jazigos selecionados
        finalChoicesPois.AddRange(DataHolder.SelectedPois);
        finalChoicesPois.AddRange(DataHolder.SelectedPoisSugestions);


        //finalChoicesPersonalities.AddRange(DataHolder.SelectedPersonalities);
        print("SelectedPois " + DataHolder.SelectedPois.Count);
        print("SelectedPoisSugestions (hash set)" + DataHolder.SelectedPoisSugestions.Count);
        print("SelectedSugestions (dictionary) " + DataHolder.SelectedSugestions.Count);
        print("finalChoicesPois " + finalChoicesPois.Count);
        print("SelectedPersonallitiesPerJaz " + DataHolder.SelectedPersonallitiesPerJaz.Count);
        SetUpList();

        //rc = new RoutesCollection();
        UnofficialRoutesObjList = new List<Route>();
        GetRouteListFromJson();
        //GetRoutesCodeListFromJson();
        CreatedBtn.onClick.AddListener(SaveRoute);
    }


    void AddSugestionsToFinalChoices()
    {
        HashSet<string> personalities;

        foreach (var elem in DataHolder.SelectedSugestions)
        {
            if (!DataHolder.SelectedPersonallitiesPerJaz.TryGetValue(elem.Key, out personalities))
            {
                personalities = new HashSet<string>(elem.Value); //elem.value contem as person das sugestions selectionadas
                DataHolder.SelectedPersonallitiesPerJaz[elem.Key] = personalities;
                print("Mais um jaz: " + elem.Key);
            }
            else
            {
                foreach (string s in elem.Value)
                {
                    if (!personalities.Contains(s))
                    {
                        personalities.Add(s);
                        print("Mais uma personalidade: " + s + "no jaz: " + elem.Key);

                    }
                }
            }


        }
    }

    void SetUpList()
    {
        GameObject ListItem = SelectedPoisListArea.transform.GetChild(0).gameObject;

        foreach (var v in DataHolder.SelectedPersonallitiesPerJaz)
        {
            //for (int i = 0; i < DataHolder.SelectedPersonallitiesPerJaz.Count; i++)
            //{
            string jazId = v.Key;
            string jazType = this.GetComponent<JazInformations>().GetJazType(jazId);
            GameObject g = Instantiate(ListItem, SelectedPoisListArea.transform);
            g.name = "item-" + jazId;
            Toggle jazToggle = g.transform.Find("Info/Toggle").GetComponent<Toggle>();
            Button openBtn = g.transform.Find("Info/Button-Open").GetComponent<Button>();
            Button closeBtn = g.transform.Find("Info/Button-Close").GetComponent<Button>();
            GameObject PersonalityListItem = g.transform.Find("Content").transform.Find("Item").gameObject;
            HashSet<string> personalities = v.Value;
            List<string> auxPers = new List<string>();
            auxPers.AddRange(personalities);

            /*Multiplas personalidades*/
            if (auxPers.Count > 1)
            {
                g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": Múltiplas Personalidades";
                jazToggle.gameObject.SetActive(false);
                openBtn.gameObject.SetActive(false);
                closeBtn.gameObject.SetActive(true);
                PersonalityListItem.SetActive(true);

                for (int k = 0; k < auxPers.Count; k++)
                {
                    string personId = auxPers[k];
                    string personName = this.GetComponent<JazInformations>().GetPersonalityName(jazId, personId);

                    GameObject gPers = Instantiate(PersonalityListItem, g.transform.Find("Content").transform);
                    //gPers.name = "pers-" + personId;
                    gPers.name = "item-" + jazId + "-" + personId;
                    gPers.transform.Find("PersName").GetComponent<Text>().text = personName;
                    Toggle persToggle = gPers.transform.Find("Toggle").GetComponent<Toggle>();
                    persToggle.isOn = true;

                    persToggle.onValueChanged.AddListener(delegate
                    {
                        ToggleValueChanged(persToggle, jazId, gPers);

                    });
                }
                PersonalityListItem.SetActive(false);
            }
            else  /*Uma personalidade*/
            {
                string personId = auxPers[0];
                string personName = this.GetComponent<JazInformations>().GetPersonalityName(jazId, personId);
                g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": " + personName;
                g.name = "item-" + jazId + "-" + personId;
                jazToggle.gameObject.SetActive(true);
                openBtn.gameObject.SetActive(false);
                PersonalityListItem.SetActive(false);
                jazToggle.isOn = true;


                jazToggle.onValueChanged.AddListener(delegate
                {
                    ToggleValueChanged(jazToggle, jazId, g);

                });

            }
        }
        //FavPoisListArea.SetActive(false);
        Destroy(ListItem);
    }

    void ToggleValueChanged(Toggle toggle, string jazId, GameObject listItem)
    {
        if (!toggle.isOn && finalChoicesPois.Count > 2)
        {
            //Se o pai é a lista principal, entao e um jaz com uma unica person
            if (listItem.transform.parent.name == SelectedPoisListArea.name)
            {
                Destroy(listItem);
                var value = finalChoicesPois.Find(item => item == jazId);
                finalChoicesPois.Remove(value);
            }
            else
            {
                //print(listItem.transform.parent.name + ": " + listItem.transform.parent.childCount);
                GameObject firstListItem = listItem.transform.parent.transform.GetChild(0).gameObject;

                if (listItem.transform.parent.transform.childCount == 2 && firstListItem.name.Equals("Item"))
                {
                    Destroy(listItem);
                    Destroy(listItem.transform.parent.parent.gameObject);
                    var value = finalChoicesPois.Find(item => item == jazId);
                    finalChoicesPois.Remove(value);
                }
                else
                {
                    Destroy(listItem);
                }
            }
        }
        else
        {
            print(listItem.transform.parent.name + ": " + listItem.transform.parent.childCount);

            //Se o pai nao e' a lista principal, entao e um jaz com multiplas person
            if (listItem.transform.parent.name != SelectedPoisListArea.name
                && listItem.transform.parent.childCount > 2)
            {
                Destroy(listItem);
            }
            else
            {
                toggle.isOn = true;
                print("Não pode remover todos os pontos");
                StartCoroutine(ShowToastMsg());
            }


        }

    }

    IEnumerator ShowToastMsg()
    {
        ToastMsg.SetActive(true);
        yield return new WaitForSeconds(3);
        ToastMsg.SetActive(false);

    }


    IEnumerator ShowLoadingScreen()
    {
        LoadingPanel.SetActive(true);
        yield return new WaitForSeconds(5);
        Scenes.LoadRoutesScene();

    }

    public void SetRouteName(string name)
    {
        this.routeName = name;
    }

    public void SetRouteDescription(string descr)
    {
        this.routeDescription = descr;
    }
    
    void GetRouteListFromJson()
    {
        if (System.IO.File.Exists(unofficialRoutesListFilePath))
        {
            string json = File.ReadAllText(unofficialRoutesListFilePath);
            UnofficialRoutesList = JSON.Parse(json.ToString());
            
            for (int i = 0; i < UnofficialRoutesList["routes"].Count; i++)
            {
                Route route = new Route();
                route.id = UnofficialRoutesList["routes"][i]["id"];
                route.name = UnofficialRoutesList["routes"][i]["name"];
                route.code = UnofficialRoutesList["routes"][i]["code"];
                route.description = UnofficialRoutesList["routes"][i]["description"];

                route.routeCategory = new List<string>();

                for (int j = 0; j < UnofficialRoutesList["routes"][i]["routeCategory"].Count; j++)
                {
                    route.routeCategory.Add(UnofficialRoutesList["routes"][i]["routeCategory"][j]);
                }

                route.pois = new List<Poi>();
                for (int k = 0; k < UnofficialRoutesList["routes"][i]["pois"].Count; k++)
                {
                    Poi p = new Poi();
                    p.ID = UnofficialRoutesList["routes"][i]["pois"][k]["id"];
                    //p.latitude = AllRoutesList["routes"][i]["pois"][k]["latitude"];
                    //p.longitude = AllRoutesList["routes"][i]["pois"][k]["longitude"];
                    //p.tipoJaz = AllRoutesList["routes"][i]["pois"][k]["tipoJaz"];
                    //p.jazImage = AllRoutesList["routes"][i]["pois"][k]["jazImage"];
                    //p.description = AllRoutesList["routes"][i]["pois"][k]["description"];

                    //p.personalidades = new List<string>();

                    //for (int y = 0; y < AllRoutesList["routes"][i]["pois"][k]["personalidades"].Count; y++)
                    //{
                    //    p.personalidades.Add(AllRoutesList["routes"][i]["pois"][k]["personalidades"][y]);
                    //}

                    route.pois.Add(p);
                }
                UnofficialRoutesObjList.Add(route);
            }
            print("Routes list count JSON: " + UnofficialRoutesList["routes"].Count);
            print("Routes list count " + UnofficialRoutesObjList.Count);


        }

    }

    public void SaveRouteToJson()
    {
        RoutesCollection rc = new RoutesCollection();
        print("After SAVE Routes" + UnofficialRoutesObjList.Count);
        rc.RoutesCol = UnofficialRoutesObjList;
        print("RoutesCol" + rc.RoutesCol.Count);

        string jsonToWrite = rc.Serialize().ToString(3);
        System.IO.File.WriteAllText(unofficialRoutesListFilePath, jsonToWrite);
    }

    public void SaveRoute()
    {
        print("Before add " + UnofficialRoutesObjList.Count);
        Route createdRoute = new Route();
        if (UnofficialRoutesObjList.Count == 0)
        {
            createdRoute.id = UNOFFICIAL_ROUTE_ID + "1";
            createdRoute.code = "unnofficial1";

        }
        else
        {
            int id = UnofficialRoutesObjList.Count + 1;
            createdRoute.id = UNOFFICIAL_ROUTE_ID + id.ToString();
            createdRoute.code = "unnofficial" + id.ToString();

        }
        createdRoute.name = this.routeName;
        createdRoute.description = this.routeDescription;

        createdRoute.routeCategory = new List<string>();
        createdRoute.routeCategory.Add("Personalizado");

        createdRoute.pois = new List<Poi>();
       
        for (int k = 0; k < finalChoicesPois.Count; k++)
        {
            Poi p = new Poi();
            //JSONNode poiJson = GetJaz(finalChoicesPois[k]);
            //string jazIdent = finalChoicesPois[k];
            print(k + " " + finalChoicesPois[k]);
            p.ID = finalChoicesPois[k];
            //p.latitude = this.GetComponent<JazInformations>().GetJazLongitude(jazIdent);
            //p.longitude = this.GetComponent<JazInformations>().GetJazLongitude(jazIdent);
            //p.tipoJaz = this.GetComponent<JazInformations>().GetJazType(jazIdent);
            //p.jazImage = this.GetComponent<JazInformations>().GetJazImage(jazIdent);
            //p.description = "Lorem ipsum dolor sit amet";

            //p.personalidades = new List<string>();

            //for (int y = 0; y < poiJson["personalidades"].Count; y++)
            //{
            //    p.personalidades.Add(poiJson["personalidades"][y]["nome"]);
            //}

            createdRoute.pois.Add(p);
        }

        //codes.Add(createdRoute.code);
        print(" createdRoute.pois " + createdRoute.pois.Count);
        UnofficialRoutesObjList.Add(createdRoute);
        print("After Add " + UnofficialRoutesObjList.Count);
        SaveRouteToJson();
        this.GetComponent<SerializableRouteElements>().SaveRouteCodeToJson(createdRoute.code);
        StartCoroutine(ShowLoadingScreen());
    }


}
