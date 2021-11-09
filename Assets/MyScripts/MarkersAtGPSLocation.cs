using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using ARLocation;
using UnityEngine.Networking;
using System;

public class MarkersAtGPSLocation : MonoBehaviour
{
    //Constants
    public const string NEAR = "Já deve conseguir ver este jazigo!"; //inferior a 10 m
    public const string CLOSE = "Encontra-se perto deste jazigo, mas não o suficiente para o ver!"; //entre 10 e 20m
    public const string FAR = "Encontra-se demasiado longe deste jazigo para o conseguir ver!"; //maior que 20
    
    [SerializeField] GameObject POIPrefab;
    [SerializeField] GameObject InitialPanel;
    [SerializeField] GameObject MoreInfoPanel;
    [SerializeField] GameObject RadarPanel;
    [SerializeField] GameObject ArSession;
    [SerializeField] GameObject ArSessionOrigin;
    //[SerializeField] Image imageUI;
    //[SerializeField] Text nameUI;
    //[SerializeField] Text descriptionUI;
    //[SerializeField] Text titleMoreInfoPage;
    [SerializeField] Text numberOfCurrentPOIs;
    [SerializeField] Slider slider;
    [SerializeField] Text sliderText;
    [SerializeField] Text JazClickedTitle;
    [SerializeField] GameObject OnePersonalityPage;
    [SerializeField] GameObject MultiplePersonalitiesPage;
    [SerializeField] GameObject Loading;
    //Private Variables
    private Text jazID;
    private Text jazLoc;
    private Text nomeJaz;
    private Text percursoJaz;
    private Image fotoPOI; 
    private JSONNode PoiListData;
    private int countPhotos;
    private bool loadedPhotos;
    private bool poiCreated;
    private bool ListenersAdded;
    private PlaceAtLocation[] allPOIs;
    private string clickedId;
    private int frameCountRange;
    private int sliderValue;
    private int savedSliderValue;
    private bool hasUsedSlider;
    private bool previousSliderClicked;
    private bool currentSliderClicked;

    private void Awake()
    {
        //TextAsset json = Resources.Load<TextAsset>("POIData");
        TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
        PoiListData = JSON.Parse(json.ToString());
        //POIPhotosArray = new Sprite[PoiListData["pois"].Count];
        //LoadPOIPhotos();
    }

    void Start()
    {
        CreatePOIAtLocation();

        allPOIs = FindObjectsOfType<PlaceAtLocation>(true);

        slider.onValueChanged.AddListener((v) => {
            sliderText.text = v.ToString();
            sliderValue = (int)v;

        });

    }

    void Update()
    {

        if (poiCreated && !ListenersAdded)
        {
            AddListenersToPois();
        }

        frameCountRange++;

        if (ListenersAdded && frameCountRange % 70 == 0)
        {
            
            foreach (PlaceAtLocation poi in allPOIs)
            {
                DistanceFromPOI(poi);
            }

            if (hasUsedSlider)
            {
                ChangeRangePOI(savedSliderValue);
            }
        }

    }


