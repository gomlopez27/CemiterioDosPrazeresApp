using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

public class LoadFromAPI : MonoBehaviour
{
    private const string URL = "https://616d4c4337f997001745d96a.mockapi.io/";
    private const int NUMBER_OF_REQUESTS = 3;
    private bool isOkay;
    private void Start()
    {
        //StartCoroutine(ProcessRequest(URL));
        StartCoroutine(GetInitialLists("PointsOfInterest", "/PoiList.json"));
        StartCoroutine(GetInitialLists("OfficialRoutes", "/OfficialRoutesList.json"));

        string unofficialRoutesListFilePath = Application.persistentDataPath + "/UnofficialRoutesList.json";
        if (!System.IO.File.Exists(unofficialRoutesListFilePath))
        {
            StartCoroutine(GetInitialLists("UnofficialRoutes", "/UnofficialRoutesList.json"));

        }
        TextAsset routeToImport1 = Resources.Load<TextAsset>("ur123");
        TextAsset routeToImport2 = Resources.Load<TextAsset>("ur456");
        string routePath1 = Application.persistentDataPath + "/ur123.json";
        string routePath2 = Application.persistentDataPath + "/ur456.json";
        System.IO.File.WriteAllText(routePath1, routeToImport1.ToString());
        System.IO.File.WriteAllText(routePath2, routeToImport2.ToString());

    }

    private void Update()
    {
        //while(count <= NUMBER_OF_REQUESTS)
        //{
        //    //this.GetComponent<LoadingHomeScene>().UpdateProgress(NUMBER_OF_REQUESTS);
        //    this.gameObject.SetActive(false);
        //}
    }
    public IEnumerator GetInitialLists(string resourceName, string fileName)
    {
        //string oficialRoutesListFilePath = Application.persistentDataPath + "/OfficialRoutesList.json";
        string filePath = Application.persistentDataPath + fileName;

        UnityWebRequest www = UnityWebRequest.Get(URL + resourceName);
        //StartCoroutine(this.GetComponent<LoadingHomeScene>().ShowDownloadProgress(www));

        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            string jsonToWrite = www.downloadHandler.text;
            Debug.Log(jsonToWrite);
            //JSONNode itemsData = JSON.Parse(www.downloadHandler.text);
            System.IO.File.WriteAllText(filePath, jsonToWrite);

        }
    }


    public string Success()
    {
        return "Success";
    }

}
