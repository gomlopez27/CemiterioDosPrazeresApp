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
                DeactiveMapMovement();
            }
            else
            {
                if (!pois[0].transform.GetChild(0).GetComponent<PoiClicked>().enabled)
                {
                    ActivatePoiClick();
                }
                ActiveMapMovement();

            }
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

    void DeactiveMapMovement()
    {
        Map.GetComponent<MapPanning>().enabled = false;
        Map.GetComponent<QuadTreeCameraMovement>().enabled = false;
    }
    void ActiveMapMovement()
    {
        Map.GetComponent<MapPanning>().enabled = true;
        Map.GetComponent<QuadTreeCameraMovement>().enabled = true;
    }
}
