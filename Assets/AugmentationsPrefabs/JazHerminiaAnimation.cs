using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazHerminiaAnimation : MonoBehaviour
{
    public GameObject mainAudio;
    public GameObject HerminiaPlaceHolder;
    public GameObject guideAvatar;
    public Button StopBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    public Button More30Btn;
    public Button Minus30Btn;


    private AudioSource audioSourceMain;

    private bool AudioPlaying;
    private bool guideIsBack;

    void Start()
    {
        audioSourceMain = mainAudio.GetComponent<AudioSource>();
        PlayMainAudio();
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

        More30Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time + 30 > audioSourceMain.clip.length)
            {
                audioSourceMain.time = audioSourceMain.clip.length;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time + 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

        });

        Minus30Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time - 30 < 0)
            {
                audioSourceMain.time = 0;

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time - 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioSourceMain.time);

        });

    }
}
