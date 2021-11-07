using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InitialAppLoad : MonoBehaviour
{
    private AssetBundle myLoadedAssetBundle;

    // Start is called before the first frame update
    void Start()
    {
        string bundleUrl = "https://pasev.di.fct.unl.pt/contentFiles/Giovanna/AssetBundles/";

#if UNITY_ANDROID && !UNITY_EDITOR
        print("UNITY_ANDROID");
        if(myLoadedAssetBundle == null){
            StartCoroutine(LoadAssetBundle(bundleUrl + "augmentations-Android"));
        }
        //testAssetBundle("augmentations-Android");
#elif UNITY_EDITOR
        if (myLoadedAssetBundle == null)
        {
            print("UNITY_EDITOR");
            StartCoroutine(LoadAssetBundle(bundleUrl + "augmentations-Windows"));
            //testAssetBundle("augmentations-Windowss");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadAssetBundle(string url)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();
        //print("request.isDone " + request.isDone);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("error: " + request.error);
            Debug.Log("AssetBundle couldn't be loaded!");

        }
        else
        {
            //if (myLoadedAssetBundle != null)
            //{
            //    print("myLoadedAssetBundle not null, so unload it");
            //    myLoadedAssetBundle.Unload(false);
            //}

            myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
            MainDataHolder.myAssetBundle = myLoadedAssetBundle;
            print("SUCESS loading Asset Bundle");
            
            yield return new WaitUntil(AllLoaded);
            this.GetComponent<LoadScenes>().LoadHomeScene();
        }

    }

    bool AllLoaded()
    {
        print("PopularPois: " + MainDataHolder.PopularPois.Count);
        print("OfficialRoutes: " + MainDataHolder.OfficialRoutes.Count);
        print("UnofficialRoutes: " + MainDataHolder.UnofficialRoutes.Count);
        if (MainDataHolder.myAssetBundle != null && MainDataHolder.PopularPois.Count > 0
            && MainDataHolder.OfficialRoutes.Count > 0 
            && MainDataHolder.UnofficialRoutes.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
