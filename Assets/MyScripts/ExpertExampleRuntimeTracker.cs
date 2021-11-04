using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using Wikitude;
public class ExpertExampleRuntimeTracker : MonoBehaviour
{

    public GameObject DefaultDrawable;

    private string _defaultAddress = "file://C:/Users/Giovanna/Documents/WTOS/trackerGuitarThree.wto";
    private GameObject _activeTracker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTracker()
    {
        CreateObjectTracker(_defaultAddress, DefaultDrawable);
    }

    private void CreateObjectTracker(string address, GameObject drawable)
    {
        _activeTracker = new GameObject("ObjectTracker");
        ObjectTracker tracker = _activeTracker.AddComponent<ObjectTracker>();
        tracker.TargetCollectionResource = new TargetCollectionResource();

        ObjectTrackable trackable = (new GameObject("trackable")).AddComponent<ObjectTrackable>();
        trackable.Drawable = drawable;
        trackable.transform.parent = _activeTracker.transform;

        SetTargetCollectionResource(tracker.TargetCollectionResource, address);
    }

    private void SetTargetCollectionResource(TargetCollectionResource resource, string address)
    {
        resource.UseCustomURL = true;
        resource.TargetPath = address;

        resource.OnFinishLoading.AddListener(() => {
            print("OnFinishLoading");
        });
        resource.OnErrorLoading.AddListener((error) => {
            print("OnErrorLoading");
           
        });
    }
}
