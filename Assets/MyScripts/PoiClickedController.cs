using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
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
    //private JSONNode FavoritesListJSON;
    //private JSONNode PoisInMap;
    private List<GameObject> SpawnedPois;
    private GameObject CurrentPoi;


    private void Awake()
    {
        //TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
        //PoisInMap = JSON.Parse(json.ToString());
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

        for (int i = 0; i < MainDataHolder.PopularPois.Count; i++)
        {
            string id = MainDataHolder.PopularPois[i].Id;
            if (id.Equals(jazId))
            {
                TextId.text = MainDataHolder.PopularPois[i].JazType + " " + jazId;
                string imageUrl = MainDataHolder.PopularPois[i].JazImage;
                Davinci.get().load(imageUrl).into(headerImage).start();

                /*Multiplas Personalidades*/
                if (MainDataHolder.PopularPois[i].Personalities.Count > 1)
                {
                    SinglePersonality.SetActive(false);
                    MultiplePersonalities.SetActive(true);
                    addToFavoritesBtn.gameObject.SetActive(false);
                    removeFromFavoritesBtn.gameObject.SetActive(false);

                    SetMultiplePersonalitiesList(jazId, MainDataHolder.PopularPois[i].Personalities);
                }
                else  /*Uma personalidade*/
                {
                    SinglePersonality.SetActive(true);
                    MultiplePersonalities.SetActive(false);
                    SetSinglePersonality(jazId, MainDataHolder.PopularPois[i].Personalities[0]);
                    string personId = MainDataHolder.PopularPois[i].Personalities[0].UriId;

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

    /*Info panel for POI with only one personality */
    void SetSinglePersonality(string jazId, Personality personality)
    {
        Image personalityImage = SinglePersonality.transform.Find("PersonImage").GetComponent<Image>();
        Text personalityName = SinglePersonality.transform.Find("PersonName").GetComponent<Text>();
        Text personalityBio = SinglePersonality.transform.Find("BioText").GetComponent<Text>();
        Button SeeMoreBtn = InfoPanel.transform.Find("ButtonBackground/VerMaisBtn").GetComponent<Button>();
        Text birthDate = SinglePersonality.transform.Find("Dates/birthDateValue").GetComponent<Text>();
        Text deathDate = SinglePersonality.transform.Find("Dates/deathDateValue").GetComponent<Text>();
        Davinci.get().load(personality.ImageUrl).into(personalityImage).start();
        personalityName.text = personality.Name;
        birthDate.text = PrintDateInText(personality.BirthDate);
        deathDate.text = PrintDateInText(personality.DeathDate);
        personalityBio.text = personality.Biography;

        SeeMoreBtn.onClick.AddListener(()=> {
            PersonInfoPanel.SetActive(true);
            SetMoreInfoPersonality(jazId, personality);
        });
    }

    /*Info panel for POI with multiple personalities */
    void SetMultiplePersonalitiesList(string jazId, List<Personality> PersonalitiesList)
    {
        GameObject ListArea = MultiplePersonalities.transform.Find("ScrollArea/Content").gameObject;
        GameObject PersonalityItem = ListArea.transform.GetChild(0).gameObject;
        PersonalityItem.SetActive(true);
    

        for (int i = 0; i < PersonalitiesList.Count; i++)
        {
            string personId = PersonalitiesList[i].UriId;
            string personName = PersonalitiesList[i].Name;
            GameObject g = Instantiate(PersonalityItem, ListArea.transform);
            g.name = "person-" + personId;
            g.transform.Find("MoreBtn/PersonName").GetComponent<Text>().text = personName;
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

            Personality personality = PersonalitiesList[i];
            SeeMore.onClick.AddListener(() => {
                PersonInfoPanel.gameObject.SetActive(true);
                print("on click" + personality.Name);

                SetMoreInfoPersonality(jazId,personality);
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

    /*More info panel of one personality, after clicking in a button */
    void SetMoreInfoPersonality(string jazId, Personality personality)
    {
        Text personalityName = PersonInfoPanel.transform.Find("TopBar/PersonName").GetComponent<Text>();
        Image personalityImage = PersonInfoPanel.transform.Find("InfoArea/Image").GetComponent<Image>();
        Text birthDate = PersonInfoPanel.transform.Find("InfoArea/Dates/birthDateValue").GetComponent<Text>();
        Text deathDate = PersonInfoPanel.transform.Find("InfoArea/Dates/deathDateValue").GetComponent<Text>();
        Button AddFav = PersonInfoPanel.transform.Find("InfoArea/Buttons/AddToFavsBtn").GetComponent<Button>();
        Button RemoveFav = PersonInfoPanel.transform.Find("InfoArea/Buttons/RemoveToFavsBtn").GetComponent<Button>();
        Text personalityBio = PersonInfoPanel.transform.Find("TextArea/Scroll View/Viewport/Content").GetComponent<Text>();

        Davinci.get().load(personality.ImageUrl).into(personalityImage).start();
        personalityName.text = personality.Name;
        birthDate.text = PrintDateInText(personality.BirthDate);
        deathDate.text = PrintDateInText(personality.DeathDate);
        personalityBio.text = personality.Biography;
        string personId = personality.UriId;

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

        AddFav.onClick.AddListener(() => {
            AddFav.gameObject.SetActive(false);
            RemoveFav.gameObject.SetActive(true);
            ButtonsController.GetComponent<FavoritePoisMap>().AddPersonalityToFavs(jazId, personId);

        });

        RemoveFav.onClick.AddListener(() => {
            RemoveFav.gameObject.SetActive(false);
            AddFav.gameObject.SetActive(true);
            ButtonsController.GetComponent<FavoritePoisMap>().RemovePersonalityFromFav(jazId, personId);

        });
    }

    private string PrintDateInText(List<int> date)
    {
        string fullDate, y, m, d = "";
        int year = date[0];
        int month = date[1];
        int day = date[2];

        y = year.ToString();
        if(month < 10 || day < 10)
        {
            m = "0" + month.ToString();
            d = "0" + day.ToString();
        }
        else
        {
            m = month.ToString();
            d = day.ToString();
        }

        fullDate = d + "/" + m + "/" + y;

        return fullDate;
    }
   
    public void TakeMeThere()
    {
     
        Directions.SetActive(true);
       
        _directionsFact.enabled = true;
        _directionsFact._waypoints[0] = StartPoint;
        _directionsFact._waypoints[1] = EndPoint;

        _directionsFact.Start();
        InfoPanel.SetActive(false);
   
       // yield return new WaitForSeconds(0.5f);
        DirectionsPanel.SetActive(true);
        takeMeThreBtnClicked = true;
    }

    /*Se um POI foi clicado, enquanto tinha as direcoes para outro POI, desligar direcções anteriores*/
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
