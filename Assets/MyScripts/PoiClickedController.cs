using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class PoiClickedController : MonoBehaviour
{
    private GameObject Player;
    private GameObject InfoPanel;
    private GameObject PersonInfoPanel;
    private GameObject SinglePersonality;
    private GameObject MultiplePersonalities;
    private GameObject DirectionsPanel;
    private GameObject DirectionsPanelMin;
    private GameObject Directions;
    private GameObject ButtonsController;
    private DirectionsFactory _directionsFact;
    private GameObject ToastMsgPanel;
    private GameObject map;
    private Transform StartPoint;
    private Transform EndPoint;
    private Text routeDistanceTxt;
    private Text routeDurationTxt;
    private bool poiClicked;
    private bool takeMeThreBtnClicked;
    private bool test;
    private string jazId;
    private JSONNode FavoritesListJSON;
    private JSONNode PoisInMap;
    private List<GameObject> SpawnedPois;
    private GameObject CurrentPoi;


    private void Awake()
    {
        TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
        PoisInMap = JSON.Parse(json.ToString());
        LoadGameObjects();
    }

    void Start()
    {
        
        SpawnedPois = map.GetComponent<MySpawnOnMap>().GetSpawnedPois();

        foreach(GameObject go in SpawnedPois)
        {
         
            if(jazId.Equals(go.transform.parent.gameObject.GetComponent<POIMapSpecifications>().GetId()))
            {
                CurrentPoi = go;
                break;
            }
        }
        map.GetComponent<PanZoom>().enabled = false;
        map.GetComponent<QuadTreeCameraMovement>().enabled = false;
        SetupInfoPanel();

        //string filePath = Application.persistentDataPath + "/Favorites.json";

        //if (System.IO.File.Exists(filePath))
        //{
        //    string json = File.ReadAllText(filePath);
        //    FavoritesListJSON = JSON.Parse(json.ToString());

        //    for (int i = 0; i < FavoritesListJSON["jazIdList"].Count; i++)
        //    {
        //        string id = FavoritesListJSON["jazIdList"][i];

        //        if (jazId.Equals(id))
        //        {
        //            removeFromFavoritesBtn.gameObject.SetActive(true);
        //            addToFavoritesBtn.gameObject.SetActive(false);
        //            print("CHANGED");
        //            break;
        //        }
        //    }
        //}

        if (map == null || Player == null || InfoPanel == null || DirectionsPanel == null
            || ToastMsgPanel == null)
        {
            print("Null game object");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (takeMeThreBtnClicked)
        {
            if (_directionsFact.GetDirectionsResponse() != null)
            {
                SetUpDirectionsPanel(_directionsFact);
                //takeMeThreBtnClicked = false;
            }
        }

    }

    public void SetJazId(string id)
    {
        this.jazId = id;
    }

    public void SetupInfoPanel()
    {
        InfoPanel.SetActive(true);
        Text TextId = InfoPanel.transform.Find("JazId").GetComponent<Text>();
        Image headerImage = InfoPanel.transform.Find("HeaderImageGO").GetComponent<Image>();
        Button directionsToBtn = InfoPanel.transform.Find("Buttons/DirectionsBtn").GetComponent<Button>();
        Button addToFavoritesBtn = InfoPanel.transform.Find("Buttons/AddToFavsBtn").GetComponent<Button>();
        Button removeFromFavoritesBtn = InfoPanel.transform.Find("Buttons/RemoveFromFavsBtn").GetComponent<Button>();

        directionsToBtn.onClick.AddListener(()=> {
            TakeMeThere();
        });
        StartPoint = Player.transform;
        //EndPoint = this.transform;
        EndPoint = CurrentPoi.transform;

        if (DirectionsPanel.activeSelf || DirectionsPanelMin.activeSelf)
        {
            StartCoroutine(TurnOffDirections());
        }


        DestroyPersonalitiesList();

        for (int i = 0; i < PoisInMap["pois"].Count; i++)
        {
            string id = PoisInMap["pois"][i]["ID"].ToString();
            if (id.Equals(jazId))
            {
                TextId.text = PoisInMap["pois"][i]["tipoJaz"] + " " + jazId;
                string imageUrl = PoisInMap["pois"][i]["jazImage"];
                Davinci.get().load(imageUrl).into(headerImage).start();

                /*Multiplas Personalidades*/
                if (PoisInMap["pois"][i]["personalidades"].Count > 1)
                {
                    SinglePersonality.SetActive(false);
                    MultiplePersonalities.SetActive(true);
                    addToFavoritesBtn.gameObject.SetActive(false);
                    removeFromFavoritesBtn.gameObject.SetActive(false);

                    SetMultiplePersonalitiesList(PoisInMap["pois"][i]["personalidades"]);
                }
                else  /*Uma personalidade*/
                {
                    SinglePersonality.SetActive(true);
                    MultiplePersonalities.SetActive(false);
                    SetSinglePersonality(PoisInMap["pois"][i]["personalidades"][0]);
                    string personId = PoisInMap["pois"][i]["personalidades"][0]["uriId"];

                    if(ButtonsController.GetComponent<FavoritePoisMap>().isFavorite(jazId, personId))
                    {
                        addToFavoritesBtn.gameObject.SetActive(false);
                        removeFromFavoritesBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        addToFavoritesBtn.gameObject.SetActive(true);
                        removeFromFavoritesBtn.gameObject.SetActive(false);
                    }
                    addToFavoritesBtn.onClick.AddListener(() =>
                    {
                        //ButtonsController.GetComponent<FavoritePoisMap>().SaveJazAsFavorite(jazId);
                        ButtonsController.GetComponent<FavoritePoisMap>().AddPersonalityToFavs(jazId, personId);
                        addToFavoritesBtn.gameObject.SetActive(false);
                        removeFromFavoritesBtn.gameObject.SetActive(true);
                    });

                    removeFromFavoritesBtn.onClick.AddListener(() =>
                    {
                        //ButtonsController.GetComponent<FavoritePoisMap>().RemoveJazAsFavorite(jazId);
                        ButtonsController.GetComponent<FavoritePoisMap>().RemovePersonalityFromFav(jazId, personId);
                        removeFromFavoritesBtn.gameObject.SetActive(false);
                        addToFavoritesBtn.gameObject.SetActive(true);
                    });
                }
                
                break;
            }
        }
    }

 

    void SetSinglePersonality(JSONNode Personality)
    {

        Image personalityImage = InfoPanel.transform.Find("SinglePersonality/PersonImage").GetComponent<Image>();
        Text personalityName = InfoPanel.transform.Find("SinglePersonality/PersonName").GetComponent<Text>();
        Text personalityBio = InfoPanel.transform.Find("SinglePersonality/BioText").GetComponent<Text>();
        Button SeeMoreBtn = InfoPanel.transform.Find("SinglePersonality/VerMaisBtn").GetComponent<Button>();
        Davinci.get().load(Personality["imageURL"]).into(personalityImage).start();
        personalityName.text = Personality["nome"];
        personalityBio.text = Personality["description"];
        SeeMoreBtn.onClick.AddListener(()=> {
            PersonInfoPanel.SetActive(true);
            SetMoreInfoPersonality(Personality);
        });
    }

    void SetMultiplePersonalitiesList(JSONNode PersonalitiesList)
    {
        GameObject ListArea = MultiplePersonalities.transform.Find("ScrollArea/Content").gameObject;
        GameObject PersonalityItem = ListArea.transform.GetChild(0).gameObject;
        PersonalityItem.SetActive(true);
    

        for (int i = 0; i < PersonalitiesList.Count; i++)
        {
            string personId = PersonalitiesList[i]["uriId"];
            string personName = PersonalitiesList[i]["nome"];
            GameObject g = Instantiate(PersonalityItem, ListArea.transform);
            g.name = "person-" + personId;
            g.transform.Find("PersonName").GetComponent<Text>().text = personName;
            Button AddFav = g.transform.Find("FavPersBtn").GetComponent<Button>();
            Button RemoveFav = g.transform.Find("NotFavPersBtn").GetComponent<Button>();
            Button SeeMore = g.transform.Find("MoreBtn").GetComponent<Button>();
            
            if (ButtonsController.GetComponent<FavoritePoisMap>().isFavorite(jazId, personId))
            {
                AddFav.gameObject.SetActive(false);
                RemoveFav.gameObject.SetActive(true);
            }
            else
            {
                AddFav.gameObject.SetActive(true);
                RemoveFav.gameObject.SetActive(false);
            }

            AddFav.onClick.AddListener(()=> {
                AddFav.gameObject.SetActive(false);
                RemoveFav.gameObject.SetActive(true);
                ButtonsController.GetComponent<FavoritePoisMap>().AddPersonalityToFavs(jazId, personId);

            });

            RemoveFav.onClick.AddListener(() => {
                RemoveFav.gameObject.SetActive(false);
                AddFav.gameObject.SetActive(true);
                ButtonsController.GetComponent<FavoritePoisMap>().RemovePersonalityFromFav(jazId, personId);

            });
            
            JSONNode Personality = PersonalitiesList[i];
            SeeMore.onClick.AddListener(() => {
                PersonInfoPanel.gameObject.SetActive(true);
                print("on click" + Personality["nome"]);

                SetMoreInfoPersonality(Personality);
            });
        }
        //Destroy(PersonalityItem);
        PersonalityItem.SetActive(false);
    }

    public void DestroyPersonalitiesList()
    {
        GameObject ListArea = MultiplePersonalities.transform.Find("ScrollArea/Content").gameObject;

        if (ListArea.transform.childCount > 1)
        {
            for (int i = 1; i < ListArea.transform.childCount; i++)
            {
                print(ListArea.transform.GetChild(i).gameObject.name);
                Destroy(ListArea.transform.GetChild(i).gameObject);
            }
        }
    }

    void SetMoreInfoPersonality(JSONNode Personality)
    {
        Image personalityImage = PersonInfoPanel.transform.Find("Image").GetComponent<Image>();
        Text personalityName = PersonInfoPanel.transform.Find("PersonName").GetComponent<Text>();
        Text personalityBio = PersonInfoPanel.transform.Find("BioText").GetComponent<Text>();
        Davinci.get().load(Personality["imageURL"]).into(personalityImage).start();
        personalityName.text = Personality["nome"];
        personalityBio.text = Personality["description"];
    }
   
    public void TakeMeThere()
    {
     
        Directions.SetActive(true);
       
        _directionsFact.enabled = true;
        _directionsFact._waypoints[0] = StartPoint;
        _directionsFact._waypoints[1] = EndPoint;

        _directionsFact.Start();
        InfoPanel.SetActive(false);
        map.GetComponent<PanZoom>().enabled = true;
       // yield return new WaitForSeconds(0.5f);
        DirectionsPanel.SetActive(true);
        takeMeThreBtnClicked = true;
    }

    /*Se um POI foi clicado, enquanto tinha as direcoes para outro POI, desligar direc��es anteriores*/
    public IEnumerator TurnOffDirections()
    {
        if (DirectionsPanel.activeSelf)
            DirectionsPanel.SetActive(false);
        if (DirectionsPanelMin.activeSelf)
            DirectionsPanelMin.SetActive(false);

        _directionsFact.CancelDirections();
        _directionsFact.enabled = false;
        Directions.SetActive(false);

        ToastMsgPanel.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        ToastMsgPanel.SetActive(false);

        //finishedDirections = true;
        takeMeThreBtnClicked = false;
    }

    void SetUpDirectionsPanel(DirectionsFactory _directionsFact)
    {
        //DirectionsPanel.SetActive(true);
        //print("GetDirectionsResponse code: " + _directionsFact.GetDirectionsResponse().Code);

        Text jazId = DirectionsPanel.transform.Find("ToTxt-Value").GetComponent<Text>();
        jazId.text = this.jazId;

        routeDistanceTxt.text = _directionsFact.GetRouteDistance().ToString() + " m";
        routeDurationTxt.text = _directionsFact.GetRouteDuration().ToString() + " seg";


    }




    void LoadGameObjects()
    {
        map = GameObject.Find("LocationBasedGame/Map");
        Player = GameObject.Find("LocationBasedGame/Player");
        InfoPanel = GameObject.Find("Canvas/SafeArea/InfoPanel");
        DirectionsPanel = GameObject.Find("Canvas/SafeArea/DirectionsPanel");
        DirectionsPanelMin = GameObject.Find("Canvas/SafeArea/DirectionsPanelMinimized");
        ToastMsgPanel = GameObject.Find("Canvas/SafeArea/ToastMsgPanel");
        _directionsFact = FindObjectsOfType<DirectionsFactory>(true)[0];
        Directions = _directionsFact.gameObject;
        routeDistanceTxt = DirectionsPanel.transform.Find("DistanceTxt-Value").GetComponent<Text>();
        routeDurationTxt = DirectionsPanel.transform.Find("DurationTxt-Value").GetComponent<Text>();
        ButtonsController = GameObject.Find("ButtonsController");

        PersonInfoPanel = GameObject.Find("Canvas/SafeArea/PersonInfoPanel");
        SinglePersonality = GameObject.Find("Canvas/SafeArea/InfoPanel/SinglePersonality");
        MultiplePersonalities = GameObject.Find("Canvas/SafeArea/InfoPanel/MultiplePersonalities");
}
}