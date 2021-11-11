using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazCesarioAnimation : MonoBehaviour
{
    public const int CHANGE_TIME = 10;
    [SerializeField] GameObject audioPoem;
    [SerializeField] GameObject CesarioModel;
    [SerializeField] GameObject guideAvatar;
    [SerializeField] Button StopBtn;
    [SerializeField] Button PlayBtn;
    [SerializeField] Button PauseBtn;
    [SerializeField] Button More10Btn;
    [SerializeField] Button Minus10Btn;
    [SerializeField] Slider slider;
    [SerializeField] Text PoemTime;
    [SerializeField] Text PoemCurrentTime;

    private bool isPlayingAudio;
    private bool isModelMoving;
    private bool isModelWaving;
    private bool clickedYes;
    private bool clickedNo;
    private int openCount;
    private AudioSource audioSourceMain;
    private Animator anim;
    private float sliderValue;
    private bool sliderValueChanged;
    private bool PoemPlaying;
    private bool guideIsBack;

    void Start()
    {
        audioSourceMain = audioPoem.GetComponent<AudioSource>();
        anim = CesarioModel.transform.GetChild(1).GetComponent<Animator>();

        PlayPoem();
        AudioControllerButtons();
        float clipLenght = audioSourceMain.clip.length;
        slider.maxValue = clipLenght;
       // print("total time: " + audioSourceMain.clip.length);
        TimeSpan t = TimeSpan.FromSeconds(clipLenght);
        string clipLenghtFormated = string.Format("{0:D2}:{1:D2}", t.Minutes,t.Seconds);
        PoemTime.text = clipLenghtFormated;

        //slider.onValueChanged.AddListener((v) => {
        //    sliderValue = v;
        //    sliderValueChanged = true;
        //});

    }

    // Update is called once per frame
    void Update()
    {

        if (PoemPlaying /*&& !guideIsBack*/)
        {
            //StartCoroutine(ShowGuide());
            //print("current time: " + audioSourceMain.time);
            float currTime= audioSourceMain.time;
            TimeSpan t = TimeSpan.FromSeconds(currTime);
            string clipLenghtFormated = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            PoemCurrentTime.text = clipLenghtFormated;
            slider.value = currTime;

            //if(sliderValueChanged)
            //{
            //    slider.value = sliderValue;
            //    audioSourceMain.time = sliderValue;
            //    sliderValueChanged = false;
            //}
        }

        if (PoemPlaying && audioSourceMain.time == audioSourceMain.clip.length)
        {
            StopAugmentation();
        }
    }

    public void PlayPoem()
    {
        CesarioModel.SetActive(true);
        guideAvatar.SetActive(false);
        audioSourceMain.Play();
        PoemPlaying = true;

    }

    IEnumerator ShowGuide()
    {

        yield return new WaitForSeconds(audioSourceMain.clip.length);
        guideAvatar.SetActive(true);
        guideIsBack = true;
    }

   

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

            if (audioSourceMain.time + CHANGE_TIME > audioSourceMain.clip.length)
            {
                audioSourceMain.time = audioSourceMain.clip.length;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time + CHANGE_TIME;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

        });

        Minus10Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time - CHANGE_TIME < 0)
            {
                audioSourceMain.time = 0;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time - CHANGE_TIME;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

        });

    }

    void StopAugmentation()
    {
        //Carolina3DModel.SetActive(false);
        PoemPlaying = false;
        anim.enabled = false;
        audioSourceMain.Stop();
        PlayBtn.gameObject.SetActive(true);
        PauseBtn.gameObject.SetActive(false);
    }

}
