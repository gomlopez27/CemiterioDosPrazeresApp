using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InitialAppLoad : MonoBehaviour
{
    private const string URL_API = "http://192.168.0.17/api/";
    private const string URL_MOCK_API = "https://616d4c4337f997001745d96a.mockapi.io/";
    private const string POIS_MOCK_RESOURCE = "PointsOfInterest";
    private const string OF_ROUTES_MOCK_RESOURCE = "OfficialRoutes";
    private const string UN_ROUTES_MOCK_RESOURCE = "UnofficialRoutes";

    [SerializeField]
    Slider slider;
    [SerializeField]
    Text loadedPercentage;

    private AssetBundle myLoadedAssetBundle;
    private string poisListFilePath;
    private string officialRoutesListFilePath;
    private string unofficialRoutesListFilePath;
    private string codesRoutesListFilePath;
    bool poisListCreated;
    bool ofRoutesListCreated;
    bool unRoutesListCreated;
    bool codesListCreated;
    float process;
    private List<string> codeList;

    // Start is called before the first frame update
    void Start()
    {
        string bundleUrl = "https://pasev.di.fct.unl.pt/contentFiles/Giovanna/AssetBundles/";

#if UNITY_ANDROID && !UNITY_EDITOR
        print("UNITY_ANDROID");
        if(myLoadedAssetBundle == null){
            //StartCoroutine(DownloadAsset(bundleUrl + "augmentations-Android"));
            //StartCoroutine(LoadAssetBundle(bundleUrl + "augmentations-Android"));
            StartCoroutine(LoadAssetBundleLocally());

        }
        //testAssetBundle("augmentations-Android");
#elif UNITY_EDITOR
        if (myLoadedAssetBundle == null)
        {
            print("UNITY_EDITOR");
            StartCoroutine(LoadAssetBundleLocally());
        }
#endif        
        poisListFilePath = Application.persistentDataPath + "/PoiList.json";
        officialRoutesListFilePath = Application.persistentDataPath + "/OfficialRoutesList.json";
        unofficialRoutesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
        //codesRoutesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";

        //if (System.IO.File.Exists(codesRoutesListFilePath))
        //{
        //    string jsonCodesRoutesList = File.ReadAllText(codesRoutesListFilePath);
        //    JSONNode CodesList = JSON.Parse(jsonCodesRoutesList.ToString());

        //    codeList = new List<string>();
        //    for (int i = 0; i < CodesList.Count; i++)
        //    {
        //        codeList.Add(CodesList[i]);
        //    }
        //}

        StartCoroutine(this.GetComponent<LoadFromAPI>().GetInitialPoiList());
        StartCoroutine(this.GetComponent<LoadFromAPI>().GetInitialOfficialRoutesLists());
        StartCoroutine(this.GetComponent<LoadFromAPI>().GetInitialUnofficialRoutesLists());

        //if (!System.IO.File.Exists(unofficialRoutesListFilePath))
        //{
        //    StartCoroutine(GetInitialUnofficialRoutesLists());

        //}

        this.GetComponent<SerializableDataElements>().CreateRoutesCodeListFromJson(codesRoutesListFilePath);
        GetRoutesToImport();


    }


    void GetRoutesToImport()
    {
        TextAsset routeToImport1 = Resources.Load<TextAsset>("ur123");
        TextAsset routeToImport2 = Resources.Load<TextAsset>("ur456");
        string routePath1 = Application.persistentDataPath + "/ur123.json";
        string routePath2 = Application.persistentDataPath + "/ur456.json";
        System.IO.File.WriteAllText(routePath1, routeToImport1.ToString());
        System.IO.File.WriteAllText(routePath2, routeToImport2.ToString());

    }

    //public IEnumerator GetInitialPoiList()
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + POIS_MOCK_RESOURCE);
    //    //StartCoroutine(ShowDownloadProgress(POIS_RESOURCE, www));

    //    yield return www.SendWebRequest();
    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        string jsonToWrite = www.downloadHandler.text;
    //        JSONNode PoisNode = JSON.Parse(jsonToWrite.ToString());
    //        List<Poi> PoisList = this.GetComponent<SerializableDataElements>().ConvertJsonToPoiList(PoisNode);
    //        //List<Poi> PoisList = new PoiCollection().Deserialize(PoisNode);

    //        MainDataHolder.PopularPois = PoisList;
    //        print("MainDataHolder.PopularPois: " + MainDataHolder.PopularPois.Count);
    //        //System.IO.File.WriteAllText(poisListFilePath, jsonToWrite);
    //    }
    //}

    //public IEnumerator GetInitialOfficialRoutesLists()
    //{
    //    UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + OF_ROUTES_MOCK_RESOURCE);
    //    //StartCoroutine(ShowDownloadProgress(OF_ROUTES_RESOURCE,www));

    //    yield return www.SendWebRequest();
    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        string jsonToWrite = www.downloadHandler.text;
    //        JSONNode OfficialRoutesJson = JSON.Parse(jsonToWrite.ToString());
    //        List<Route> OfficialRoutes = this.GetComponent<SerializableDataElements>().ConvertJsonToRouteList(OfficialRoutesJson);
    //        MainDataHolder.OfficialRoutes = OfficialRoutes;
    //        print("MainDataHolder.OfficialRoutes: " + MainDataHolder.OfficialRoutes.Count);

    //        //System.IO.File.WriteAllText(officialRoutesListFilePath, jsonToWrite);

    //    }
    //}


    //public IEnumerator GetInitialUnofficialRoutesLists()
    //{
    //    //string codesURL = BuildCodesUrl();
    //    UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + UN_ROUTES_MOCK_RESOURCE);
    //    //StartCoroutine(ShowDownloadProgress(UN_ROUTES_RESOURCE, www));

    //    yield return www.SendWebRequest();
    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        string jsonToWrite = www.downloadHandler.text;
    //        JSONNode UnofficialRoutesJson = JSON.Parse(jsonToWrite.ToString());
    //        List<Route> UnofficialRoutes = this.GetComponent<SerializableDataElements>().ConvertJsonToRouteList(UnofficialRoutesJson);
    //        MainDataHolder.AllUnofficialRoutes = UnofficialRoutes;
    //        //MainDataHolder.MyUnofficialRoutes = UnofficialRoutes; //UNCOMMENT when USING the SI
    //        FilterUnofficialRoutes(UnofficialRoutes); //UNCOMMENT when NOT  using the SI
    //        print("MainDataHolder.UnofficialRoutes: " + MainDataHolder.MyUnofficialRoutes.Count);

    //    }
    //}

    //private string BuildCodesUrl()
    //{
    //    /*http://localhost:8080/api/routes?codes=uiiid1,uuid2 */

    //    string codesUrl = "?codes=";
    //    if (codeList != null)
    //    {
    //        for (int i = 0; i < codeList.Count; i++)
    //        {
    //            if (i == codeList.Count - 1)
    //            {
    //                codesUrl += codeList[i];
    //            }
    //            else
    //            {
    //                codesUrl += codeList[i] + ",";
    //            }
    //        }
    //    }

    //    return codesUrl;
    //}

    //private void FilterUnofficialRoutes(List<Route> UnofficialRoutes)
    //{
    //    List<Route> auxUnofficialRoutes = new List<Route>();
    //    foreach (Route r in UnofficialRoutes)
    //    {
    //        if (codeList!= null && codeList.Contains(r.Code))
    //        {
    //            auxUnofficialRoutes.Add(r);
    //        }
    //     }

    //    MainDataHolder.MyUnofficialRoutes = auxUnofficialRoutes;

    //}


    IEnumerator LoadAssetBundleLocally()
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "augmentationsprefabs"));
        while (!bundleLoadRequest.isDone)
        {
            string s = (string.Format("{0:0%}", bundleLoadRequest.progress));
            loadedPercentage.text = s;
            slider.value = bundleLoadRequest.progress;
            yield return null;
        }
        yield return bundleLoadRequest;

        AssetBundle myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        MainDataHolder.myAssetBundle = myLoadedAssetBundle;
        print("Asset bundle loaded!");

        GameObject[] assetsLoadRequest = myLoadedAssetBundle.LoadAllAssets<GameObject>();
        yield return assetsLoadRequest;
        MainDataHolder.augmentationsGO = assetsLoadRequest;
        myLoadedAssetBundle.Unload(false);
        this.GetComponent<LoadScenes>().LoadHomeScene();


    }


    IEnumerator LoadAssetBundle(string url)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();
        // print("request progress: " + request.downloadProgress);

        //StartCoroutine(ShowDownloadProgress("Asset bundle loading: ", request));

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("error: " + request.error);
            Debug.Log("AssetBundle couldn't be loaded!");

        }
        else
        {
            print("request progress: " + request.downloadProgress);

            //if (myLoadedAssetBundle != null)
            //{
            //    print("myLoadedAssetBundle not null, so unload it");
            //    myLoadedAssetBundle.Unload(false);
            //}

            myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
            MainDataHolder.myAssetBundle = myLoadedAssetBundle;
            print("SUCESS loading Asset Bundle");

            //yield return new WaitUntil(AllLoaded);
            this.GetComponent<LoadScenes>().LoadHomeScene();
        }

    }


    IEnumerator DownloadAssetFromServer(string url)
    {

        using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            var operation = uwr.SendWebRequest();
            while (!operation.isDone)
            {
                string s = (string.Format("{0:0%}", uwr.downloadProgress));
                loadedPercentage.text = s;
                slider.value = uwr.downloadProgress;
                yield return null;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
            MainDataHolder.myAssetBundle = bundle;
            loadedPercentage.text = (string.Format("{0:0%}", uwr.downloadProgress));
            slider.value = 1;
            print("SUCESS loading Asset Bundle");
            //bundle.Unload(false);

            //yield return new WaitUntil(AllLoaded);
            this.GetComponent<LoadScenes>().LoadHomeScene();
        }
    }

}
