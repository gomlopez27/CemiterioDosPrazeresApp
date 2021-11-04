using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

public class LoadFromAPI : MonoBehaviour
{
    private const string URL = "https://616d4c4337f997001745d96a.mockapi.io/";
    private const int NUMBER_OF_REQUESTS = 3;
    int count = 1;

    private void Start()
    {

        //StartCoroutine(ProcessRequest(URL));
        StartCoroutine(GetInitialLists("PointsOfInterest", "/PoiList.json"));
        StartCoroutine(GetInitialLists("OfficialRoutes", "/OfficialRoutesList.json"));
        StartCoroutine(GetInitialLists("UnofficialRoutes", "/UnofficialRoutesList.json"));


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
            count++;
            // Show results as text
            string jsonToWrite = www.downloadHandler.text;
            Debug.Log(jsonToWrite);
            //JSONNode itemsData = JSON.Parse(www.downloadHandler.text);

            System.IO.File.WriteAllText(filePath, jsonToWrite);

        }
    }




}
