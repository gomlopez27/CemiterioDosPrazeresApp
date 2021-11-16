using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazHerminiaAnimation : MonoBehaviour
{
    public const int CHANGE_TIME_VALUE = 10;

    [SerializeField] GameObject mainAudio;
    [SerializeField] GameObject HerminiaPlaceHolder;
    [SerializeField] GameObject guideAvatar;
    [SerializeField] Button StopBtn;
    [SerializeField] Button PlayBtn;
    [SerializeField] Button PauseBtn;
    [SerializeField] Button More10Btn;
    [SerializeField] Button Minus10Btn;
    [SerializeField] Slider slider;
    [SerializeField] Text AudioTotalTime;
    [SerializeField] Text AudioCurrentTime;

    private AudioSource audioSourceMain;
    //private Animator anim;
    private bool AudioPlaying;
    private bool guideIsBack;

    void Start()
    {
        //anim = HerminiaPlaceHolder.GetComponent<Animator>();
        audioSourceMain = mainAudio.GetComponent<AudioSource>();
        PlayMainAudio();
        AudioControllerButtons();
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
        if (AudioPlaying)
        {
            //StartCoroutine(ShowGuide());
            //print("current time: " + audioSourceMain.time);
            float currTime = audioSourceMain.time;
            TimeSpan t = TimeSpan.FromSeconds(currTime);
            string clipLenghtFormated = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            AudioCurrentTime.text = clipLenghtFormated;
            slider.value = currTime;


        }
        if (AudioPlaying && audioSourceMain.time == audioSourceMain.clip.length)
        {
            StopAugmentation();
        }
    }

    public void PlayMainAudio()
    {
        HerminiaPlaceHolder.SetActive(true);
        guideAvatar.SetActive(false);
        audioSourceMain.Play();
        AudioPlaying = true;

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
        AudioPlaying = false;
        //anim.enabled = false;
        audioSourceMain.Stop();
        PlayBtn.gameObject.SetActive(true);
        PauseBtn.gameObject.SetActive(false);
    }

    public void AudioControllerButtons()
    {

        StopBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Stop();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        PlayBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Play();
            PlayBtn.gameObject.SetActive(false);
            PauseBtn.gameObject.SetActive(true);
        });

        PauseBtn.onClick.AddListener(() =>
        {
            audioSourceMain.Pause();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        More10Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time + CHANGE_TIME_VALUE > audioSourceMain.clip.length)
            {
                audioSourceMain.time = audioSourceMain.clip.length;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time + CHANGE_TIME_VALUE;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

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
