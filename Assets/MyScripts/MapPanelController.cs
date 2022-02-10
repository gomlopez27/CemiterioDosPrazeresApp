using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanelController : MonoBehaviour
{
    [SerializeField]
    GameObject Map;
    [SerializeField]
    GameObject InfoPanel;
    [SerializeField]
    GameObject FilterPanel;

    List<GameObject> pois;
    // Start is called before the first frame update
    void Start()
    {
        pois = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (pois.Count == 0)
        {
            pois = Map.GetComponent<MySpawnOnMap>().GetSpawnedPois();

        }
        else
        {
            if (InfoPanel.activeInHierarchy || FilterPanel.activeInHierarchy)
            {
                DeactivatePoiClick();
                Map.GetComponent<MapPanZoom>().enabled = false;
            }
            else
            {
                if (!pois[0].transform.GetChild(0).GetComponent<PoiClicked>().enabled)
                {
                    ActivatePoiClick();
                }
                Map.GetComponent<MapPanZoom>().enabled = true;


            }
        }

        if (Input.touchCount == 2)
        {
            DeactivatePoiClick();
        }
        else
        {
            ActivatePoiClick();
        }

    }


    void DeactivatePoiClick()
    {
        foreach(GameObject p in pois)
        {
            p.transform.GetChild(0).GetComponent<PoiClicked>().enabled = false;
        }
    }

    void ActivatePoiClick()
    {
        foreach (GameObject p in pois)
        {
            p.transform.GetChild(0).GetComponent<PoiClicked>().enabled = true;
        }
    }

   
}
