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
    //Mock resources
    private const string URL_MOCK_API = "https://616d4c4337f997001745d96a.mockapi.io/";
    private const string POIS_MOCK_RESOURCE = "PointsOfInterest";
    private const string OF_ROUTES_MOCK_RESOURCE = "OfficialRoutes";
    private const string UN_ROUTES_MOCK_RESOURCE = "UnofficialRoutes";

    //Resources from SI
    private const string POIS_RESOURCE = "funeral-constructions";
    private const string ROUTES_RESOURCE = "routes";
    private const string CREATE_ROUTES_RESOURCE = "routes/create-route";

    private List<string> codeList;
    private string codesRoutesListFilePath;

    private void Start()
    {
        
    }
    public IEnumerator GetInitialPoiList()
    {
        //UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + POIS_MOCK_RESOURCE); //UNCOMMENT when NOT  using the SI
        UnityWebRequest www = UnityWebRequest.Get(MainDataHolder.URL_API + POIS_RESOURCE); //UNCOMMENT when USING the SI
        //StartCoroutine(ShowDownloadProgress(POIS_RESOURCE, www));

        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonToWrite = www.downloadHandler.text;
            JSONNode PoisNode = JSON.Parse(jsonToWrite.ToString());
            List<Poi> PoisList = this.GetComponent<SerializableDataElements>().ConvertJsonToPoiList(PoisNode);
            //List<Poi> PoisList = new PoiCollection().Deserialize(PoisNode);

            MainDataHolder.PopularPois = PoisList;
            print("MainDataHolder.PopularPois: " + MainDataHolder.PopularPois.Count);
            //System.IO.File.WriteAllText(poisListFilePath, jsonToWrite);
        }
    }

    public IEnumerator GetInitialOfficialRoutesLists()
    {
        //UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + OF_ROUTES_MOCK_RESOURCE); //UNCOMMENT when NOT  using the SI
        UnityWebRequest www = UnityWebRequest.Get(MainDataHolder.URL_API + ROUTES_RESOURCE); //UNCOMMENT when USING the SI
        //StartCoroutine(ShowDownloadProgress(OF_ROUTES_RESOURCE,www));

        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonToWrite = www.downloadHandler.text;
            JSONNode OfficialRoutesJson = JSON.Parse(jsonToWrite.ToString());
            List<Route> AllRoutes = this.GetComponent<SerializableDataElements>().ConvertJsonToRouteList(OfficialRoutesJson);
            List<Route> OfficialRoutes = new List<Route>();
            foreach(Route r in AllRoutes)
            {
                if (r.isOfficial)
                    OfficialRoutes.Add(r);
            }
            MainDataHolder.OfficialRoutes = OfficialRoutes;
            print("MainDataHolder.OfficialRoutes: " + MainDataHolder.OfficialRoutes.Count);

            //System.IO.File.WriteAllText(officialRoutesListFilePath, jsonToWrite);

        }
    }


    public IEnumerator GetInitialUnofficialRoutesLists()
    {
        LoadCodeList();
        string codesURL = BuildCodesUrl();
        print("codesURL: " + codesURL);

        if (codesURL != "")
        {
            //UnityWebRequest www = UnityWebRequest.Get(URL_MOCK_API + UN_ROUTES_MOCK_RESOURCE); //UNCOMMENT when NOT  using the SI
            UnityWebRequest www = UnityWebRequest.Get(MainDataHolder.URL_API + ROUTES_RESOURCE + codesURL); //UNCOMMENT when USING the SI
            //StartCoroutine(ShowDownloadProgress(UN_ROUTES_RESOURCE, www));

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                MainDataHolder.MyUnofficialRoutes = new List<Route>();
            }
            else
            {
                string jsonToWrite = www.downloadHandler.text;
                JSONNode UnofficialRoutesJson = JSON.Parse(jsonToWrite.ToString());
                List<Route> UnofficialRoutes = this.GetComponent<SerializableDataElements>().ConvertJsonToRouteList(UnofficialRoutesJson);
               // MainDataHolder.AllUnofficialRoutes = UnofficialRoutes;
                MainDataHolder.MyUnofficialRoutes = UnofficialRoutes; //UNCOMMENT when USING the SI
                //FilterUnofficialRoutes(UnofficialRoutes); //UNCOMMENT when NOT  using the SI
                print("MainDataHolder.UnofficialRoutes: " + MainDataHolder.MyUnofficialRoutes.Count);

            }
        }


    }

    private void LoadCodeList()
    {
        codesRoutesListFilePath = Application.persistentDataPath + "/RoutesCodesList.json";

        if (System.IO.File.Exists(codesRoutesListFilePath))
        {
            string jsonCodesRoutesList = File.ReadAllText(codesRoutesListFilePath);
            JSONNode CodesList = JSON.Parse(jsonCodesRoutesList.ToString());

            codeList = new List<string>();
            for (int i = 0; i < CodesList.Count; i++)
            {
                codeList.Add(CodesList[i]);
            }
        }
    }

    /*build url string: routes?codes=uiiid1,uuid2 */
    private string BuildCodesUrl()
    {
        string codesUrl = "";

        if (codeList != null)
        {
            codesUrl = "?codes=";
            for (int i = 0; i < codeList.Count; i++)
            {
                if (i == codeList.Count - 1)
                {
                    codesUrl += codeList[i];
                }
                else
                {
                    codesUrl += codeList[i] + ",";
                }
            }
        }

        return codesUrl;
    }

    private void FilterUnofficialRoutes(List<Route> UnofficialRoutes)
    {
        List<Route> auxUnofficialRoutes = new List<Route>();
        foreach (Route r in UnofficialRoutes)
        {
            if (codeList != null && codeList.Contains(r.Code))
            {
                auxUnofficialRoutes.Add(r);
            }
        }

        MainDataHolder.MyUnofficialRoutes = auxUnofficialRoutes;

    }

    public IEnumerator PostRoute(string json, GameObject LoadingPanel, LoadScenes Scenes)
    {
        //print(json);
        //UnityWebRequest uwr = UnityWebRequest.Post(URL_POST_ROUTE, json);
        var uwr = new UnityWebRequest(MainDataHolder.URL_API + CREATE_ROUTES_RESOURCE, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

    
        yield return uwr.SendWebRequest();

        while (!uwr.isDone)
        {
            LoadingPanel.SetActive(true);

        }
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            string jsonToWrite = uwr.downloadHandler.text;
            LoadingPanel.SetActive(false);

            Debug.Log(jsonToWrite);
        }
        else
        {
            Debug.Log("SUCCESS");
            string jsonCreatedRoute = uwr.downloadHandler.text;
            Debug.Log(jsonCreatedRoute);
            JSONNode CreatedRouteJsonNode = JSON.Parse(jsonCreatedRoute.ToString());
            Route createdRoute = this.GetComponent<SerializableDataElements>().ConvertJsonToRoute(CreatedRouteJsonNode);

            List<Route> auxRoutes = new List<Route>();
            if (MainDataHolder.MyUnofficialRoutes != null && MainDataHolder.MyUnofficialRoutes.Count > 0) //Check if this user already has personalized routes 
            {
                auxRoutes = MainDataHolder.MyUnofficialRoutes;
            }
            auxRoutes.Add(createdRoute);
            MainDataHolder.MyUnofficialRoutes = auxRoutes;

            this.GetComponent<SerializableDataElements>().SaveRouteCodeToJson(createdRoute.Code);
            Scenes.LoadRouteListScene();

        }
    }

    public IEnumerator GetImportedRoute(string code, GameObject ConfirmPanel, GameObject LoadingPanel, GameObject ToastMsgWrongCode)
    {
        UnityWebRequest www = UnityWebRequest.Get(MainDataHolder.URL_API + ROUTES_RESOURCE +"/"+ code);
        //StartCoroutine(ShowDownloadProgress(UN_ROUTES_RESOURCE, www));

        yield return www.SendWebRequest();
        ConfirmPanel.SetActive(false);
        print(MainDataHolder.URL_API + ROUTES_RESOURCE + "/" + code);
        while (!www.isDone)
        {
            LoadingPanel.SetActive(true);
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            LoadingPanel.SetActive(false);

            print("Código Não existe");
            ToastMsgWrongCode.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            ToastMsgWrongCode.SetActive(false);
            this.GetComponent<LoadScenes>().LoadRouteListScene();

        }
        else
        {
            LoadingPanel.SetActive(true);

            string jsonToWrite = www.downloadHandler.text;
            JSONNode jsonImportedRoute = JSON.Parse(jsonToWrite.ToString());
            Route importedRoute = this.GetComponent<SerializableDataElements>().ConvertJsonToRoute(jsonImportedRoute);

            List<Route> auxRoutes = new List<Route>();
            if (MainDataHolder.MyUnofficialRoutes.Count > 0) //Check if this user already has personalized routes 
            {
                auxRoutes = MainDataHolder.MyUnofficialRoutes;
            }
            auxRoutes.Add(importedRoute);
            MainDataHolder.MyUnofficialRoutes = auxRoutes;

            this.GetComponent<SerializableDataElements>().SaveRouteCodeToJson(importedRoute.Code);

            //List<string> auxCodes = new List<string>();
            //if (MainDataHolder.RouteCodes.Count > 0) //Check if this user already has personalized routes codes
            //{
            //    auxCodes = MainDataHolder.RouteCodes;
            //}
            //auxCodes.Add(code);
            //MainDataHolder.RouteCodes = auxCodes;
            //this.GetComponent<SerializableDataElements>().SaveUpdatedRouteCodeList(MainDataHolder.RouteCodes);

            print("MainDataHolder.UnofficialRoutes: " + MainDataHolder.MyUnofficialRoutes.Count);
            print("MainDataHolder.RouteCodes: " + MainDataHolder.RouteCodes.Count);
            this.GetComponent<LoadScenes>().LoadRouteListScene();

        }
    }


}
