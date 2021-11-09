using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class FavoritesList
{
    public List<string> jazIdList = new List<string>();
}

[System.Serializable]
public class FavoritesBD
{
    public Dictionary<string, HashSet<string>> FavoritePersonPerJaz = new Dictionary<string, HashSet<string>>();

    public JSONNode Serialize()
    {
        var favoritesList = new JSONArray();

        foreach (var v in FavoritePersonPerJaz)
        {
            var obj = new JSONObject();

            var list = new JSONArray();
            foreach (var pers in v.Value)
            {
                list.Add(pers);
            }
            obj["jazId"] = v.Key;
            obj["personalities"] = list;

            favoritesList.Add(obj);
        }
        return favoritesList;
    }

    public void Deserialize(JSONNode node)
    {
        FavoritePersonPerJaz = new Dictionary<string, HashSet<string>>();
        var l = node.AsArray;
        foreach (var elem in l)
        {

            HashSet<string> personList = new HashSet<string>();
            string jazId = elem.Value["jazId"];
            var list = elem.Value["personalities"].AsArray;

            foreach (var personId in list)
            {
                personList.Add(personId.Value);

            }
            FavoritePersonPerJaz[elem.Key] = personList;

        }
    }
}


public class FavoritePoisMap : MonoBehaviour
{
    public const string MapScene = "CemeteryMapScene";

    [SerializeField]
    GameObject FavoritesListArea;
    [SerializeField]
    GameObject FavoritesListItem;
    [SerializeField]
    GameObject EmptyListItem;
    [SerializeField]
    GameObject ConfirmDeletePanel;
    [SerializeField]
    public Camera _referenceCamera;
    [SerializeField]
    AbstractMap _map;
    [SerializeField]
    Button FavoriteListBtn;
    [SerializeField]
    Button ClearBtn;    
    [SerializeField]
    JazInformations jazInfo;

    private JSONNode FavoritesListJSON;
    private JSONNode FavoritesListJSONTest;
    private JSONNode PoisJson;

    //private string filePath;
    private string favoritesFilePath;
    private POIMapSpecifications[] poisInMap;
    private Dictionary<string, HashSet<string>> favoritePersonalities;

    void Start()
    {
        favoritesFilePath = Application.persistentDataPath + "/FavoritesList.json";
        favoritePersonalities = new Dictionary<string, HashSet<string>>();
        GetFromJson();
        FavoriteListBtn.onClick.AddListener(() => {
            DeactivatePoiOnMap();
            SetUpFavList();
        });
    }

    private void Update()
    {
    }

    public void AddPersonalityToFavs(string jazId, string personId)
    {
        HashSet<string> personalities;

        if (favoritePersonalities == null)
        {
            favoritePersonalities = new Dictionary<string, HashSet<string>>();

        }

        if (!favoritePersonalities.TryGetValue(jazId, out personalities))
        {
            personalities = new HashSet<string>();
            favoritePersonalities.Add(jazId,personalities);
        }

        personalities.Add(personId);
        //SaveToJson();

    }

    public void RemovePersonalityFromFav(string jazId, string personId)
    {
        HashSet<string> personalities;

        if (favoritePersonalities.TryGetValue(jazId, out personalities))
        {
            var value = personalities.Contains(personId);

            if (value)
            {
                personalities.Remove(personId);
                if (personalities.Count == 0)
                {
                    favoritePersonalities.Remove(jazId);
                }
            }
        }

        if (favoritePersonalities.Count == 0)
        {
            EmptyListItem.SetActive(true);
        }

        //SaveToJson();
    }

    public bool isFavorite(string jazId, string personId)
    {
        HashSet<string> personalities;

        if (favoritePersonalities!= null && favoritePersonalities.TryGetValue(jazId, out personalities))
        {
            var value = personalities.Contains(personId);

            if (value)
            {
                return true;
            }
        }

        return false;

    }

    public void SaveToJson()
    {
        print("favoritePersonalities count: " + favoritePersonalities.Count);

        FavoritesBD favorites = new FavoritesBD();
        favorites.FavoritePersonPerJaz = favoritePersonalities;
        string jsonToWrite = favorites.Serialize().ToString(3);
        System.IO.File.WriteAllText(favoritesFilePath, jsonToWrite);

    }

    public void GetFromJson()
    {
        if (System.IO.File.Exists(favoritesFilePath))
        {
            string json = File.ReadAllText(favoritesFilePath);
            FavoritesListJSONTest = JSON.Parse(json.ToString());

            //if (favoritePersonalities == null)
            //{
            //    favoritePersonalities = new Dictionary<string, HashSet<string>>();
            //}
            for (int i = 0; i < FavoritesListJSONTest.Count; i++)
            {
                HashSet<string> auxSet = new HashSet<string>();
                string jazid = FavoritesListJSONTest[i]["jazId"];
                JSONNode personalities = FavoritesListJSONTest[i]["personalities"];

                for (int j = 0; j < personalities.Count; j++)
                {
                    auxSet.Add(personalities[j]);
                }
                favoritePersonalities.Add(jazid, auxSet);
            }

            print("GetFromJson favoritePersonalities.Count: " + favoritePersonalities.Count);

        }

    }


