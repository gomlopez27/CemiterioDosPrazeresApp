using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiClicked : MonoBehaviour
{
    string JazIdClicked;
    string thisJazId;
    GameObject PoiController;

    // Start is called before the first frame update
    void Start()
    {
        thisJazId = this.transform.parent.parent.name.Split('-')[1];
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
                    HandlePoiClicked(go);
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
                    // hit.collider.GetComponent<MeshRenderer>().material.color = newColor;
                    HandlePoiClicked(go);
                }
            }
        }
#endif

    }


    public void HandlePoiClicked(GameObject go)
    {
        if (PoiController != null)
        {
            PoiController.Destroy();
        }

        GameObject markerParent = go.transform.parent.gameObject;
        GameObject GOpoi = markerParent.transform.parent.gameObject;

        JazIdClicked = GOpoi.GetComponent<POIMapSpecifications>().GetId();

        if (thisJazId.Equals(JazIdClicked))
        {
 
            PoiController = new GameObject("PoiController-" + JazIdClicked);
            PoiController.transform.position = markerParent.transform.position;
            PoiController.AddComponent<PoiClickedController>();
            PoiController.GetComponent<PoiClickedController>().SetJazId(JazIdClicked);
        }




    }




}
