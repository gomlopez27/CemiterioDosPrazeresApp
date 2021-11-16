using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wikitude;
using System.IO;
using SimpleJSON;
using UnityEngine.Events;
using System;
using UnityEngine.Networking;

public class RuntimeObjectTracker : MonoBehaviour
{

    public const string DEFAULT_AUGM = "default";
    [SerializeField] GameObject WikitudeCamera;
    //[SerializeField] GameObject defaultAugmententation;
    [SerializeField] Button MoreInfoButton;
    //[SerializeField] Image imageUI;
    //[SerializeField] Text nameUI;
    //[SerializeField] Text descriptionUI;
    [SerializeField] GameObject defaultAugmentationPanel;
    [SerializeField] Image defaultAugmentationImage;
    [SerializeField] Text defaultAugmentationText;
    [SerializeField] GameObject ARCanvas;
    [SerializeField] GameObject InfoCanvas;
    [SerializeField] Text TargetTitle;
    [SerializeField] GameObject OnePersonalityPage;
    [SerializeField] GameObject MultiplePersonalitiesPage;
    [SerializeField] GameObject OnePersonalityContent;
    [SerializeField] GameObject LoadingText;

    private GameObject trackerObject;
    private string onRecognizedObjName;
    private JSONNode trackersListData;
    private AssetBundle myLoadedAssetBundle;
    private bool hasCreatedTrackers;
    private bool hasDeactivatedTrackers;
    private int trackerConter;
    private bool MoreInfoBtnClicked;
    private bool objTargetDestroyed;
    private ObjectTarget CurrentObjTarget;
    private GameObject currentAugmentation;
    private ObjectTrackable currentObjectTrackable;
    private bool dataLoaded;
    private List<GameObject> createdObjTrackers;
    private bool lauchDefaultAugmentation;
    private void Awake()
    {

      
        TextAsset jsonTrackersData = Resources.Load<TextAsset>("TrackersData");
        trackersListData = JSON.Parse(jsonTrackersData.ToString());
        trackerConter = 0;
        createdObjTrackers = new List<GameObject>();
        //string url = Application.dataPath + "/AssetBundles/augmentations";
        //myLoadedAssetBundle = AssetBundle.LoadFromFile(url);
    }


    private void Start()
    {
        MoreInfoButton.onClick.AddListener(LoadMoreInfo);

        myLoadedAssetBundle = MainDataHolder.myAssetBundle;

        foreach(GameObject go in MainDataHolder.augmentationsGO)
        {
            print("augmentationsGO: " + go.name);
        }
//        string bundleUrl = "https://pasev.di.fct.unl.pt/contentFiles/Giovanna/AssetBundles/";

        //#if UNITY_ANDROID && !UNITY_EDITOR
        //        print("UNITY_ANDROID");
        //        if(myLoadedAssetBundle == null){
        //            StartCoroutine(LoadAssetBundle(bundleUrl + "augmentations-Android"));
        //        }
        //        //testAssetBundle("augmentations-Android");
        //#elif UNITY_EDITOR
        //        if (myLoadedAssetBundle == null)
        //        {
        //            print("UNITY_EDITOR");
        //            StartCoroutine(LoadAssetBundle(bundleUrl + "augmentations-Windows"));
        //            //testAssetBundle("augmentations-Windowss");
        //        }
        //#endif

    }



    void testAssetBundle(string name)
    {
    
        if (myLoadedAssetBundle != null)
        {
            print("myLoadedAssetBundle != null");
            myLoadedAssetBundle.Unload(false);
        }


        myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
    
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        //foreach (string n in myLoadedAssetBundle.GetAllAssetNames())
        //{
        //    print("asset: " + n);
        //}
        //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("JazCarolinaBeatriz");
        //Instantiate(prefab);

        //myLoadedAssetBundle.Unload(false);
    }
    
