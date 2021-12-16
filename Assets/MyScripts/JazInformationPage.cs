using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazInformationPage : MonoBehaviour
{
    [SerializeField] GameObject OnePersonalityPage;
    [SerializeField] GameObject MultiplePersonalitiesPage;
    [SerializeField] GameObject OnePersonalityContent;

    public void SetSinglePersonality(Personality personality)
    {
        OnePersonalityPage.SetActive(true);
        MultiplePersonalitiesPage.SetActive(false);
        Image personalityImage = OnePersonalityPage.transform.Find("Info/PersonImage").GetComponent<Image>();
        Text personalityName = OnePersonalityPage.transform.Find("Info/PersonName").GetComponent<Text>();
        //Text personalityBio = OnePersonalityContent.transform.Find("BioText").GetComponent<Text>();
        Text personalityBio = OnePersonalityContent.GetComponent<Text>();
        Davinci.get().load(personality.ImageUrl).setCached(true).into(personalityImage).start();
        personalityName.text = personality.Name;
        personalityBio.text = personality.Description;

    }

    public void SetMultiplePersonalitiesList(string jazImageUrl, List<Personality> PersonalitiesList)
    {
        MultiplePersonalitiesPage.SetActive(true);
        OnePersonalityPage.SetActive(false);
        Image jazImage = MultiplePersonalitiesPage.transform.Find("HeaderImage").gameObject.GetComponent<Image>();
        Davinci.get().load(jazImageUrl).setCached(true).into(jazImage).start();
        GameObject ListArea = MultiplePersonalitiesPage.transform.Find("ScrollArea/Content").gameObject;
        GameObject PersonalityItem = ListArea.transform.GetChild(0).gameObject;
        PersonalityItem.SetActive(true);

        if (ListArea.transform.childCount > 1)
        {
            for (int i = 1; i < ListArea.transform.childCount; i++)
            {
                Destroy(ListArea.transform.GetChild(i).gameObject);
            }
        }

        print("childCount " + ListArea.transform.childCount);
        for (int i = 0; i < PersonalitiesList.Count; i++)
        {
            string personId = PersonalitiesList[i].UriId;
            string personName = PersonalitiesList[i].Name;
            GameObject g = Instantiate(PersonalityItem, ListArea.transform);
            g.name = "person-" + personId;
            g.transform.Find("PersonName").GetComponent<Text>().text = personName;
            //Button SeeMore = g.transform.Find("MoreBtn").GetComponent<Button>();
            Button BtnItem = g.transform.GetComponent<Button>();
            print("" + BtnItem.name);
            Personality personality = PersonalitiesList[i];
            BtnItem.onClick.AddListener(() =>
            {

                print("on click" + personality.Name);
                OnePersonalityPage.SetActive(true);
                SetSinglePersonality(personality);
            });
        }
        PersonalityItem.SetActive(false);


    }

  
}
