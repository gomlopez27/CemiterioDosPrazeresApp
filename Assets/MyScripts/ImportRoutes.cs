using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImportRoutes : MonoBehaviour
{
    private const string URL_GET_ROUTE = "http://localhost/api/routes/";

    [SerializeField]
    GameObject ImportRouteGO;
    [SerializeField]
    GameObject ConfirmPanel;
    [SerializeField]
    InputField inputCode;
    [SerializeField]
    Button confirmImportBtn;
    [SerializeField]
    GameObject LoadingPanel;
    [SerializeField]
    GameObject RouteListGO;
    [SerializeField]
    GameObject ToastMsgWrongCode;

    private string code;


    void Start()
    {
        RouteListGO.GetComponent<RoutesList>().enabled = false;
        ConfirmPanel.SetActive(true);
        LoadingPanel.SetActive(false);

        print("From static class UnofficialRoutes.Count: " + MainDataHolder.MyUnofficialRoutes.Count);
        print("From static class UnofficialRoutes 1ª rota: " + MainDataHolder.MyUnofficialRoutes[0].Name);
    }

    public void SetCode(string s)
    {
        code = s;
    }

    public void Clear()
    {
        inputCode.text = "";
    }

    public void Import()
    {
        StartCoroutine(this.GetComponent<LoadFromAPI>().GetImportedRoute(code, ConfirmPanel, LoadingPanel, ToastMsgWrongCode));
    }


    public IEnumerator GetImportedRoute(string code)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL_GET_ROUTE + code);
        //StartCoroutine(ShowDownloadProgress(UN_ROUTES_RESOURCE, www));

        yield return www.SendWebRequest();
        ConfirmPanel.SetActive(false);
        print(URL_GET_ROUTE + code);
        while (!www.isDone)
        {
            LoadingPanel.SetActive(true);
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);

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
            List<string> auxCodes = new List<string>();
            if(MainDataHolder.MyUnofficialRoutes.Count > 0) //Check if this user already has personalized routes 
            {
                auxRoutes = MainDataHolder.MyUnofficialRoutes;
            }
            auxRoutes.Add(importedRoute);
            MainDataHolder.MyUnofficialRoutes = auxRoutes;

            if (MainDataHolder.RouteCodes.Count > 0) //Check if this user already has personalized routes codes
            {
                auxCodes = MainDataHolder.RouteCodes;
            }
            auxCodes.Add(code);
            MainDataHolder.RouteCodes = auxCodes;
            this.GetComponent<SerializableDataElements>().SaveUpdatedRouteCodeList(MainDataHolder.RouteCodes);

            print("MainDataHolder.UnofficialRoutes: " + MainDataHolder.MyUnofficialRoutes.Count);
            print("MainDataHolder.RouteCodes: " + MainDataHolder.RouteCodes.Count);
            this.GetComponent<LoadScenes>().LoadRouteListScene();

        }
    }

 
}
