using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wikitude;
using System.IO;
using SimpleJSON;
using UnityEngine.Events;
using System;

public class ObjectTrackersSwap : MonoBehaviour
{

    public ObjectTracker o;
    public ObjectTrackable t;
    public GameObject augmententation;
    ObjectTarget target;
    public GameObject GuitarOne;
    public GameObject GuitarTwo;
    [SerializeField]
    public Text GuitarText;
    public GameObject MoreInfoButton; 
    private TargetInformation targetInformationScript;


    private void Start()
    {
        createObjectTrackers();
        //print(t.TargetPattern);
        //Debug.Log("colletion 1 is active: " + o.gameObject.activeSelf);
        //Debug.Log("colletion 2 is active: " + o2.gameObject.activeSelf);
    }
    public void OnGuitarOneTrack()
    {
        GuitarOne.SetActive(true);
        GuitarTwo.SetActive(false);
        GuitarText.text = "Guitar One";
        

    }

    public void OnGuitarTwoTrack()
    {
        GuitarTwo.SetActive(true);
        GuitarOne.SetActive(false);
        GuitarText.text = "Guitar Two";

    }
    
    

    private void Update()
    {
        //if(t.OnObjectRecognized.GetPersistentEventCount() != 0)
        //print("GetPersistentEventCount " + t.OnObjectRecognized.GetPersistentEventCount());
    }

    //UnityAction<ObjectTarget> m_MyFirstAction;

    //void MyFunction(ObjectTarget o)
    //{
    //    //Add to the number
    //    //Display the number so far with the message
    //    Debug.Log("First Added : ");
    //}
    public void createObjectTrackers()
    {
        string json = File.ReadAllText(Application.dataPath + "/Resources/tracker-guitarras-info.json");
        JSONNode itemsData = JSON.Parse(json);
       
        //Debug.Log(itemsData["wtoName"]);
        //Debug.Log("Count: " + itemsData["targets"].Count);


       
        GameObject trackerParent = Instantiate(o.gameObject);
        o.TargetCollectionResource = new TargetCollectionResource();
        o.TargetCollectionResource.UseCustomURL = true;
        o.TargetCollectionResource.TargetPath = "file://C:/Users/Giovanna/Documents/WTOS/tracker-guitarras.wto";
        //o.TargetCollectionResource.TargetPath = "file://" + Application.dataPath + "/StreamingAssets/Wikitude/tracker-guitarras.wto";

        //Handling the original trackable inside ObjectTracker
        Transform child = o.gameObject.transform.GetChild(0);
        ObjectTrackable trackable = child.GetComponent<ObjectTrackable>();
        trackable.TargetPattern = itemsData["targets"][0]["targetName"];
        trackable.Drawable = augmententation;
        
        trackable.OnObjectRecognized.AddListener(OnObjectRecognized);
        //trackable.OnObjectRecognized.AddListener((objTarget) => {
        //    MoreInfoButton.SetActive(true);
        //    targetInformationScript.LoadData(objTarget);
        //});

        //trackable.OnObjectLost.AddListener((objTarget) => {
        //    MoreInfoButton.SetActive(false);
        //});
        //m_MyFirstAction += MyFunction;

        //Creating other trackables based on the original one
        for (int i = 0; i < itemsData["targets"].Count - 1; i++)
        {
           GameObject goTrackable =  Instantiate(trackable.gameObject, trackerParent.transform);
           ObjectTrackable trackableAux = goTrackable.GetComponent<ObjectTrackable>();
           trackableAux.TargetPattern = itemsData["targets"][i + 1]["targetName"];
           trackableAux.OnObjectRecognized.AddListener(OnObjectRecognized);
            //trackableAux.OnObjectRecognized.AddListener((objTarget) => {
            //    MoreInfoButton.SetActive(true);
            //    targetInformationScript.LoadData(objTarget);
            //});

            //trackableAux.OnObjectLost.AddListener((objTarget) => {
            //    MoreInfoButton.SetActive(false);
            //});

            //trackableAux.OnObjectRecognized.AddListener(m_MyFirstAction);
        }
        

        //Debug.Log(o.TargetCollectionResource.UseCustomURL);
        //Debug.Log(o.TargetCollectionResource.TargetPath);
        //Debug.Log(trackableChild.gameObject.activeSelf);
        //Debug.Log(trackableChild.gameObject.name);

    }

   

    private UnityAction<ObjectTarget> TestActionOnRecognized()
    {
        Debug.Log("ON RECOGNIZED");
        return null;
    }

    public void OnObjectRecognized(ObjectTarget o)
    {
        Debug.Log("Name: " + o.Name);

    }

    //[System.Serializable]
    //public class OnObjectRecognized2Event : UnityEvent<ObjectTarget>
    //{
    //}



}
