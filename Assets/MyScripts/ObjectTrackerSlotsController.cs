using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Wikitude;
using SimpleJSON;

[System.Serializable]
public class GPSPoint
{
    public double latitude;
    public double longitude;

    public GPSPoint(double latitudePoint, double longitudePoint)
    {
        this.latitude = latitudePoint;
        this.longitude = longitudePoint;
    }
}

[System.Serializable]
public class GPSPointDecimalPart
{
    public long latitude;
    public long longitude;

    public GPSPointDecimalPart(GPSPoint coordinates)
    {
        //int auxLatDecimalPart = (int)(coordinates.latitude - Math.Truncate(coordinates.latitude));
        //int auxLngDecimalPart = (int)(coordinates.longitude - Math.Truncate(coordinates.longitude));
        string Lat = coordinates.latitude.ToString();
        string Lng = coordinates.longitude.ToString();
        this.latitude = CheckNumberOfDigits(Lat);
        this.longitude = CheckNumberOfDigits(Lng);

    }

    long CheckNumberOfDigits(string coordinate)
    {
        string sCoordinate = coordinate.ToString();
        string decimalPart = sCoordinate.Split(',')[1];

        int nrOfDigits = decimalPart.Length;

        if (nrOfDigits < 10)
        {
            Debug.Log("ENTREI NO IF");
            int valuesLeft = 10 - nrOfDigits;
            string zeros = AppendZeros(valuesLeft);
            decimalPart += zeros;
            long valueWithZeros = ConvertStringToNumber(decimalPart);
            return valueWithZeros;
  
        }
        else if (nrOfDigits > 10)
        {
            Debug.Log("nrOfDigits > 10: " + decimalPart);

            //string digit10 = decimalPart.Split(decimalPart[9])[0];
            string digit10 = decimalPart.Substring(0, 10);

            long value = ConvertStringToNumber(digit10);
            return value;
        }
        else
        {
            long value = ConvertStringToNumber(decimalPart);
            return value;
        }

       
    }

    long ConvertStringToNumber(string s)
    {
        try
        {
            long value = Int64.Parse(s);
            return value;
        }
        catch (FormatException e)
        {
            Console.WriteLine(e.Message);
            return 0;
        }

    }

    string AppendZeros(int nrOfTimes)
    {
        string aux = "";
        for(int i = 0; i < nrOfTimes; i++)
        {
            aux += "0";
        }
        return aux;
    }
}

[System.Serializable]
public class JazSlotsCollection
{
    public Dictionary<string, string> JazSlots = new Dictionary<string, string>();

    public JSONNode Serialize()
    {
        var slotsList = new JSONArray();

        foreach (var v in JazSlots)
        {
            var obj = new JSONObject();
            obj["jazId"] = v.Key;
            obj["SlotName"] = v.Value;

            slotsList.Add(obj);
        }
        return slotsList;
    }
}
public class ObjectTrackerSlotsController : MonoBehaviour
{
    [SerializeField]
    GPSPoint topLeft;
    [SerializeField]
    GPSPoint topRight;
    [SerializeField]
    GPSPoint bottomLeft;
    [SerializeField]
    GPSPoint bottomRight; //Ponto origem
    [SerializeField]
    int horizontalSlots; //Número de slots na horizontal
    [SerializeField]
    int verticalSlots; //Número de slots na vertical
    [SerializeField]
    Text ActiveTracker; //debug
    [SerializeField]
    Text userCurrentZone;//debug
    //horizontal(X) - longitude
    //vertical(y) - latitude
    private GPSPointDecimalPart topLeftDecimal; //apenas parte decimal do
    private GPSPointDecimalPart topRightDecimal;
    private GPSPointDecimalPart bottomLeftDecimal;
    private GPSPointDecimalPart bottomRightDecimal;
    private long horizontalDiference; //"Largura" do retangulo
    private long verticalDiference; //"Altura" do retangulo
    private double horizontalDivisionValue; //"Largura" de um slot do retangulo
    private double verticalDivisionValue; //"Altura" de um slot do retangulo
    private bool isUpdating;
    private JSONNode trackersListData;
    private ObjectTracker[] allObjectTrackers;
    private double userCurrentLat;
    private double userCurrentLong;
    private List<GameObject> ObjecTrackersGO;
    private string userCurrentSlot = "";
    private bool hasLocation;
   
   
    private void Start()
    {
        string filePath = Application.persistentDataPath + "/JazSlotsList.json";
        TextAsset json = Resources.Load<TextAsset>("TrackersData");
        trackersListData = JSON.Parse(json.ToString());
        topLeftDecimal = new GPSPointDecimalPart(topLeft);
        topRightDecimal = new GPSPointDecimalPart(topRight);
        bottomLeftDecimal = new GPSPointDecimalPart(bottomLeft);
        bottomRightDecimal = new GPSPointDecimalPart(bottomRight);
        ObjecTrackersGO = new List<GameObject>();
        CalculateDifferences();
        CalculateDivisionFactor();
        print(filePath);
        TextAsset jsonPoisInMap = Resources.Load<TextAsset>("MapPopularPOI");
        JSONNode PoisJson = JSON.Parse(jsonPoisInMap.ToString());

        if (!System.IO.File.Exists(filePath))
        {
           
            printPoisSlotNames(filePath, PoisJson);
        }
        else
        {
            string jsonSlots = File.ReadAllText(filePath);
            JSONNode SlotsJSON = JSON.Parse(jsonSlots.ToString());
            print("SlotsJSON count: " + SlotsJSON.Count);
            if(PoisJson["pois"].Count > SlotsJSON.Count)
            {
                printPoisSlotNames(filePath, PoisJson);
            }
        }

        //////Teste jaz
        //GPSPointDecimalPart location = new GPSPointDecimalPart(new GPSPoint(38.71405789, -9.17045588));
        //int x = CalculateSlotX(location);
        //int y = CalculateSlotY(location);
        //print("Zona " + x + " " + y);
    }

