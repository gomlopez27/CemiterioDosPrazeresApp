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
    GameObject LoadingArea;
    [SerializeField]
    GameObject ServerUnavailableArea;
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
        codesRoutesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";

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
        print("MainDataHolder.serverUnavailable: " + MainDataHolder.serverUnavailable);
        //if (!System.IO.File.Exists(unofficialRoutesListFilePath))
        //{
        //    StartCoroutine(GetInitialUnofficialRoutesLists());

        //}

        this.GetComponent<SerializableDataElements>().CreateRoutesCodeListFromJson(codesRoutesListFilePath);
        //GetRoutesToImport();


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

    IEnumerator LoadAssetBundleLocally()
    {
        if (!MainDataHolder.serverUnavailable) //If was able to connect to server
        {
            LoadingArea.SetActive(true);
            ServerUnavailableArea.SetActive(false);
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
        else
        {
            LoadingArea.SetActive(false);
            ServerUnavailableArea.SetActive(true);
            print("FICAR NA PAGINA INICIAL!");
        }
    }


}


    //IEnumerator LoadAssetBundle(string url)
    //{
    //    UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
    //    yield return request.SendWebRequest();
    //    // print("request progress: " + request.downloadProgress);

    //    //StartCoroutine(ShowDownloadProgress("Asset bundle loading: ", request));

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("error: " + request.error);
    //        Debug.Log("AssetBundle couldn't be loaded!");

    //    }
    //    else
    //    {
    //        print("request progress: " + request.downloadProgress);

    //        //if (myLoadedAssetBundle != null)
    //        //{
    //        //    print("myLoadedAssetBundle not null, so unload it");
    //        //    myLoadedAssetBundle.Unload(false);
    //        //}

    //        myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
    //        MainDataHolder.myAssetBundle = myLoadedAssetBundle;
    //        print("SUCESS loading Asset Bundle");

    //        //yield return new WaitUntil(AllLoaded);
    //        this.GetComponent<LoadScenes>().LoadHomeScene();
    //    }

    //}


    //IEnumerator DownloadAssetFromServer(string url)
    //{

    //    using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
    //    {
    //        var operation = uwr.SendWebRequest();
    //        while (!operation.isDone)
    //        {
    //            string s = (string.Format("{0:0%}", uwr.downloadProgress));
    //            loadedPercentage.text = s;
    //            slider.value = uwr.downloadProgress;
    //            yield return null;
    //        }

    //        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
    //        MainDataHolder.myAssetBundle = bundle;
    //        loadedPercentage.text = (string.Format("{0:0%}", uwr.downloadProgress));
    //        slider.value = 1;
    //        print("SUCESS loading Asset Bundle");
    //        //bundle.Unload(false);

    //        //yield return new WaitUntil(AllLoaded);

    //        this.GetComponent<LoadScenes>().LoadHomeScene();
    //    }
    //}

//}
