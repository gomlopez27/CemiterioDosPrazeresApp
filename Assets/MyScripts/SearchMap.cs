using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SearchMap : MonoBehaviour
{
    [SerializeField]
    InputField inputField;
    [SerializeField]
    GameObject SearchPanel;
    [SerializeField]
    Button searchBtn;
    [SerializeField]
    GameObject NotFoundTxt;
    [SerializeField]
    InputField searchInputField;
    [SerializeField]
    Button closeSearchBtn;
    private TouchScreenKeyboard keyboard;

    private string searchInput;
    private JSONNode PoiList;
    private POIMapSpecifications[] poisInMap;
    private bool NotFound;
    // Start is called before the first frame update
    void Start()
    {

        TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
        PoiList = JSON.Parse(json.ToString());
        poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        NotFound = false;
        searchBtn.onClick.AddListener(DoSearch);
    }

    // Update is called once per frame
    void Update()
    {
        while(poisInMap.Length <= 0)
        {
            print(poisInMap.Length);
            poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        }

        if (NotFound)
        {
            StartCoroutine(ShowNotFound());
            NotFound = false;
        }
        //if (Input.GetKeyDown("space"))
        //{
        //    print("Space key was pressed");
        //}

        //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) 
        //    /*|| inputField.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done*/)
        //{
        //    print("enter key was pressed");
        //}

    }

    public void ReadStringInput(string s)
    {
        searchInput = s;
    }  
    
    public void ClearStringInput()
    {
        searchInput = "";
    }

    IEnumerator ShowNotFound()
    {
        NotFoundTxt.SetActive(true);
        yield return new WaitForSeconds(3);
        NotFoundTxt.SetActive(false);
    }
    public void DoSearch()
    {
        //if (inputField.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done)
        //{ }
        if (searchInput != null)
        {
            //o search input é um numero, logo é um id, caso contrario é uma string e é um nome
            if (Int32.TryParse(searchInput, out int jazId))
            {
                for (int i = 0; i < PoiList["pois"].Count; i++)
                {
                    string id = PoiList["pois"][i]["ID"];
                    if (id.Equals(searchInput))
                    {
                        HashSet<string> aux = new HashSet<string>();
                        aux.Add(id);
                        FindPoiInMap(aux);
                        break;
                    }
                    else
                    {
                        if (i == PoiList["pois"].Count - 1)
                        {
                            NotFound = true;
                        }
                    }                  
                }
            }
            else
            {
                //vai procurar o nome introduzido na lista de personalidades e se encontrar faz match do id do jazigo
                HashSet<string> poisIdMatched = new HashSet<string>();
                //bool found = false;
                for (int i = 0; i < PoiList["pois"].Count; i++)
                {
                    for (int j = 0; j < PoiList["pois"][i]["personalidades"].Count; j++)
                    {
                        string name = PoiList["pois"][i]["personalidades"][j]["nome"];
                        string nameClean = MainDataHolder.RemoveAccents(name).ToLower();
                        string searchInputClean = MainDataHolder.RemoveAccents(searchInput).ToLower();
                        print("nameClean: " + nameClean);

                        if (nameClean.Equals(searchInputClean) || nameClean.Contains(searchInputClean))
                        {
                            print("searchInputClean: " + searchInputClean);
                            string poiIdMatched = PoiList["pois"][i]["ID"];
                            poisIdMatched.Add(poiIdMatched);
                            //found = true;
                            break;
                        }
                    }
                }

                if (poisIdMatched.Count > 0)
                {
                    print(poisIdMatched.Count);
                    foreach(string s in poisIdMatched)
                    {
                        print("FOUND corresponing ID: " + s);

                    }
                    //FindPoiInMap(Int32.Parse(poiIdMatched));
                    FindPoiInMap(poisIdMatched);
                }
                else
                {
                    //if (i == PoiList["pois"].Count - 1)
                    //{
                    NotFound = true;                    
                }
            }
        }
    }
  
    void FindPoiInMap(HashSet<string> matchedIds)
    {
 
        foreach (POIMapSpecifications poi in poisInMap)
        {
            if (matchedIds.Contains(poi.id))
            {
                poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.gray;
            }
        }
        closeSearchBtn.gameObject.SetActive(true);
        SearchPanel.SetActive(false);

    }

    public void ResetSearch()
    {
        if (closeSearchBtn.gameObject.activeInHierarchy)
        {
            closeSearchBtn.gameObject.SetActive(false);
        }

        searchInputField.text = "";

        foreach (POIMapSpecifications poi in poisInMap)
        {
         
            poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.red;
            
        }
    }


    //Called when Input changes
    //private void inputSubmitCallBack()
    //{
    //    Debug.Log("Input Submitted");
    //    inputField.text = ""; //Clear Inputfield text
    //    inputField.ActivateInputField(); //Re-focus on the input field
    //    inputField.Select();//Re-focus on the input field
    //}

    ////Called when Input is submitted
    //private void inputChangedCallBack()
    //{
    //    Debug.Log("Input Changed");
    //}

    //void OnEnable()
    //{
    //    //Register InputField Events
    //    inputField.onEndEdit.AddListener(delegate { inputSubmitCallBack(); });
    //    inputField.onValueChanged.AddListener(delegate { inputChangedCallBack(); });
    //}

    //void OnDisable()
    //{
    //    //Un-Register InputField Events
    //    inputField.onEndEdit.RemoveAllListeners();
    //    inputField.onValueChanged.RemoveAllListeners();
    //}
}
