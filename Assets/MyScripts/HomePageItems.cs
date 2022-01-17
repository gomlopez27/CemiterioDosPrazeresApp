using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePageItems : MonoBehaviour
{
    [SerializeField]
    GameObject InitialPagePoiList;
    [SerializeField]
    GameObject PoiListItem;
    [SerializeField]
    GameObject InitialPageRouteList;
    [SerializeField]
    GameObject RouteListItem;
    [SerializeField]
    GameObject Canvas;
    [SerializeField]
    GameObject InfoCanvas;
    [SerializeField]
    Text JazTitle;
    //private JSONNode PoisList;
    private JSONNode ThemedRoutesList;

    // Start is called before the first frame update
    void Start()
    {
        //TextAsset jsonPois = Resources.Load<TextAsset>("MapPopularPOI");
        //PoisList = JSON.Parse(jsonPois.ToString());
        TextAsset jsonRoutes = Resources.Load<TextAsset>("ThemedRoutes");
        ThemedRoutesList = JSON.Parse(jsonRoutes.ToString());
        SetupInitialPagePois();
        SetupInitialPageThemedRoutes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    HashSet<int> GenerateRandom(int max)
    {
        HashSet<int> aux = new HashSet<int>();
        while (aux.Count < 10)
        {
            int indexPoi = Random.Range(0, max - 1);
            aux.Add(indexPoi);
        }
        //int count = 0;
        //foreach(int i in aux)
        //{
        //    //print( count++ + " " + i);
        //}
        return aux;
    }

    void SetupInitialPagePois()
    {
        HashSet<int> PoiIndexes = GenerateRandom(MainDataHolder.PopularPois.Count);
        List<int> auxPois = new List<int>();
        auxPois.AddRange(PoiIndexes);
        PoiListItem.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            int indexPoi = auxPois[i];
            //print("indexPoi: " + indexPoi);
            GameObject g = Instantiate(PoiListItem, InitialPagePoiList.transform);
            Image jazImg = g.transform.Find("ImageBackground/JazImage").GetComponent<Image>();
            Text personNameTxt = g.transform.Find("ImageBackground/SeeMoreBtn/PersonName").GetComponent<Text>();
            Button seeMore = g.transform.Find("ImageBackground/SeeMoreBtn").GetComponent<Button>();
            JazTitle.text = MainDataHolder.PopularPois[indexPoi].JazType + " " + MainDataHolder.PopularPois[indexPoi].Id;
            //print("count personalities " + PoisList["pois"][indexPoi]["personalidades"].Count);
            int indexPerson = Random.Range(0, MainDataHolder.PopularPois[indexPoi].Personalities.Count);
            string personName = MainDataHolder.PopularPois[indexPoi].Personalities[indexPerson].Name;
            string url = MainDataHolder.PopularPois[indexPoi].Personalities[indexPerson].ImageUrl;

            if(url.Equals("")||url == null)
            {
                url = MainDataHolder.PopularPois[indexPoi].JazImage;
            }

            Davinci.get().load(url).into(jazImg).start();
            personNameTxt.text = personName;
            //PoisList["pois"][indexPoi]["personalidades"].Count;

            g.GetComponent<Button>().onClick.AddListener(()=>
            {
                Canvas.SetActive(false);
                InfoCanvas.SetActive(true);
                this.GetComponent<JazInformationPage>().SetSinglePersonality(MainDataHolder.PopularPois[indexPoi].Personalities[indexPerson]);
            });
            ////g.GetComponent<Button>().AddEventListener(routeId, ItemClicked);
        }

        PoiListItem.SetActive(false);

    }

  
    public void BackButton()
    {
        Canvas.SetActive(true);
        InfoCanvas.SetActive(false);
    }
    void SetupInitialPageThemedRoutes()
    {
        HashSet<int> RouteIndexes = GenerateRandom(ThemedRoutesList.Count);
        List<int> auxRoutes = new List<int>();
        auxRoutes.AddRange(RouteIndexes);
        RouteListItem.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            int indexRoute = auxRoutes[i];
            GameObject g = Instantiate(RouteListItem, InitialPageRouteList.transform);
            Text RouteNameTxt = g.transform.Find("Background/RouteName").GetComponent<Text>();
            Button seeMore = g.transform.Find("Background/SeeMoreBtn").GetComponent<Button>();

            RouteNameTxt.text = ThemedRoutesList[indexRoute]["name"];
            //PoisList["pois"][indexPoi]["personalidades"].Count;

            //g.GetComponent<Button>().onClick.AddListener(delegate ()
            //{
            //    ItemClicked(routeId, routeName, routeCode);
            //});
            ////g.GetComponent<Button>().AddEventListener(routeId, ItemClicked);
        }

        RouteListItem.SetActive(false);

    }


}
