using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Wikitude;
using SimpleJSON;

public class GPSLocation : MonoBehaviour
{
    [System.Serializable]
    public class TrackersGPSData
    {
        public string gameObjName;
        public string wtoName;
        public double gameObjLatitude;
        public double gameObjLongitude;

        public TrackersGPSData(string gameObjName, string wtoName, double gameObjLatitude, double gameObjLongitude)
        {
            this.gameObjName = gameObjName;
            this.wtoName = wtoName;
            this.gameObjLatitude = gameObjLatitude;
            this.gameObjLongitude = gameObjLongitude;
        }
    }
   
    private bool isUpdating;
    private bool hasProcessedGPSInfo;
    private AuxiliarGPS AuxiliarGPSScript;
    private JSONNode trackersListData;
    private ObjectTracker[] allObjectTrackers;
    private List<TrackersGPSData> TrackersAsGameObj = new List<TrackersGPSData>();
    private double userCurrentLat;
    private double userCurrentLong;
    //for debug panel
    private Text userCoords;
    private Text activeTracker;


    private void Start()
    {
        //string json = File.ReadAllText(Application.dataPath + "/Resources/TrackersData.json");
        TextAsset json = Resources.Load<TextAsset>("TrackersData");
        trackersListData = JSON.Parse(json.ToString());
        AuxiliarGPSScript = GetComponent<AuxiliarGPS>();
        //GetTrackersGPSPosition();


        //Debug.Log("lat: " + trackersListData["trackers"][1]["gpsPosition"]["latitude"]);
        //Debug.Log("long: " + trackersListData["trackers"][1]["gpsPosition"]["longitude"]);

    }

    private void Update()
    {
        
        if (!hasProcessedGPSInfo)
        {
            allObjectTrackers = FindObjectsOfType<ObjectTracker>(true);
            print("From GPSLocationScript -> allObjectTrackers: " + allObjectTrackers.Length);
      
            if(allObjectTrackers.Length != 0)
            {
                GetTrackersGPSPosition();
            }
           
        }
        if (!isUpdating)
        {
            StartCoroutine(StartLocationService());
            isUpdating = !isUpdating;
        }

        if (hasProcessedGPSInfo && isUpdating)
        {
            ActivateTracker();
        }

    }
    private IEnumerator StartLocationService()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }


        yield return new WaitForSeconds(2);
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            print("isEnabledByUser-> NO");

            yield return new WaitForSeconds(3);
        }


        // Start service before querying location
        Input.location.Start();

       yield return new WaitForSeconds(2);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            userCurrentLat = Input.location.lastData.latitude;
            userCurrentLong = Input.location.lastData.longitude;
            // Access granted and location value could be retrieved
            Debug.Log("MY Latitude : " + userCurrentLat);
            Debug.Log("MY Longitude : " + userCurrentLong);
            userCoords.text = "Lat: " + userCurrentLat + "; Long: " + userCurrentLong;
        }

        // Stop service if there is no need to query location updates continuously
        isUpdating = !isUpdating;
        Input.location.Stop();

    }

    //public static bool isWithin(GeoCoordinate pt, GeoCoordinate sw, GeoCoordinate ne)
    //{
    //    return pt.Latitude >= sw.Latitude &&
    //           pt.Latitude <= ne.Latitude &&
    //           pt.Longitude >= sw.Longitude &&
    //           pt.Longitude <= ne.Longitude;
    //}

    //calculate the distance between two coordinates, in KM
    double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    double ToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }

    public void GetTrackersGPSPosition()
    {
        foreach (ObjectTracker o in allObjectTrackers)
        {
            //ObjectTracker o = child.GetComponent<ObjectTracker>();
            string s = o.TargetCollectionResource.TargetPath;
            string[] subs = s.Split('/');
            string trackerNameAux = subs[subs.Length - 1].Split('.')[0]; //WTO name

            for (int j = 0; j < trackersListData["trackers"].Count; j++) //percorrer JSON dos trackers
            {
                string trackerNameJson = trackersListData["trackers"][j]["trackerName"];
                if (trackerNameJson.Equals(trackerNameAux))
                {
                    double lat = trackersListData["trackers"][j]["gpsPosition"]["latitude"];
                    double longi = trackersListData["trackers"][j]["gpsPosition"]["longitude"];
                    TrackersGPSData auxGO = new TrackersGPSData(o.gameObject.name, trackerNameAux, lat, longi);
                    TrackersAsGameObj.Add(auxGO);
                }

            }
        }
        hasProcessedGPSInfo = true;
        print("hasProcessedGPSInfo: " + hasProcessedGPSInfo);
    }

    //Raio de 10 m
    public void ActivateTracker()
    {
        foreach (TrackersGPSData go in TrackersAsGameObj)
        {
           
            if(GetDistance(userCurrentLat, userCurrentLong, go.gameObjLatitude, go.gameObjLongitude) < 0.01){
               
                foreach (ObjectTracker ot in allObjectTrackers)
                {
                    GameObject auxGO = ot.gameObject;
                    if (auxGO.name.Equals(go.gameObjName))
                    {
                        auxGO.SetActive(true);
                        ot.enabled = true;
                        string[] s = ot.TargetCollectionResource.TargetPath.Split('/');
                        activeTracker.text = s[s.Length-1];

                        //for(int i = 0; i < auxGO.transform.childCount; i++)
                        //{
                        //    GameObject child = auxGO.gameObject.transform.GetChild(i).gameObject;
                        //    ObjectTrackable trackable = child.GetComponent<ObjectTrackable>();
                        //    print("trackable name: " + trackable.TargetPattern);
                        //    print("trackable Drawable name: " + trackable.Drawable);
                        // SET DRAWABLE FROM ASSET BUNDLE HERE!!!!! (TO many updates)
                        //}
                    }
                    else
                    {
                        if(ot.gameObject.activeSelf)
                            ot.gameObject.SetActive(false);
                    }
                    
                }
                
            }
            //print("Game Object Name: " + go.gameObjName);
            //print("WTO Name in Unity: " + go.wtoName);
            //print("JSON lat: " + go.gameObjLatitude);
            //print("JSON longi: " + go.gameObjLongitude);
        }
    }
}
