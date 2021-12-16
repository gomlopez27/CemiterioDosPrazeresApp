using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterMap : MonoBehaviour
{

    [SerializeField]
    GameObject FilterPanel;
    [SerializeField]
    Button SaveFilterChoices;
    [SerializeField]
    GameObject FilterByRouteArea;
    [SerializeField]
    Button NrOfFilteredPois;
    //private JSONNode PoiList;
    private POIMapSpecifications[] poisInMap;
    private HashSet<string> FilterByRouteTags;
    private bool hasRouteTags = false;
    private Toggle firstToggle;
    private List<Toggle> toggleRouteTagsList;
    private List<string> selectedTags;

    // Start is called before the first frame update
    void Start()
    {
        //TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
        //PoiList = JSON.Parse(json.ToString());
        poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        FilterByRouteTags = new HashSet<string>();
        toggleRouteTagsList = new List<Toggle>();
        selectedTags = new List<string>();
        GetRouteFilterTags();

        SaveFilterChoices.onClick.AddListener(()=> {
            FindFilteredPois(selectedTags);
            FilterPanel.SetActive(false);
            print("selectedTags.Count: " + selectedTags.Count);

            if(selectedTags.Count==1 && selectedTags.Contains("Ver tudo"))
            {
                NrOfFilteredPois.gameObject.SetActive(false);

            }
            else
            {
                NrOfFilteredPois.gameObject.SetActive(true);

            }

        }
        );

        NrOfFilteredPois.onClick.AddListener(() =>
        {
            foreach (POIMapSpecifications poi in poisInMap)
            {
                poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.red;
            }
            NrOfFilteredPois.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (poisInMap.Length <= 0)
        {
            poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        }

        if (hasRouteTags)
        {
            BuildFilterTags();
        }

        //foreach(string s in selectedTags)
        //{
        //    print("Size: " + selectedTags.Count + "--- element: " +  selectedTags.FindIndex(item => item == s) + " value: " + s);
        //}
  
    }

    //Encontrar todos as diferentes tags de percursos
    void GetRouteFilterTags()
    {
        for (int i = 0; i < MainDataHolder.PopularPois.Count; i++)
        {
            for (int j = 0; j < MainDataHolder.PopularPois[i].RoutesCategory.Count; j++)
            {
                string tag = MainDataHolder.PopularPois[i].RoutesCategory[j];
                FilterByRouteTags.Add(tag);

            }

        }

        hasRouteTags = true;
    }

    void BuildFilterTags()
    {
        GameObject FilterItem = FilterByRouteArea.transform.GetChild(0).gameObject;

        GameObject firstTag;
        GameObject tag;

        firstTag = Instantiate(FilterItem, FilterByRouteArea.transform);
        firstToggle = firstTag.transform.Find("Toggle").GetComponent<Toggle>();
        firstToggle.isOn = true;
        Text firstToggleTxt = firstToggle.transform.Find("Label").GetComponent<Text>();
        firstToggleTxt.text = "Ver tudo";
        selectedTags.Add(firstToggleTxt.text);

        firstToggle.onValueChanged.AddListener(delegate
        {
            FirstToggleValueChanged(firstToggle, firstToggleTxt.text);
        });

        foreach (string s in FilterByRouteTags)
        {
            tag = Instantiate(FilterItem, FilterByRouteArea.transform);
            Toggle toggle = tag.transform.Find("Toggle").GetComponent<Toggle>();
            Text toggleTxt = toggle.transform.Find("Label").GetComponent<Text>();
            toggleTxt.text = s;
            toggleRouteTagsList.Add(toggle);

            toggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(toggle, toggleTxt.text);
            });

        }

        Destroy(FilterItem);
        hasRouteTags = false;
    }


    void FirstToggleValueChanged(Toggle toggle, string toggleTxt)
    {
        if (toggle.isOn)
        {
            foreach(Toggle t in toggleRouteTagsList)
            {
                t.isOn = false;
            }
            selectedTags.Clear();
            selectedTags.Add(toggleTxt);
        }
        else
        {
            selectedTags.Clear();
        }

    }    
    
    void ToggleValueChanged(Toggle toggle, string toggleTxt)
    {
        print(toggleTxt + " " + toggle.isOn);
        if (toggle.isOn)
        {
            firstToggle.isOn = false;
            selectedTags.Add(toggleTxt);
        }
        else
        {
            var value = selectedTags.Find(item => item == toggleTxt);
            selectedTags.Remove(value);
        }
    }

    void FindFilteredPois(List<string> selectedFilterTags)
    {
        foreach(string s in selectedFilterTags)
        {
            print("selected tags: " + s);
        }

        List<string> matchedIds = new List<string>();
        bool found = false;

        for (int i = 0; i < MainDataHolder.PopularPois.Count; i++)
        {
            for (int j = 0; j < MainDataHolder.PopularPois[i].RoutesCategory.Count; j++)
            {
                string poiPercursoTag = MainDataHolder.PopularPois[i].RoutesCategory[j];

                foreach (string tag in selectedFilterTags)
                {
                    //caso um POI pertenca a mais de um percurso, basta na mesma encontrar uma tag igual 
                    if (poiPercursoTag.Equals(tag))
                    {
                        matchedIds.Add(MainDataHolder.PopularPois[i].Id);
                        found = true;
                        break; 
                    }
                }
                if (found)
                {
                    break;
                }
            }
            
        }
        FilterPoisOnMap(matchedIds);
    }

    void FilterPoisOnMap(List<string> jazIds)
    {
        Text filteredPoisNr = NrOfFilteredPois.transform.GetChild(0).GetComponent<Text>();
        if (jazIds.Count > 1)
        {
            filteredPoisNr.text = jazIds.Count.ToString() + " jazigos selecionados.";
        }
        else
        {
            filteredPoisNr.text = jazIds.Count.ToString() + " jazigo selecionado.";

        }

        foreach (POIMapSpecifications poi in poisInMap)
        {
            if (jazIds.Count == 0)
            {
                poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.red;

            }
            else
            {
                foreach (string id in jazIds)
                {
                    if (id.Equals(poi.id))
                    {
                        poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                    }

                    poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.gray;

                }
            }
        }

    }
}