    private void Update()
    {
        if (!isUpdating)
        {
            StartCoroutine(StartLocationService());
            isUpdating = !isUpdating;
        }

        if(ObjecTrackersGO.Count == 0)
        {
            ObjecTrackersGO = this.GetComponent<RuntimeObjectTracker>().GetObjectTrackers();
        }

        if (ObjecTrackersGO.Count > 0 && isUpdating && hasLocation)
        {
            ActivateObjectTracker();
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
            hasLocation = false;
            yield break;
        }
        else
        {
            hasLocation = true;
            userCurrentLat = Input.location.lastData.latitude;
            userCurrentLong = Input.location.lastData.longitude;
            StartCoroutine(UserCurrentSlot());

            // Access granted and location value could be retrieved
            Debug.Log("MY Latitude : " + userCurrentLat);
            Debug.Log("MY Longitude : " + userCurrentLong);
            //userCoords.text = "Lat: " + userCurrentLat + "; Long: " + userCurrentLong;
        }

        // Stop service if there is no need to query location updates continuously
        isUpdating = !isUpdating;
        Input.location.Stop();

    }

    /*Calculo das diferenças dos valores das coordenadas correspondendo a largura e altura do retangulo*/
    void CalculateDifferences()
    {
        //Horizontal - eixo X/"Largura"/longitude
        horizontalDiference = bottomLeftDecimal.longitude - bottomRightDecimal.longitude;
        verticalDiference = topRightDecimal.latitude - bottomRightDecimal.latitude;
        print("horizontalDiference: " + bottomLeftDecimal.longitude +"-"+ bottomRightDecimal.longitude +"="+ horizontalDiference);
        print("verticalDiference: " + topRightDecimal.latitude + "-" + bottomRightDecimal.latitude + "=" + + verticalDiference);
    }

    void CalculateDivisionFactor()
    {
        horizontalDivisionValue = horizontalDiference / horizontalSlots;
        verticalDivisionValue = verticalDiference / verticalSlots;
        print("horizontalDivisionValue: " + horizontalDivisionValue);
        print("verticalDivisionValue: " + verticalDivisionValue);
    }

    int CalculateSlotX(GPSPointDecimalPart location)
    {
        //GPSPointDecimalPart userLocation = new GPSPointDecimalPart(new GPSPoint(userLatitude, userLongitude));

        //Slot HORIZONTAL: (LONGITUDE DO PONTO  - LONGITUDE DO PONTO 0,0) / horizontalDivisionValue
        print("Location: " + location.longitude);
        print("bootm right dec: " + bottomRightDecimal.longitude);
        double aux = (location.longitude - bottomRightDecimal.longitude) / horizontalDivisionValue;
        int xValue = (int)Math.Truncate(aux);
        return xValue;
    }

    int CalculateSlotY(GPSPointDecimalPart location)
    {
        //GPSPointDecimalPart userLocation = new GPSPointDecimalPart(new GPSPoint(userLatitude, userLongitude));

        double aux = (location.latitude - bottomRightDecimal.latitude) / verticalDivisionValue;
        int yValue = (int)Math.Truncate(aux);
        return yValue;
    }

    IEnumerator UserCurrentSlot()
    {
        GPSPointDecimalPart userLocation = new GPSPointDecimalPart(new GPSPoint(userCurrentLat, userCurrentLong));
        int x = CalculateSlotX(userLocation);
        int y = CalculateSlotY(userLocation);
        string aux = "Zona" + x + y;
        if (!userCurrentSlot.Equals(aux))
        {
            userCurrentSlot = aux;

        }
        userCurrentZone.text = userCurrentSlot;
        print("userCurrentSlot: " + userCurrentSlot);
        yield return new WaitForSeconds(5f);
    }
    public void ActivateObjectTracker()
    {
        foreach (GameObject go in ObjecTrackersGO)
        {
          
            ObjectTracker ot = go.GetComponent<ObjectTracker>();
            string[] s = ot.TargetCollectionResource.TargetPath.Split('/');
            string otName = s[s.Length - 1];
        
            //if (userCurrentSlot.Equals(otName)) //TODO: mudar de volta para ir buscar o nome ao WTO
            if (userCurrentSlot.Equals(go.name))
            {
                go.SetActive(true);
                ot.enabled = true;
                ActiveTracker.text = go.name;
            }
            else
            {
                go.SetActive(false);
                ot.enabled = false;
     
                //if (ot.gameObject.activeSelf)
                //    ot.gameObject.SetActive(false);
            }

         

        }
    }

    void printPoisSlotNames(string filePath, JSONNode PoisJson)
    {
     
        Dictionary<string, string> aux = new Dictionary<string, string>();

        for(int i = 0; i < PoisJson["pois"].Count; i++)
        {
            string id = PoisJson["pois"][i]["ID"];
            double lat = PoisJson["pois"][i]["latitude"];
            double lng = PoisJson["pois"][i]["longitude"];
            print(id);
            GPSPointDecimalPart jazLocation = new GPSPointDecimalPart(new GPSPoint(lat, lng));
            int x = CalculateSlotX(jazLocation);
            int y = CalculateSlotY(jazLocation);
            string slotName = "Zona" + x.ToString() + y.ToString();
            aux.Add(id, slotName);

        }
     
        JazSlotsCollection jazSlots = new JazSlotsCollection();
        jazSlots.JazSlots = aux;
        string jsonToWrite = jazSlots.Serialize().ToString(3);
        System.IO.File.WriteAllText(filePath, jsonToWrite);

    }

}
