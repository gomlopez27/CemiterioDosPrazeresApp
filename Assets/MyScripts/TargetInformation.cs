using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Wikitude;
using SimpleJSON;


public class TargetInformation : MonoBehaviour
{

    //[System.Serializable]
    //public class TargetData
    //{
    //    public string name;
    //    public string description;
    //    public string imageURL;
    //}

    //[System.Serializable]
    //public class TrackerData
    //{
    //    public string trackerName;
    //    public TargetData[] targets; 
    //}

    //[System.Serializable]
    //public class TrackerList
    //{
    //    public TrackerData[] trackers; 
    //}

  

    //public TrackerList myTrackersList = new TrackerList();

    //public List<ObjectTracker> objectTrackers;
    //public List<string> objectTrackersNames;
    public Image imageUI;
    public Text nameUI;
    public Text descriptionUI;
    private string targetNameReconized;
    //public List<GameObjectData> TrackersAsGameObj = new List<GameObjectData>();
    private JSONNode trackersListData;
    private ObjectTracker[] allObjectTrackers;


    void Start()
    {
        //myTrackersList = JsonConvert.DeserializeObject<TrackerList>(json);
        //myTrackersList = JsonUtility.FromJson<TrackerList>(json);
        //string json = File.ReadAllText(Application.dataPath + "/Resources/TrackersData.json");
        TextAsset json = Resources.Load<TextAsset>("TrackersData");
        trackersListData = JSON.Parse(json.ToString());
       // GetTrackersGPSPosition();


        //allObjectTrackers = FindObjectsOfType<ObjectTracker>(true);
        //for (int i = 0; i<allObjectTrackers.Length; i++)
        //{
        //    print(allObjectTrackers[i].transform.gameObject.name + " is active?" + allObjectTrackers[i].transform.gameObject.activeInHierarchy);

        //}

    }

    void Update()
    {
       
    }

    public void OnObjectRecognized(ObjectTarget o)
    {
        targetNameReconized = o.Name;       
      
    }

    public virtual void OnObjectLost(ObjectTarget o)
    {
        targetNameReconized = "";
    }
    public void test(string s)
    {
        print("teste: " + s);
    }

    public void LoadData(string address, string objTargetName)
    {
        for (int j = 0; j < trackersListData["trackers"].Count; j++)
        {
            //foreach (Transform child in ObjectTrackers.transform)
            //{
            //    //    foreach (ObjectTracker o in objectTrackers)
            //    //{
            //    //Só vale a pena procurar no ficheiro JSON, se o object tracker estiver ativo na hieraquia
            //    //Só vai estar um objectTracker activo de cada vez, talvez possa fazer este codigo noutro sitio
            //    //Debug.Log(o.gameObject.name + " is active?: " + o.gameObject.activeSelf);
            //    if (child.gameObject.activeSelf)
            //    {
            //        ObjectTracker o = child.GetComponent<ObjectTracker>();
            //        string s = o.TargetCollectionResource.TargetPath;
            //        string[] subs = s.Split('/');
            //        string trackerNameAux = subs[subs.Length - 1].Split('.')[0];

                    //TrackerData tracker = myTrackersList.trackers[j];
                    string trackerNameJson = trackersListData["trackers"][j]["trackerName"];

                    if (trackerNameJson.Equals(address))
                    {
                        for (int i = 0; i < trackersListData["trackers"][j]["targets"].Count; i++)
                        {
                            string targetNameAux = trackersListData["trackers"][j]["targets"][i]["name"];
                            Debug.Log("nameT:" + targetNameAux);
                            Debug.Log("targetNameReconized: " + objTargetName);

                            if (objTargetName.Equals(targetNameAux))
                            {
                                nameUI.text = targetNameAux;
                                descriptionUI.text = trackersListData["trackers"][j]["targets"][i]["description"];
                                StartCoroutine(GetTexture(trackersListData["trackers"][j]["targets"][i]["imageURL"]));
                            }
                            else
                            {
                                nameUI.text = "Indisponivel";
                                descriptionUI.text = "Indisponivel";
                            }
                        }
                    }

                //}
            //}
        }

     }

        //public void LoadData()
        //{
        //    for (int j = 0; j < myTrackersList.trackers.Length; j++)
        //    {

        //        foreach (Transform child in ObjectTrackers.transform)
        //        {
        //        //    foreach (ObjectTracker o in objectTrackers)
        //        //{
        //            //Só vale a pena procurar no ficheiro JSON, se o object tracker estiver ativo na hieraquia
        //            //Só vai estar um objectTracker activo de cada vez, talvez possa fazer este codigo noutro sitio
        //            //Debug.Log(o.gameObject.name + " is active?: " + o.gameObject.activeSelf);
        //            if (child.gameObject.activeSelf)
        //            {
        //                ObjectTracker o = child.GetComponent<ObjectTracker>();
        //                string s = o.TargetCollectionResource.TargetPath;
        //                string[] subs = s.Split('/');
        //                string trackerNameAux = subs[subs.Length - 1].Split('.')[0];

        //                TrackerData tracker = myTrackersList.trackers[j];

        //            if (tracker.trackerName.Equals(trackerNameAux))
        //            {
        //                for (int i = 0; i < tracker.targets.Length; i++)
        //                {
        //                    string targetNameAux = tracker.targets[i].name;
        //                    Debug.Log("nameT:" + targetNameAux);
        //                    //Debug.Log("targetNameReconized:" + targetNameReconized);

        //                    if (targetNameReconized.Equals(targetNameAux))
        //                    {
        //                        nameUI.text = targetNameAux;
        //                        descriptionUI.text = tracker.targets[i].description;
        //                        StartCoroutine(GetTexture(tracker.targets[i].imageURL));
        //                    }
        //                    else
        //                    {
        //                        nameUI.text = "Indisponivel";
        //                        descriptionUI.text = "Indisponivel";
        //                    }
        //                }
        //            }

        //            }
        //        }
        //    }

        //}

        IEnumerator GetTexture(string url)
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
            imageUI.sprite = sprite;

        }
        }


}
