using Mapbox.Unity.MeshGeneration.Factories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiOnMap : MonoBehaviour
{
    string id;
    string location;
    string latitude;
    string longitude;
    GameObject InfoPanel;
    GameObject Directions;
    GameObject Player;
    //public GameObject InfoPanel;
    //public Text JazIdOnPanel;
    // Start is called before the first frame update
    void Start()
    {
        Button takeMeThereBtn = InfoPanel.transform.Find("TakeMeThereBtn").GetComponent<Button>();
        takeMeThereBtn.onClick.AddListener(TakeMeThere);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    GameObject go = hit.transform.gameObject;

                    //Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
                    //hit.collider.GetComponent<MeshRenderer>().material.color = newColor;
                    //print("Clicked on " + id);
                    InfoPanel.SetActive(true);
                    Text TextId = InfoPanel.transform.Find("JazId").GetComponent<Text>();
                    TextId.text = go.GetComponent<PoiOnMap>().id;
                }
            }
        }
#endif
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != null)
                {
                    GameObject go = hit.transform.gameObject;
                    //Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
                    //hit.collider.GetComponent<MeshRenderer>().material.color = newColor;
                    print("Clicked on " + go.GetComponent<PoiOnMap>().id);
                    
                    InfoPanel.SetActive(true);
                    Text TextId = InfoPanel.transform.Find("JazId").GetComponent<Text>();
                    TextId.text = go.GetComponent<PoiOnMap>().id;
                }
            }
        }
#endif
     
    }

    //public void SetVariables(string id, string lat, string longi)
    //{
    //    this.id = id;
    //    this.latitude = lat;
    //    this.longitude = longi;

    //}
    //public void SetGameObjects(GameObject InfoPanel, GameObject Directions, GameObject Player)
    //{
    //    this.InfoPanel = InfoPanel;
    //    this.Directions = InfoPanel;
    //    this.Player = InfoPanel;

    //}

    public void TakeMeThere()
    {
        print("TakeMeThere");
        //DirectionsFactory _directionsFact = GetComponent<DirectionsFactory>();
        //_directionsFact.enabled = true;
        InfoPanel.SetActive(false);
        print(this.transform.parent.parent.localPosition);
        //_directionsFact._waypoints.Add(Player.transform);
        //_directionsFact._waypoints.Add(this.transform);
    }

    //private void OnMouseDown()
    //{
    //    print("Clicked on " + id);
    //    print("Clicked on " + latitude);
    //    print("Clicked on " + longitude);
    //    //InfoPanel.SetActive(true);
    //    //JazIdOnPanel.text = id;
    //}




}
