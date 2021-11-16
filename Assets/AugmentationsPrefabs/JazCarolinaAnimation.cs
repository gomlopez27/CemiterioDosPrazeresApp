using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazCarolinaAnimation : MonoBehaviour
{
    public const int CHANGE_TIME_VALUE = 10;
    [SerializeField] GameObject mainAudio;
    [SerializeField] GameObject Carolina3DModel;
    [SerializeField] GameObject guideAvatar;
    [SerializeField] Button StopBtn;
    [SerializeField] Button PlayBtn;
    [SerializeField] Button PauseBtn;
    [SerializeField] Button More10Btn;
    [SerializeField] Button Minus10Btn;
    [SerializeField] Slider slider;
    [SerializeField] Text AudioTotalTime;
    [SerializeField] Text AudioCurrentTime;
    //public GameObject VotingTable;
    //public GameObject VotePaper;
    //public GameObject VotePaperPos;

    private AudioSource audioSourceMain;

    private Animator anim;

    private bool MainAudioPlaying;
    private bool guideIsBack;
    private bool VotingTableVisible;
    private bool VotingTableInvisible;

    void Start()
    {
        anim = Carolina3DModel.GetComponent<Animator>();
        audioSourceMain = mainAudio.GetComponent<AudioSource>();
        PlayMainAudio();
        AudioControllerButtons();
        print(audioSourceMain.clip.name);

        float clipLenght = audioSourceMain.clip.length;
        slider.maxValue = clipLenght;
        TimeSpan t = TimeSpan.FromSeconds(clipLenght);
        string clipLenghtFormated = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        AudioTotalTime.text = clipLenghtFormated;

    }

    // Update is called once per frame
    void Update()
    {

        //if (PoemPlaying && !guideIsBack)
        //{
        //    StartCoroutine(ShowGuide());
        //}

        //if (!VotingTableVisible)
        //{
        //    ShowVotingTable();
        //}

        //if (VotingTableVisible && !VotingTableInvisible)
        //{
        //    NotShowVotingTable();
        //}

        if (MainAudioPlaying)
        {
            //StartCoroutine(ShowGuide());
            //print("current time: " + audioSourceMain.time);
            float currTime = audioSourceMain.time;
            TimeSpan t = TimeSpan.FromSeconds(currTime);
            string clipLenghtFormated = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            AudioCurrentTime.text = clipLenghtFormated;
            slider.value = currTime;

           
        }
        if (MainAudioPlaying && audioSourceMain.time == audioSourceMain.clip.length)
        {
            StopAugmentation();
        }
        

    }

    public void PlayMainAudio()
    {
        Carolina3DModel.SetActive(true);
        guideAvatar.SetActive(false);
        audioSourceMain.Play();
        MainAudioPlaying = true;
        //yield return new WaitForSeconds(audioSourceMain.clip.length);
        //PoemPlaying = false;

        //StartCoroutine(ShowVotingTable());
        //StartCoroutine(NotShowVotingTable());

    }

    IEnumerator ShowGuide()
    {

        yield return new WaitForSeconds(audioSourceMain.clip.length);

        guideAvatar.SetActive(true);
        guideIsBack = true;
    }

    void StopAugmentation()
    {
        //Carolina3DModel.SetActive(false);
        MainAudioPlaying = false;
        anim.enabled = false;
        audioSourceMain.Stop();
        PlayBtn.gameObject.SetActive(true);
        PauseBtn.gameObject.SetActive(false);
    }


    //IEnumerator ShowVotingTable()
    //{
    //yield return new WaitForSeconds(5);
    //VotingTable.SetActive(true);
    //yield return new WaitForSeconds(1);
    //Vector3 newPos = new Vector3(VotePaper.transform.position.x, VotePaper.transform.position.y - 100, VotePaper.transform.position.z);
    //VotePaper.transform.position = Vector3.MoveTowards(VotePaper.transform.position, newPos, (float)0.005);
    //VotingTableVisible = true;
    //}

    //IEnumerator NotShowVotingTable()
    //{
    //    yield return new WaitForSeconds(20);
    //    VotingTable.SetActive(false);
    //    VotingTableInvisible = true;

    //}


    public void AudioControllerButtons()
    {

        StopBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Stop();
            anim.enabled = false;
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        PlayBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Play();
            anim.enabled = true;
            PlayBtn.gameObject.SetActive(false);
            PauseBtn.gameObject.SetActive(true);
        });

        PauseBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Pause();
            anim.enabled = false;
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        More10Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time + CHANGE_TIME_VALUE > audioSourceMain.clip.length)
            {
                // audioSourceMain.time = audioSourceMain.clip.length;
                audioSourceMain.Stop();
                anim.enabled = false;

                Debug.Log("audioSourceYesPoem.time + 10 " + (audioSourceMain.time + 10f));
                Debug.Log("audioSourceYesPoem.length " + audioSourceMain.clip.length);

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time + CHANGE_TIME_VALUE;

            }

        });

        Minus10Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time - CHANGE_TIME_VALUE < 0)
            {
                audioSourceMain.time = 0;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time - CHANGE_TIME_VALUE;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

        });

    }
}
