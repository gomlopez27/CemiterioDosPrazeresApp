using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Button StopBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    public Button More30Btn;
    public Button Minus30Btn;
    public AudioSource AudioToControl;
    // Start is called before the first frame update
    void Start()
    {
        AudioControllerButtons(AudioToControl);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AudioControllerButtons(AudioSource audioToControl)
    {

        StopBtn.onClick.AddListener(() =>
        {
            audioToControl.Stop();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        PlayBtn.onClick.AddListener(() =>
        {
            audioToControl.Play();
            PlayBtn.gameObject.SetActive(false);
            PauseBtn.gameObject.SetActive(true);
        });

        PauseBtn.onClick.AddListener(() =>
        {
            audioToControl.Pause();
            PlayBtn.gameObject.SetActive(true);
            PauseBtn.gameObject.SetActive(false);

        });

        More30Btn.onClick.AddListener(() =>
        {

            if (audioToControl.time + 30 > audioToControl.clip.length)
            {
                audioToControl.time = audioToControl.clip.length;

            }
            else
            {
                audioToControl.time = audioToControl.time + 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioToControl.time);

        });

        Minus30Btn.onClick.AddListener(() =>
        {

            if (audioToControl.time - 30 < 0)
            {
                audioToControl.time = 0;

            }
            else
            {
                audioToControl.time = audioToControl.time - 30;

            }
            Debug.Log("audioSourceYesPoem.time " + audioToControl.time);

        });



    }

}