    public void CreatePOIAtLocation()
    {
        for (int i = 0; i < PoiListData["pois"].Count; i++)
        {
            //GameObject POIObject = new GameObject("GPSStageObject-"+ PoiListData["pois"][i]["ID"]);
            GameObject POIObject = new GameObject(PoiListData["pois"][i]["ID"]); //TODO: mudar para id completo
           
            var loc = new Location()
            {
                //Latitude = PoiListData["pois"][i]["gpsPosition"]["latitude"],
                //Longitude = PoiListData["pois"][i]["gpsPosition"]["longitude"],

                Latitude = PoiListData["pois"][i]["latitude"],
                Longitude = PoiListData["pois"][i]["longitude"],
                Altitude = 2,
                AltitudeMode = AltitudeMode.GroundRelative
            };

            var opts = new PlaceAtLocation.PlaceAtOptions()
            {
                HideObjectUntilItIsPlaced = true,
                MaxNumberOfLocationUpdates = 2,
                MovementSmoothing = 0.1f,
                UseMovingAverage = false
            };

            PlaceAtLocation.AddPlaceAtComponent(POIObject, loc, opts);

            GameObject thisPOI = Instantiate(POIPrefab, POIObject.transform);

            //Find the correct text to change
            jazID = thisPOI.transform.Find("Canvas/Panel/ContentPanel/TopContentPanel/TitleContentPanel/jazigoTextId").GetComponent<Text>();
            jazLoc = thisPOI.transform.Find("Canvas/Panel/ContentPanel/TopContentPanel/TitleContentPanel/LocalText").GetComponent<Text>();
            nomeJaz = thisPOI.transform.Find("Canvas/Panel/ContentPanel/BottomContentPanel/NomePessoa").GetComponent<Text>();
            percursoJaz = thisPOI.transform.Find("Canvas/Panel/ContentPanel/BottomContentPanel/NomePercurso").GetComponent<Text>();
            fotoPOI = thisPOI.transform.Find("Canvas/Panel/ContentPanel/TopContentPanel/photo").GetComponent<Image>();

            //Change the text
            jazID.text = PoiListData["pois"][i]["ID"];
            jazLoc.text = PoiListData["pois"][i]["jazLocation"];

            print(PoiListData["pois"][i]["ID"] + "; number of ppl: " + PoiListData["pois"][i]["personalidades"].Count);
            print("person: " + PoiListData["pois"][i]["personalidades"][0]["nome"]);
            
            if (PoiListData["pois"][i]["personalidades"].Count > 1)
            {
                nomeJaz.text = "Múltiplas pessoas encontram-se sepultadas neste jazigo.";
                Davinci.get().load(PoiListData["pois"][i]["imageIconPlaceholder"]).setCached(true).into(fotoPOI).start();

            }
            else
            {
                nomeJaz.text = PoiListData["pois"][i]["personalidades"][0]["nome"];
                Davinci.get().load(PoiListData["pois"][i]["personalidades"][0]["imageURL"]).setCached(true).into(fotoPOI).start();

            }

            //percursoJaz.text = PoiListData["pois"][i]["route"];


        }

        poiCreated = true;
    }

    public void AddListenersToPois()
    {
       foreach(PlaceAtLocation poi in allPOIs)
        {
            GameObject poiPrefab = poi.gameObject.transform.GetChild(0).gameObject;
            GameObject POIPanel = poiPrefab.transform.Find("Canvas/Panel").gameObject;
            string jazId = poiPrefab.transform.Find("Canvas/Panel/ContentPanel/TopContentPanel/TitleContentPanel/jazigoTextId").GetComponent<Text>().text;

            //print(poi.gameObject.name);
            Button moreInfo = POIPanel.AddComponent<Button>();

            moreInfo.onClick.AddListener(() => {
                clickedId = poi.gameObject.name;

                // clickedId = jazId;
                LoadMoreInfo(clickedId);
            });
          
        }

        ListenersAdded = true;

    }

    public void LoadPOIPhotos()
    {
        for (int i = 0; i < PoiListData["pois"].Count; i++)
        {
            StartCoroutine(GetTexture(PoiListData["pois"][i]["PoiListData"], i));
            countPhotos = i;
        }

        if (countPhotos == PoiListData["pois"].Count - 1)
            loadedPhotos = true;
    }

