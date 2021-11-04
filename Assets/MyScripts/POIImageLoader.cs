using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class POIImageLoader : MonoBehaviour
{
    private JSONNode PoiListData;

    // Start is called before the first frame update
    void Start()
    {
        //TextAsset json = Resources.Load<TextAsset>("POIData");
        //PoiListData = JSON.Parse(json.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPOIPhotos()
    {
        for (int i = 0; i < PoiListData["pois"].Count; i++)
        {
            StartCoroutine(GetTexture(PoiListData["pois"][i]["photoUrl"], PoiListData["pois"][i]["jazId"]));
        }
    }

    IEnumerator GetTexture(string url, string fileName)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            var myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(myTexture,
            new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + ".png", www.downloadHandler.data);


        }
    }
}