    private void Update()
    {
        if (!hasCreatedTrackers && MainDataHolder.augmentationsGO != null)
        {
            CreateObjectTrackers();
            print("CREATED OBJ TRACKERS" + createdObjTrackers.Count);
            LoadingText.SetActive(false);
        }
        //if (lauchDefaultAugmentation)
        //{
        //    StartCoroutine(DefaultAugmentationCreation(CurrentObjTarget.Name));
        //}

        //if (hasCreatedTrackers && trackerConter == wtoPaths.Length && !hasDeactivatedTrackers)
        //{
        //    DeactivateAllObjectTrackers();

        //}

    }


    public void CreateObjectTrackers()
    {
        TextAsset json = Resources.Load<TextAsset>("CreateObjectTrackersOld"); //TODO: change back
        //TextAsset json = Resources.Load<TextAsset>("targetNamesPerTracker");

        JSONNode ObjectTrackersList = JSON.Parse(json.ToString());
        for (int i = 0; i < ObjectTrackersList.Count; i++)
        {
            trackerConter = i + 1;
            //GameObject trackerObject = new GameObject("ObjectTracker-" + trackerConter);
            GameObject trackerObject = new GameObject(ObjectTrackersList[i]["wtoName"]);
            ObjectTracker objectTracker = trackerObject.AddComponent<ObjectTracker>();

            objectTracker.TargetCollectionResource = new TargetCollectionResource();
            objectTracker.TargetCollectionResource.UseCustomURL = true;
            objectTracker.TargetCollectionResource.TargetPath = ObjectTrackersList[i]["wtoUrl"];

            for (int k = 0; k < ObjectTrackersList[i]["targets"].Count; k++)
            {
                string targetName = ObjectTrackersList[i]["targets"][k]["targetName"];

                ObjectTrackable objTrackable = (new GameObject("ObjectTrackable-" + targetName)).AddComponent<ObjectTrackable>();
                objTrackable.transform.parent = trackerObject.transform;
                objTrackable.TargetPattern = targetName;
                string augmentation = ObjectTrackersList[i]["targets"][k]["augmentation"];
                //Debug.Log("augmentation: " + augmentation);
                string assetBundleName = "assets/augmentationsprefabs/" + augmentation + ".prefab";
                string resourcesOathName = "augmentations/" + augmentation + ".prefab";

                if (!augmentation.Equals(DEFAULT_AUGM))
                {
                    foreach (GameObject go in MainDataHolder.augmentationsGO)
                    {
                        if (go.name.Equals(augmentation))
                        {
                            objTrackable.Drawable = go;
                            objTrackable.ExtendedTracking = true;
                            break;
                        }
                    }
                    //if (/*myLoadedAssetBundle != null && */myLoadedAssetBundle.Contains(assetBundleName))
                    //{
                    //    objTrackable.Drawable = myLoadedAssetBundle.LoadAsset<GameObject>(augmentation);

                    //}
                    //else
                    //{
                    //    GameObject go = Resources.Load<GameObject>(resourcesOathName);
                    //    print("BUscar augmenteation as minhas pastas " + go.name);

                    //    objTrackable.Drawable = go;

                    //}

                }
                //if (myLoadedAssetBundle.Contains(assetBundleName))
                //{
                //    objTrackable.Drawable = myLoadedAssetBundle.LoadAsset<GameObject>(augmentation);
                //}
                //else
                //{
                //    //Vai para default augmentation
                //    //objTrackable.Drawable = null;
                //    objTrackable.Drawable = defaultAugmentationPanel;
                //}

                

                objTrackable.OnObjectRecognized.AddListener((objTarget) =>
                {
                    MoreInfoButton.gameObject.SetActive(true);               
                    CurrentObjTarget = objTarget;
                    print("CurrentObjTarget name: " + CurrentObjTarget.Name);
                    //print("augmentation name: " + augmentation);
                    //if (objTrackable.Drawable == defaultAugmentationPanel)
                    if (augmentation.Equals(DEFAULT_AUGM))
                    {
                        print("DEFAULT_AUGM");
                        //lauchDefaultAugmentation = true;
                        /*StartCoroutine(*/DefaultAugmentationCreation(CurrentObjTarget.Name);
                        defaultAugmentationPanel.SetActive(true);

                    }
                });

                objTrackable.OnObjectLost.AddListener((objTarget) =>
                {
                    MoreInfoButton.gameObject.SetActive(false);
                    if (augmentation.Equals(DEFAULT_AUGM))
                    {
                        defaultAugmentationPanel.SetActive(false);

                    }
                    //lauchDefaultAugmentation = false;
                });
            }

            createdObjTrackers.Add(trackerObject);
        }


        //myLoadedAssetBundle.Unload(false);
        //trackerObject.SetActive(false);
        //objectTracker.enabled = false;
      

        hasCreatedTrackers = true;

    }