    IEnumerator GetTexture(string url, int index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            var myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(myTexture,
            new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);

            //if(index != downloadFlag)
                //POIPhotosArray[index] = sprite;

        }
    }

    //public void LoadData()
    //{
    //    for (int i = 0; i < PoiListData["pois"].Count; i++)
    //    {
    //        if (PoiListData["pois"][i]["ID"].Equals(clickedId))
    //        {
    //            titleMoreInfoPage.text = "Jazigo nº" + PoiListData["pois"][i]["ID"];
    //            //nameUI.text = PoiListData["pois"][i]["personName"];
    //            if (PoiListData["pois"][i]["personalidades"].Count > 1)
    //            {
    //                nameUI.text = "Múltiplas pessoas encontram-se sepultadas neste jazigo.";
    //                descriptionUI.text = PoiListData["pois"][i]["personalidades"][0]["description"];
    //            }
    //            else
    //            {
    //                nameUI.text = PoiListData["pois"][i]["personalidades"][0]["nome"];
    //                descriptionUI.text = PoiListData["pois"][i]["moreInfo"]["description"];
    //            }
    //            //Davinci.get().load(PoiListData["pois"][i]["moreInfo"]["imageURL"]).setCached(true).into(imageUI).start();
    //            Davinci.get().load(PoiListData["pois"][i]["jazImage"]).setCached(true).into(imageUI).start();

    //        }
            
    //    }
    

    //}

    public void ChangeRangePOI(int stepValue)
    {
        int count = 0;

        foreach (PlaceAtLocation poi in allPOIs)
        {
            print(poi.gameObject.name + ": " + poi.SceneDistance);

            if (poi.SceneDistance < stepValue)
            {
                poi.gameObject.SetActive(true);
                Debug.Log("activate: " + poi.gameObject.name + " " + poi.Location.Latitude + ", " + poi.Location.Latitude);
                count++;
            }
            else
            {
                poi.gameObject.SetActive(false);
                Debug.Log("deactivate: " + poi.gameObject.name + " " + poi.Location.Latitude + ", " + poi.Location.Latitude);

            }
        }

        numberOfCurrentPOIs.text = count.ToString();
        Loading.SetActive(false);

    }

    public void SavePoiRange()
    {
        savedSliderValue = sliderValue;
        hasUsedSlider = true;
        Loading.SetActive(true);

        //previousSliderClicked = currentSliderClicked;
        //currentSliderClicked = !currentSliderClicked;
        //numberOfCurrentPOIs.text = countPOIWithinDistance.ToString();

        //ChangeRangePOI(sliderValue);
    }

    public void DistanceFromPOI(PlaceAtLocation poi)
    {
        GameObject poiPrefab = poi.gameObject.transform.GetChild(0).gameObject;
        Text percurso = poiPrefab.transform.Find("Canvas/Panel/ContentPanel/BottomContentPanel/NomePercurso").GetComponent<Text>();
        double distance = poi.SceneDistance;

        if(distance <= 10)
        {
            percurso.text = NEAR;
        }
        else if(distance > 10 && distance < 21)
        {
            percurso.text = CLOSE;
        }
        else
        {
            percurso.text = FAR;

        }

        //percurso.text = poi.SceneDistance.ToString("0.00") + "metros";
    }

    public void ShowAllPOI()
    {
        foreach (PlaceAtLocation poi in allPOIs)
        {
            poi.gameObject.SetActive(true);
        }

        slider.value = slider.minValue;
        sliderText.text = "?";
        numberOfCurrentPOIs.text = allPOIs.Length.ToString();
        hasUsedSlider = false;
        RadarPanel.SetActive(true);
    }


    public void LoadMoreInfo(string jazIdClicked)
    {
        print("load: "+jazIdClicked);
        InitialPanel.SetActive(false);
        MoreInfoPanel.SetActive(true);
        ArSession.SetActive(false);
        ArSessionOrigin.SetActive(false);
        JSONNode jaz = this.GetComponent<JazInformations>().GetJaz(jazIdClicked); //TODO: change back

       
        //JSONNode jaz = this.GetComponent<JazInformations>().GetJaz("1500");


        JazClickedTitle.text = jaz["tipoJaz"] + " " + jazIdClicked;

        if (jaz["personalidades"].Count > 1)
        {
            this.GetComponent<JazInformationPage>().SetMultiplePersonalitiesList(jaz["jazImage"], jaz["personalidades"]);
        }
        else
        {
            this.GetComponent<JazInformationPage>().SetSinglePersonality(jaz["personalidades"][0]);
        }
        //dataLoaded = true;

    }

    public void BackButtonFromSinglePerson()
    {
        if (MultiplePersonalitiesPage.activeInHierarchy)
        {
            OnePersonalityPage.SetActive(false);
        }
        else
        {
            BackButton();
        }

    }

    public void BackButton()
    {
        InitialPanel.SetActive(true);
        MoreInfoPanel.SetActive(false);
        ArSession.SetActive(true);
        ArSessionOrigin.SetActive(true);
    }

}