    /*Chamada quando se carrega no botão dos favoritos*/
    public void SetUpFavList()
    {
        SaveToJson();
        TextAsset jsonPoisInMap = Resources.Load<TextAsset>("MapPopularPOI");
        PoisJson = JSON.Parse(jsonPoisInMap.ToString());
        //favoritesFilePath = Application.persistentDataPath + "/FavoritesList.json";
        print(favoritesFilePath);
        //GetFromJson();
        DestroyFavList();
        //GameObject FavoritesListItem = FavoritesListArea.transform.Find("FavListItem").gameObject;
        //GameObject EmptyListItem = FavoritesListArea.transform.Find("EmptyListItem").gameObject;

        if (!System.IO.File.Exists(favoritesFilePath))
        {

            EmptyListItem.SetActive(true);
            FavoritesListItem.SetActive(false);

        }
        else
        {
            string json = File.ReadAllText(favoritesFilePath);
            FavoritesListJSONTest = JSON.Parse(json.ToString());

            if (FavoritesListJSONTest.Count <= 0)
            {
                EmptyListItem.SetActive(true);
                FavoritesListItem.SetActive(false);
            }
            else
            {
                EmptyListItem.SetActive(false);
                //FavoritesListItem.SetActive(true);

                for (int i = 0; i < FavoritesListJSONTest.Count; i++)
                {
                    string jazId = FavoritesListJSONTest[i]["jazId"];
                    JSONNode personalities = FavoritesListJSONTest[i]["personalities"];
                    for (int j = 0; j < personalities.Count; j++)
                    {
                        string personId = personalities[j];

                        GameObject g = Instantiate(FavoritesListItem, FavoritesListArea.transform);
                        g.SetActive(true);
                        g.name = "item-" + jazId + "-" + personId;
                        Text title = g.transform.Find("PoiId").GetComponent<Text>();
                        
                        string personName = jazInfo.GetPersonalityName(jazId, personId);
                        title.text = personName + "  " + " (Jazigo " + jazId + ")";

                        Button SeePoiInMapBtn = g.transform.Find("SeeOnMapBtn").gameObject.GetComponent<Button>();
                        Button DeleteBtn = g.transform.Find("DeleteBtn").gameObject.GetComponent<Button>();
                        DeleteBtn.onClick.AddListener(() =>
                        {
                            ConfirmDeletePanel.SetActive(true);
                            ConfirmDelete(jazId, personId, g);
                        });
                        SeePoiInMapBtn.onClick.AddListener(() =>
                        {
                            DestroyFavList();
                            FocusInMap(jazId);
                        });
                    }
                }

                FavoritesListItem.SetActive(false);
            }


        }

    }

   
    public void DestroyFavList()
    {
        if (FavoritesListArea.transform.childCount > 0)
        {
            for (int i = 0; i < FavoritesListArea.transform.childCount; i++)
            {
                print(FavoritesListArea.transform.GetChild(i).gameObject.name);
                Destroy(FavoritesListArea.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ConfirmDelete(string jazId, string personId, GameObject listItem)
    {
        Button ConfirmDelBtn = ConfirmDeletePanel.transform.Find("Background/BottomLine/ConfirmDeleteBtn").gameObject.GetComponent<Button>();
        ConfirmDelBtn.onClick.AddListener(() =>
        {
            //RemoveJazAsFavorite(id);
            RemovePersonalityFromFav(jazId, personId);
            Destroy(listItem);
            ConfirmDeletePanel.SetActive(false);
        });

    }


    public void DeactivatePoiOnMap()
    {
        poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        print("DeactivatePoiOnMap length" + poisInMap.Length);
        foreach (POIMapSpecifications poi in poisInMap)
        {
            poi.gameObject.SetActive(false);
        }
    }

    public void ActivatePoiOnMap()
    {
        poisInMap = GameObject.FindObjectsOfType<POIMapSpecifications>(true);
        print("ActivatePoiOnMap length" + poisInMap.Length);

        foreach (POIMapSpecifications poi in poisInMap)
        {
            poi.gameObject.SetActive(true);
        }
    }

    public void FocusInMap(string id)
    {
        foreach (POIMapSpecifications poi in poisInMap)
        {
            if (poi.id.Equals(id))
            {
                string auxName = "Poi-" + id;
                print(auxName);

                GameObject poiAux = GameObject.Find(auxName);

                //_referenceCamera.transform.position = new Vector3(poiAux.transform.position.x,
                //                                             _referenceCamera.transform.position.y,
                //                                             poiAux.transform.position.z);
                string location = poi.latitude + "," + poi.longitude;
                Vector2d latlong = Conversions.StringToLatLon(location);
                poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.green;
                _map.SetCenterLatitudeLongitude(latlong);
                _map.UpdateMap(_map.CenterLatitudeLongitude, 17.5f);
                ClearBtn.gameObject.SetActive(true);

                ClearBtn.onClick.AddListener(() =>
                {
                    poi.transform.GetChild(0).Find("pinpoint").GetComponent<MeshRenderer>().material.color = Color.red;
                    ClearBtn.gameObject.SetActive(false);

                });

                break;
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        if (!scene.name.Equals(MapScene))
        {
            SaveToJson();

        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        SaveToJson();
    }

}