    public List<GameObject> GetObjectTrackers()
    {
        return createdObjTrackers;
    }

    public void DeactivateAllObjectTrackers()
    {
        /*Setting all object tracker game objects to false, since it is only allowed to have one active and that is activated via GPSLocation script*/
        //allObjectTrackers = FindObjectsOfType<ObjectTracker>(true);
        print("From RuntimeObjecTracker -> createdObjTrackers: " + createdObjTrackers.Count);

        //GameObject.Find("ObjectTracker-1").SetActive(false);
        for (int i = 0; i < createdObjTrackers.Count; i++)
        {
            print(createdObjTrackers[i].gameObject.name);
            createdObjTrackers[i].SetActive(false);
        }

        hasDeactivatedTrackers = true;

    }

    //TODO: mudar isto para variavel global, acho que funciona na mesma
    public void OnObjectRecognized(ObjectTarget o)
    {
        onRecognizedObjName = o.Name;
        //MoreInfoButton.SetActive(true);
    }

    IEnumerator LoadAssetBundle(string url)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();
        print("request.isDone " + request.isDone);

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("error: " + request.error);
        }

        if (request.error != null)
        {
            Debug.LogError(request.error);
            Debug.Log("AssetBundle couldn't be loaded!");
        }
        else
        {
            print("NOT REQUEST ERROR");

            myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(request);

            if (myLoadedAssetBundle != null)
            {
                print("myLoadedAssetBundle != null");
                myLoadedAssetBundle.Unload(false);
            }

        }
   
    }


    public void StopAugmentation()
    {
        if(CurrentObjTarget != null)
        {
            GameObject target = GameObject.Find("ObjectTrackable-" + CurrentObjTarget.Name);
            target.SetActive(false);

        }
    }

    public /*IEnumerator*/ void DefaultAugmentationCreation(string targetName)
    {
        JSONNode jaz = this.GetComponent<JazInformations>().GetJaz(targetName);
        //JSONNode jaz = this.GetComponent<JazInformations>().GetJaz("2271");
        int personCout = jaz["personalidades"].Count;

        
        if (personCout > 1)
        {
            //defaultAugmentationImage.sprite = imageUI.sprite;
            defaultAugmentationText.text = "Este jazigo tem " + personCout + " personalidades";
            for(int i = 0; i < personCout; i++)
            {
                string personImageUrl = jaz["personalidades"][i]["imageURL"];
                Davinci.get().load(personImageUrl).setCached(true).into(defaultAugmentationImage).start();
                //yield return new WaitForSeconds(1.5f);
                //defaultAugmentationPanel.SetActive(true);
            }
        }
        else
        {
            string personImageUrl = jaz["personalidades"][0]["imageURL"];
            Davinci.get().load(personImageUrl).setCached(true).into(defaultAugmentationImage).start();
            //yield return new WaitForSeconds(1);
           // defaultAugmentationPanel.SetActive(true);

            defaultAugmentationText.text = jaz["personalidades"][0]["nome"];
        }


    }

    /*---------------Loading target data, when target recognized---------------------------*/
    public void LoadMoreInfo()
    {
        WikitudeCamera.SetActive(false);
        StopAugmentation();
        string targetName = CurrentObjTarget.Name;
        print(targetName);
        JSONNode jaz = this.GetComponent<JazInformations>().GetJaz(targetName); //TODO: change back
        //JSONNode jaz = this.GetComponent<JazInformations>().GetJaz("1500");
        ARCanvas.SetActive(false);
        InfoCanvas.SetActive(true);
        TargetTitle.text = jaz["tipoJaz"] + " " + targetName;

        if (jaz["personalidades"].Count > 1)
        {
           //MultiplePersonalitiesPage.SetActive(true);
            this.GetComponent<JazInformationPage>().SetMultiplePersonalitiesList(jaz["jazImage"], jaz["personalidades"]);
        }
        else
        {
            //OnePersonalityPage.SetActive(true);
            //MultiplePersonalitiesPage.SetActive(false);
            this.GetComponent<JazInformationPage>().SetSinglePersonality(jaz["personalidades"][0]);
            
        }
        dataLoaded = true;
     
    }

    public void BackButtonFromSinglePerson()
    {

        if (MultiplePersonalitiesPage.activeInHierarchy)
        {
            OnePersonalityPage.SetActive(false);
        }
        else
        {

            WikitudeCamera.SetActive(true);
            ARCanvas.SetActive(true);
            InfoCanvas.SetActive(false);
        }

    }
    //void SetSinglePersonality(JSONNode Personality)
    //{

    //    Image personalityImage = OnePersonalityContent.transform.Find("Info/PersonImage").GetComponent<Image>();
    //    Text personalityName = OnePersonalityContent.transform.Find("Info/PersonName").GetComponent<Text>();
    //    Text personalityBio = OnePersonalityContent.transform.Find("BioText").GetComponent<Text>();
    //    Davinci.get().load(Personality["imageURL"]).setCached(true).into(personalityImage).start();
    //    personalityName.text = Personality["nome"];
    //    personalityBio.text = Personality["description"];

    //}

    //void SetMultiplePersonalitiesList(string jazImageUrl, JSONNode PersonalitiesList)
    //{
    //    MultiplePersonalitiesPage.SetActive(true);
    //    OnePersonalityPage.SetActive(false);
    //    Image jazImage = MultiplePersonalitiesPage.transform.Find("HeaderImage").gameObject.GetComponent<Image>();
    //    Davinci.get().load(jazImageUrl).setCached(true).into(jazImage).start();
    //    GameObject ListArea = MultiplePersonalitiesPage.transform.Find("ScrollArea/Content").gameObject;
    //    GameObject PersonalityItem = ListArea.transform.GetChild(0).gameObject;
    //    PersonalityItem.SetActive(true);

    //    if(ListArea.transform.childCount > 1)
    //    {
    //        for (int i = 1; i < ListArea.transform.childCount; i++)
    //        {
    //            Destroy(ListArea.transform.GetChild(i).gameObject);
    //        }
    //    }

    //    print("childCount " + ListArea.transform.childCount);
    //    for (int i = 0; i < PersonalitiesList.Count; i++)
    //    {
    //        string personId = PersonalitiesList[i]["uriId"];
    //        string personName = PersonalitiesList[i]["nome"];
    //        GameObject g = Instantiate(PersonalityItem, ListArea.transform);
    //        g.name = "person-" + personId;
    //        g.transform.Find("PersonName").GetComponent<Text>().text = personName;
    //        //Button SeeMore = g.transform.Find("MoreBtn").GetComponent<Button>();
    //        Button BtnItem = g.transform.GetComponent<Button>();
    //        print("" + BtnItem.name);
    //        JSONNode Personality = PersonalitiesList[i];
    //        BtnItem.onClick.AddListener(() =>
    //        {

    //            print("on click" + Personality["nome"]);
    //            OnePersonalityPage.SetActive(true);
    //            SetSinglePersonality(Personality);
    //        });
    //    }
    //    PersonalityItem.SetActive(false);


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
            //imageUI.sprite = sprite;

        }
    }

   

}
