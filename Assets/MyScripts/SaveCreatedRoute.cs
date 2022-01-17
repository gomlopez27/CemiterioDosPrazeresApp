using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SaveCreatedRoute : MonoBehaviour
{
    private const string URL_POST_ROUTE = "http://localhost/api/routes/create-route";

    public const string UNOFFICIAL_ROUTE_ID = "ur";
    public const string UNOFFICIAL_ROUTE = "unofficial";

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

    //DataHolderRouteCreation DataHolder;
    List<string> finalChoicesPois;
    List<string> finalChoicesPersonalities;
    string routeName;
    string routeDescription;
    //JSONNode AllPoiList;
    JSONNode UnofficialRoutesList;
    //RoutesCollection rc;
    List<Route> UnofficialRoutesObjList;
    //TextAsset jsonAllRoutesList;
    string unofficialRoutesListFilePath;
    bool RequestedRoute;

   

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

        //DataHolder = this.GetComponent<DataHolderRouteCreation>();
        finalChoicesPois = new List<string>();
        finalChoicesPersonalities = new List<string>();
        AddSugestionsToFinalChoices();


        //Lista com apenas os ids de todos os jazigos selecionados
        finalChoicesPois.AddRange(RouteCreationDataHolder.SelectedPois);
        finalChoicesPois.AddRange(RouteCreationDataHolder.SelectedPoisSugestions);


        //finalChoicesPersonalities.AddRange(DataHolder.SelectedPersonalities);
        print("SelectedPois " + RouteCreationDataHolder.SelectedPois.Count);
        print("SelectedPoisSugestions (hash set)" + RouteCreationDataHolder.SelectedPoisSugestions.Count);
        print("SelectedSugestions (dictionary) " + RouteCreationDataHolder.SelectedPersonalitiesFromSugestions.Count);
        print("finalChoicesPois " + finalChoicesPois.Count);
        print("SelectedPersonallitiesPerJaz " + RouteCreationDataHolder.SelectedPersonallitiesPerJaz.Count);

     
        SetUpList();
   
        //rc = new RoutesCollection();
        UnofficialRoutesObjList = new List<Route>();
        UnofficialRoutesObjList = MainDataHolder.MyUnofficialRoutes;
        //GetRouteListFromJson();
        //GetRoutesCodeListFromJson();
        //CreatedBtn.onClick.AddListener(SaveRoute);
        RequestedRoute = false;
        CreatedBtn.onClick.AddListener(delegate
        {
            StartCoroutine(this.GetComponent<LoadFromAPI>().PostRoute(SerializeChoosenPersonalities().ToString(), LoadingPanel));

        });
        
    }


    private void Update()
    {
        if (RouteCreationDataHolder.LastedCreatedRouteCode != null && !RequestedRoute)
        {
            print("ENTREI NO UPDATE");
            LoadingPanel.SetActive(true);
            StartCoroutine(this.GetComponent<LoadFromAPI>().GetRoute(Scenes));
            RequestedRoute = true;
        }
    }

    void AddSugestionsToFinalChoices()
    {
        HashSet<string> personalities;

        foreach (var elem in RouteCreationDataHolder.SelectedPersonalitiesFromSugestions)
        {
            if (!RouteCreationDataHolder.SelectedPersonallitiesPerJaz.TryGetValue(elem.Key, out personalities))
            {
                personalities = new HashSet<string>(elem.Value); //elem.value contem as person das sugestions selectionadas
                RouteCreationDataHolder.SelectedPersonallitiesPerJaz[elem.Key] = personalities;
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

        foreach (var v in RouteCreationDataHolder.SelectedPersonallitiesPerJaz)
        {
            string jazId = v.Key;
            string jazType = MainDataHolder.GetPoi(jazId).JazType;
            GameObject g = Instantiate(ListItem, SelectedPoisListArea.transform);
            g.name = "item-" + jazId;
            Toggle jazToggle = g.transform.Find("Info/Toggle").GetComponent<Toggle>();
            Button openBtn = g.transform.Find("Info/Button-Open").GetComponent<Button>();
            Button closeBtn = g.transform.Find("Info/Button-Close").GetComponent<Button>();
            GameObject PersonalityListItem = g.transform.Find("Content").transform.Find("Item").gameObject;
            HashSet<string> personalities = v.Value;
            List<string> auxPers = new List<string>();
            auxPers.AddRange(personalities);
            finalChoicesPersonalities.AddRange(personalities);

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
                    //string personName = this.GetComponent<JazInformations>().GetPersonalityName(jazId, personId);
                    string personName = MainDataHolder.GetPersonality(jazId, personId).Name;
                    GameObject gPers = Instantiate(PersonalityListItem, g.transform.Find("Content").transform);
                    //gPers.name = "pers-" + personId;
                    gPers.name = "item-" + jazId + "-" + personId;
                    gPers.transform.Find("PersName").GetComponent<Text>().text = personName;
                    Toggle persToggle = gPers.transform.Find("Toggle").GetComponent<Toggle>();
                    persToggle.isOn = true;

                    persToggle.onValueChanged.AddListener(delegate
                    {
                        ToggleValueChanged(persToggle, jazId, personId, gPers);

                    });
                }
                PersonalityListItem.SetActive(false);
            }
            else  /*Uma personalidade*/
            {
                string personId = auxPers[0];
                string personName = MainDataHolder.GetPersonality(jazId, personId).Name;
                g.transform.Find("Info/PoiName").GetComponent<Text>().text = jazType + " " + jazId + ": " + personName;
                g.name = "item-" + jazId + "-" + personId;
                jazToggle.gameObject.SetActive(true);
                openBtn.gameObject.SetActive(false);
                PersonalityListItem.SetActive(false);
                jazToggle.isOn = true;

                jazToggle.onValueChanged.AddListener(delegate
                {
                    ToggleValueChanged(jazToggle, jazId, personId, g);
                });

            }
        }
        //FavPoisListArea.SetActive(false);
        Destroy(ListItem);
    }
    
    void ToggleValueChanged(Toggle toggle, string jazId, string personId, GameObject listItem)
    {
        //var personValue = finalChoicesPois.Find(item => item == personId);
        //finalChoicesPersonalities.Remove(personValue);

        //foreach (string s in finalChoicesPersonalities)
        //{
        //    print("person id: " + s);
        //}

        print("listItem.transform.parent.name: " + listItem.transform.parent.name);
        if (!toggle.isOn && finalChoicesPois.Count > 2)
        {
            //Se o pai é a lista principal, entao e um jaz com uma unica person
            if (listItem.transform.parent.name == SelectedPoisListArea.name)
            {
                Destroy(listItem);
                var value = finalChoicesPois.Find(item => item == jazId);
                finalChoicesPois.Remove(value);
                print("finalChoicesPersonalities before " + finalChoicesPersonalities.Count);
                var personValue = finalChoicesPersonalities.Find(item => item == personId);
                finalChoicesPersonalities.Remove(personValue);
                print("finalChoicesPersonalities after " + finalChoicesPersonalities.Count);

            }
            else //Se o pai nao e' a lista principal, entao e um jaz com multiplas person
            {
                GameObject firstListItem = listItem.transform.parent.transform.GetChild(0).gameObject;

                //O childCount==2 corresponde a 1 personalidade + o item GO inativo
                if (listItem.transform.parent.transform.childCount == 2 && firstListItem.name.Equals("Item"))
                {
                    Destroy(listItem);
                    Destroy(listItem.transform.parent.parent.gameObject);
                    var value = finalChoicesPois.Find(item => item == jazId);
                    finalChoicesPois.Remove(value);
                    print("finalChoicesPersonalities before " + finalChoicesPersonalities.Count);
                    var personValue = finalChoicesPersonalities.Find(item => item == personId);
                    finalChoicesPersonalities.Remove(personValue);
                    print("finalChoicesPersonalities after " + finalChoicesPersonalities.Count);
                }
                else
                {
                    Destroy(listItem);
                    print("finalChoicesPersonalities before " + finalChoicesPersonalities.Count);
                    var personValue = finalChoicesPersonalities.Find(item => item == personId);
                    finalChoicesPersonalities.Remove(personValue);
                    print("finalChoicesPersonalities after " + finalChoicesPersonalities.Count);
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
                print("finalChoicesPersonalities before " + finalChoicesPersonalities.Count);
                var personValue = finalChoicesPersonalities.Find(item => item == personId);
                finalChoicesPersonalities.Remove(personValue);
                print("finalChoicesPersonalities after " + finalChoicesPersonalities.Count);
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

    public void SetRouteName(string name)
    {
        this.routeName = name;
    }

    public void SetRouteDescription(string descr)
    {
        this.routeDescription = descr;
    }

    JSONNode SerializeChoosenPersonalities()
    {
        var route = new JSONObject();
        route["designation"] = this.routeName;
        route["description"] = this.routeDescription;

        var iriList = new JSONArray();

        foreach (string s in finalChoicesPersonalities)
        {
            var personalityObj = new JSONObject();
            personalityObj["iri"] = s;
            iriList.Add(personalityObj);
        }
        route["personalities"] = iriList;

        return route;

    }

    //void GetRouteListFromJson()
    //{
    //    if (System.IO.File.Exists(unofficialRoutesListFilePath))
    //    {
    //        string json = File.ReadAllText(unofficialRoutesListFilePath);
    //        UnofficialRoutesList = JSON.Parse(json.ToString());

    //        for (int i = 0; i < UnofficialRoutesList["routes"].Count; i++)
    //        {
    //            Route route = new Route();
    //            route.Id = UnofficialRoutesList["routes"][i]["id"];
    //            route.Name = UnofficialRoutesList["routes"][i]["name"];
    //            route.Code = UnofficialRoutesList["routes"][i]["code"];
    //            route.Description = UnofficialRoutesList["routes"][i]["description"];

    //            route.RouteCategory = new List<string>();

    //            for (int j = 0; j < UnofficialRoutesList["routes"][i]["routeCategory"].Count; j++)
    //            {
    //                route.RouteCategory.Add(UnofficialRoutesList["routes"][i]["routeCategory"][j]);
    //            }

    //            route.Pois = new List<Poi>();
    //            for (int k = 0; k < UnofficialRoutesList["routes"][i]["pois"].Count; k++)
    //            {
    //                Poi p = new Poi();
    //                p.Id = UnofficialRoutesList["routes"][i]["pois"][k]["id"];
    //                //p.latitude = AllRoutesList["routes"][i]["pois"][k]["latitude"];
    //                //p.longitude = AllRoutesList["routes"][i]["pois"][k]["longitude"];
    //                //p.tipoJaz = AllRoutesList["routes"][i]["pois"][k]["tipoJaz"];
    //                //p.jazImage = AllRoutesList["routes"][i]["pois"][k]["jazImage"];
    //                //p.description = AllRoutesList["routes"][i]["pois"][k]["description"];

    //                //p.personalidades = new List<string>();

    //                //for (int y = 0; y < AllRoutesList["routes"][i]["pois"][k]["personalidades"].Count; y++)
    //                //{
    //                //    p.personalidades.Add(AllRoutesList["routes"][i]["pois"][k]["personalidades"][y]);
    //                //}

    //                route.Pois.Add(p);
    //            }
    //            UnofficialRoutesObjList.Add(route);
    //        }
    //        print("Routes list count JSON: " + UnofficialRoutesList["routes"].Count);
    //        print("Routes list count " + UnofficialRoutesObjList.Count);


    //    }

    //}

    //public void SaveRouteToJson()
    //{
    //    RoutesCollection rc = new RoutesCollection();
    //    print("After SAVE Routes" + UnofficialRoutesObjList.Count);
    //    rc.RoutesCol = UnofficialRoutesObjList;
    //    print("RoutesCol" + rc.RoutesCol.Count);

    //    string jsonToWrite = rc.Serialize().ToString(3);
    //    System.IO.File.WriteAllText(unofficialRoutesListFilePath, jsonToWrite);
    //}

    //public void SaveRoute()
    //{
    //    print("Before add " + UnofficialRoutesObjList.Count);
    //    Route createdRoute = new Route();
    //    if (UnofficialRoutesObjList.Count == 0)
    //    {
    //        createdRoute.Id = UNOFFICIAL_ROUTE_ID + "1";
    //        createdRoute.Code = UNOFFICIAL_ROUTE + "1";

    //    }
    //    else
    //    {
    //        int id = UnofficialRoutesObjList.Count + 1;
    //        createdRoute.Id = UNOFFICIAL_ROUTE_ID + id.ToString();
    //        createdRoute.Code = UNOFFICIAL_ROUTE + id.ToString();

    //    }
    //    createdRoute.Name = this.routeName;
    //    createdRoute.Description = this.routeDescription;

    //    //createdRoute.RouteCategory = new List<string>();
    //    //createdRoute.RouteCategory.Add("Personalizado");

    //    createdRoute.Pois = new List<Poi>();

    //    for (int k = 0; k < finalChoicesPois.Count; k++)
    //    {
    //        Poi p = new Poi();
    //        //JSONNode poiJson = GetJaz(finalChoicesPois[k]);
    //        //string jazIdent = finalChoicesPois[k];
    //        print(k + " " + finalChoicesPois[k]);
    //        p.Id = finalChoicesPois[k];
    //        //p.latitude = this.GetComponent<JazInformations>().GetJazLongitude(jazIdent);
    //        //p.longitude = this.GetComponent<JazInformations>().GetJazLongitude(jazIdent);
    //        //p.tipoJaz = this.GetComponent<JazInformations>().GetJazType(jazIdent);
    //        //p.jazImage = this.GetComponent<JazInformations>().GetJazImage(jazIdent);
    //        //p.description = "Lorem ipsum dolor sit amet";

    //        //p.personalidades = new List<string>();

    //        //for (int y = 0; y < poiJson["personalidades"].Count; y++)
    //        //{
    //        //    p.personalidades.Add(poiJson["personalidades"][y]["nome"]);
    //        //}

    //        createdRoute.Pois.Add(p);
    //    }

    //    //codes.Add(createdRoute.code);
    //    print(" createdRoute.pois " + createdRoute.Pois.Count);
    //    UnofficialRoutesObjList.Add(createdRoute);
    //    print("After Add " + UnofficialRoutesObjList.Count);
    //    //SaveRouteToJson();
    //    MainDataHolder.MyUnofficialRoutes = UnofficialRoutesObjList;

    //    this.GetComponent<SerializableDataElements>().SaveRouteCodeToJson(createdRoute.Code);
    //    StartCoroutine(ShowLoadingScreen());
    //}

    //public IEnumerator PostRoute()
    //{
    //    string json = SerializeChoosenPersonalities().ToString();
    //    //print(json);
    //    //UnityWebRequest uwr = UnityWebRequest.Post(URL_POST_ROUTE, json);
    //    var uwr = new UnityWebRequest(URL_POST_ROUTE, "POST");
    //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    uwr.SetRequestHeader("Content-Type", "application/json");
    //    yield return uwr.SendWebRequest();

    //    if (uwr.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(uwr.error);
    //        string jsonToWrite = uwr.downloadHandler.text;

    //        Debug.Log(jsonToWrite);
    //    }
    //    else
    //    {
    //        Debug.Log("SUCCESS");

    //        string jsonToWrite = uwr.downloadHandler.text;

    //        Debug.Log(jsonToWrite);
    //    }
    //}

    //IEnumerator ShowLoadingScreen()
    //{
    //    LoadingPanel.SetActive(true);
    //    yield return new WaitForSeconds(5);
    //    Scenes.LoadRouteListScene();

    //}




}
