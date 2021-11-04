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
    public const string Near = "J� deve conseguir ver este jazigo!"; //inferior a 10 m
    public const string Close = "Encontra-se perto deste jazigo, mas n�o o suficiente para o ver!"; //entre 10 e 20m
    public const string Far = "Encontra-se demasiado longe deste jazigo para o conseguir ver!"; //maior que 20
    
    //Public Variables
    public GameObject POIPrefab;
    public GameObject InitialPanel;
    public GameObject MoreInfoPanel;
    public GameObject RadarPanel;
    public GameObject ArSession;
    public GameObject ArSessionOrigin;
    public Image imageUI;
    public Text nameUI;
    public Text descriptionUI;
    public Text titleMoreInfoPage;
    public Text numberOfCurrentPOIs;
    public Slider slider;
    public Text sliderText;
    
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
            GameObject POIObject = new GameObject(PoiListData["pois"][i]["ID"]);
           
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
                nomeJaz.text = "M�ltiplas pessoas encontram-se sepultadas neste jazigo.";
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

            print(poi.gameObject.name);
            Button moreInfo = POIPanel.AddComponent<Button>();
            moreInfo.onClick.AddListener(() => {
                MoreInfoPanel.SetActive(true);
                InitialPanel.SetActive(false);
                ArSession.SetActive(false);
                ArSessionOrigin.SetActive(false);
               // clickedId = jazId;
                clickedId = poi.gameObject.name;
                LoadData();
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

    public void LoadData()
    {
        for (int i = 0; i < PoiListData["pois"].Count; i++)
        {
            if (PoiListData["pois"][i]["ID"].Equals(clickedId))
            {
                titleMoreInfoPage.text = "Jazigo n�" + PoiListData["pois"][i]["ID"];
                //nameUI.text = PoiListData["pois"][i]["personName"];
                if (PoiListData["pois"][i]["personalidades"].Count > 1)
                {
                    nameUI.text = "M�ltiplas pessoas encontram-se sepultadas neste jazigo.";
                    descriptionUI.text = PoiListData["pois"][i]["personalidades"][0]["description"];
                }
                else
                {
                    nameUI.text = PoiListData["pois"][i]["personalidades"][0]["nome"];
                    descriptionUI.text = PoiListData["pois"][i]["moreInfo"]["description"];
                }
                //Davinci.get().load(PoiListData["pois"][i]["moreInfo"]["imageURL"]).setCached(true).into(imageUI).start();
                Davinci.get().load(PoiListData["pois"][i]["jazImage"]).setCached(true).into(imageUI).start();

            }
            
        }
    

    }

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

    }

    public void SavePoiRange()
    {
        savedSliderValue = sliderValue;
        hasUsedSlider = true;
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
            percurso.text = Near;
        }
        else if(distance > 10 && distance < 21)
        {
            percurso.text = Close;
        }
        else
        {
            percurso.text = Far;

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
    }

}