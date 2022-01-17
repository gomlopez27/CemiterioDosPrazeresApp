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
        Text birthDate = OnePersonalityPage.transform.Find("Info/Dates/dataNascValue").GetComponent<Text>();
        Text deathDate = OnePersonalityPage.transform.Find("Info/Dates/DataObitoValue").GetComponent<Text>();
        //Text personalityBio = OnePersonalityContent.transform.Find("BioText").GetComponent<Text>();
        Text personalityBio = OnePersonalityContent.GetComponent<Text>();
        Davinci.get().load(personality.ImageUrl).setCached(true).into(personalityImage).start();
        personalityName.text = personality.Name;
        personalityBio.text = personality.Biography;
        birthDate.text = PrintDateInText(personality.BirthDate);
        deathDate.text = PrintDateInText(personality.DeathDate);
    }
    
    public void SetSinglePersonality()
    {
        OnePersonalityPage.SetActive(true);
        MultiplePersonalitiesPage.SetActive(false);
        Image personalityImage = OnePersonalityPage.transform.Find("Info/PersonImage").GetComponent<Image>();
        Text personalityName = OnePersonalityPage.transform.Find("Info/PersonName").GetComponent<Text>();
        Text birthDate = OnePersonalityPage.transform.Find("Info/Dates/dataNascValue").GetComponent<Text>();
        Text deathDate = OnePersonalityPage.transform.Find("Info/Dates/DataObitoValue").GetComponent<Text>();
        //Text personalityBio = OnePersonalityContent.transform.Find("BioText").GetComponent<Text>();
        Text personalityBio = OnePersonalityContent.GetComponent<Text>();
        Davinci.get().load(MainDataHolder.clickedPersonality.ImageUrl).setCached(true).into(personalityImage).start();
        personalityName.text = MainDataHolder.clickedPersonality.Name;
        personalityBio.text = MainDataHolder.clickedPersonality.Biography;
        birthDate.text = PrintDateInText(MainDataHolder.clickedPersonality.BirthDate);
        deathDate.text = PrintDateInText(MainDataHolder.clickedPersonality.DeathDate);
    }

    private string PrintDateInText(List<int> date)
    {
        string fullDate, y, m, d = "";
        int year = date[0];
        int month = date[1];
        int day = date[2];

        y = year.ToString();
        if (month < 10 || day < 10)
        {
            m = "0" + month.ToString();
            d = "0" + day.ToString();
        }
        else
        {
            m = month.ToString();
            d = day.ToString();
        }

        fullDate = d + "/" + m + "/" + y;

        return fullDate;
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
        print("PersonalitiesList.Count " + PersonalitiesList.Count);
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
            print("personality:::: " + personality.Name);
            BtnItem.onClick.AddListener(() =>
            {

                print("on click" + personality.Name);
                OnePersonalityPage.SetActive(true);
                SetSinglePersonality(personality);
            });
        }
        PersonalityItem.SetActive(false);


    }

    public void SetMultiplePersonalitiesList(string jazImageUrl)
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
        print("PersonalitiesList.Count " + MainDataHolder.clickedMultiplePersonality.Count);
        for (int i = 0; i < MainDataHolder.clickedMultiplePersonality.Count; i++)
        {
            string personId = MainDataHolder.clickedMultiplePersonality[i].UriId;
            string personName = MainDataHolder.clickedMultiplePersonality[i].Name;
            GameObject g = Instantiate(PersonalityItem, ListArea.transform);
            g.name = "person-" + personId;
            g.transform.Find("PersonName").GetComponent<Text>().text = personName;
            //Button SeeMore = g.transform.Find("MoreBtn").GetComponent<Button>();
            Button BtnItem = g.transform.GetComponent<Button>();
            print("" + BtnItem.name);
            //Personality personality = PersonalitiesList[i];
            MainDataHolder.clickedPersonality = MainDataHolder.clickedMultiplePersonality[i];
            print("personality:::: " + MainDataHolder.clickedPersonality.Name);
            BtnItem.onClick.AddListener(() =>
            {

                print("on click" + MainDataHolder.clickedPersonality.Name);
                OnePersonalityPage.SetActive(true);
                SetSinglePersonality();
            });
        }
        PersonalityItem.SetActive(false);


    }
}
