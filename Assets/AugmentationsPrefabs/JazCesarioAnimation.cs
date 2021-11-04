using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazCesarioAnimation : MonoBehaviour
{
    public GameObject audioPoem;
    public GameObject CesarioPlaceHolder;
    public GameObject guideAvatar;
    public Button StopBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    public Button More30Btn;
    public Button Minus30Btn;

    private bool isPlayingAudio;
    private bool isModelMoving;
    private bool isModelWaving;
    private bool clickedYes;
    private bool clickedNo;
    private int openCount;
    private AudioSource audioSourcePoem;

    private bool PoemPlaying;
    private bool guideIsBack;

    void Start()
    {
        audioSourcePoem = audioPoem.GetComponent<AudioSource>();
        PlayPoem();
        AudioControllerButtons();

    }

    // Update is called once per frame
    void Update()
    {

        //if (PoemPlaying && !guideIsBack)
        //{
        //    StartCoroutine(ShowGuide());
        //}
    }

    public void PlayPoem()
    {
        CesarioPlaceHolder.SetActive(true);
        guideAvatar.SetActive(false);
        audioSourcePoem.Play();
        PoemPlaying = true;

    }

    IEnumerator ShowGuide()
    {

        yield return new WaitForSeconds(audioSourcePoem.clip.length);
        guideAvatar.SetActive(true);
        guideIsBack = true;
    }

    public void AudioControllerButtons()
    {

        StopBtn.onClick.AddListener(() =>
        {
            audioSourcePoem.Stop();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        PlayBtn.onClick.AddListener(() =>
        {
            audioSourcePoem.Play();
            PlayBtn.gameObject.SetActive(false);
            PauseBtn.gameObject.SetActive(true);
        });

        PauseBtn.onClick.AddListener(() =>
        {
            audioSourcePoem.Pause();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        More30Btn.onClick.AddListener(() =>
        {

            if (audioSourcePoem.time + 30 > audioSourcePoem.clip.length)
            {
                audioSourcePoem.time = audioSourcePoem.clip.length;

            }
            else
            {
                audioSourcePoem.time = audioSourcePoem.time + 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourcePoem.time);

        });

        Minus30Btn.onClick.AddListener(() =>
        {

            if (audioSourcePoem.time - 30 < 0)
            {
                audioSourcePoem.time = 0;

            }
            else
            {
                audioSourcePoem.time = audioSourcePoem.time - 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourcePoem.time);

        });

    }


}
