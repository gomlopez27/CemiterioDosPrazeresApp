using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIMapSpecifications : MonoBehaviour
{
    public string id;
    public string latitude;
    public string longitude;
    //public GameObject InfoPanel;
    //public GameObject Directions;
    //public GameObject Player;
    // Start is called before the first frame update
    private void Awake()
    {
        //GameObject poi = this.transform.GetChild(0).gameObject;
        //GameObject pinpoint = poi.transform.GetChild(0).gameObject;
        //PoiOnMap poionmap = pinpoint.AddComponent<PoiOnMap>();
        //poionmap.SetVariables(id, latitude, longitude);
        //poionmap.SetGameObjects(InfoPanel, Directions, Player);
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVariables(string id, string lat, string longi)
    {
        this.id = id;
        this.latitude = lat;
        this.longitude = longi;

    }
    //public void SetGameObjects(GameObject InfoPanel, GameObject Directions, GameObject Player)
    //{
    //    this.InfoPanel = InfoPanel;
    //    this.Directions = Directions;
    //    this.Player = Player;

    //}

    public string GetId()
    {
        return id;
    }

    public string GetLatitude()
    {
        return latitude;
    }

    public string GetLongitude()
    {
        return longitude;
    }


    //private void OnMouseDown()
    //{
    //    print("Clicked on " + id);
    //    print("latitude " + latitude);
    //    print("longitude " + longitude);
    //    //InfoPanel.SetActive(true);
    //    //JazIdOnPanel.text = id;
    //}


}
