using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideAnimation : MonoBehaviour
{
    public GameObject guideAvatar;
    public GameObject audioIntro;
    public Transform[] Positions;
    public float speed;
    public GameObject jazSpecificAnimation;

    //public GameObject audioPoem;
    public GameObject audioNo;
    public GameObject BtnsPanel;
    //public GameObject CesarioPlaceHolder;
    //public Button StopBtn;
    //public Button PlayBtn;
    //public Button PauseBtn;
    //public Button More30Btn;
    //public Button Minus30Btn;

    private Transform NextPos;
    private int NextPosIndex;
    private Animator modelAnimator;
    private bool hasFineshedMoving;
    private bool hasFineshedPlayingIntroAudio;
    private bool isPlayingIntroAudio;
    private bool isPlayingAudio;
    private bool isModelMoving;
    private bool isModelWaving;
    private bool clickedYes;
    private bool clickedNo;
    private bool PanelVisible;
    private int openCount;
    private AudioSource audioSourceIntro;
    //private AudioSource audioSourceYes;
    private AudioSource audioSourceNo;
    private bool PoemPlaying;
    private bool guideIsBack;
    private bool newScript;

    // Start is called before the first frame update
    void Start()
    {
        //startTime = Time.time;
        //openCount = 0;
        NextPos = Positions[0];
        modelAnimator = guideAvatar.GetComponent<Animator>();
        audioSourceIntro = audioIntro.GetComponent<AudioSource>();
        audioSourceNo = audioNo.GetComponent<AudioSource>();

        //audioSourceYes = audioPoem.GetComponent<AudioSource>();
        //AudioControllerButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFineshedMoving)
        {
            MoveGuide();
            modelAnimator.SetBool("isWalking", true);
        }

        if (hasFineshedMoving & !isPlayingIntroAudio && !hasFineshedPlayingIntroAudio)
        {
            modelAnimator.SetBool("isWalking", false);
            StartCoroutine(PlayIntroAudio());
      
        }

        //if(hasFineshedPlayingIntroAudio && !newScript)
        //{
        //    cesarioAnimation.enabled = true;
        //    //this.GetComponent<GuideAnimation>().enabled = false;
        //}

        if (hasFineshedPlayingIntroAudio && !PanelVisible)
        {
            print("entrei");
            BtnsPanel.SetActive(true);
            PanelVisible = true;
        }

        ////if (PoemPlaying && !guideIsBack)
        ////{
        ////    StartCoroutine(ShowGuide());
        ////}


        //if (isModelMoving)
        //{
        //    modelAnimator.SetBool("isWalking", true);
        //}

        //if (!isModelMoving)
        //{
        //    modelAnimator.SetBool("isWalking", false);
        //    modelAnimator.SetBool("isWaving", true);
        //}

        //if (modelAnimator.GetBool("isWaving") && !isPlayingAudio)
        //{
        //    StartCoroutine(playSound());

        //}
    }

    public void MoveGuide()
    {
        if (guideAvatar.transform.position == NextPos.position)
        {
            NextPosIndex++;
            if(NextPosIndex >= Positions.Length)
            {
                NextPosIndex = Positions.Length - 1; //To stop at the last pos
                hasFineshedMoving = true;

            }
            NextPos = Positions[NextPosIndex];
        }
        else
        {
            guideAvatar.transform.position = Vector3.MoveTowards(guideAvatar.transform.position, NextPos.position, speed);
        }
    }

    IEnumerator PlayIntroAudio()
    {
        isPlayingIntroAudio = true;
        audioSourceIntro.Play();
        print("playing intro");
        yield return new WaitForSeconds(audioSourceIntro.clip.length);
        audioSourceIntro.Stop();
        modelAnimator.SetBool("isIdle", true);
        isPlayingIntroAudio = false;
        hasFineshedPlayingIntroAudio = true;
    }


    public void ClickYesPanel()
    {
        //clickedYes = true;
        //CesarioPlaceHolder.SetActive(true);
        ////modelAnimator.SetBool("isIdle", true);
        //guideAvatar.SetActive(false);
        //BtnPoemPanel.SetActive(false);
        //audioSourceYesPoem.Play();
        //PoemPlaying = true;

        BtnsPanel.SetActive(false);
        jazSpecificAnimation.gameObject.SetActive(true);

    }
    public void ClickNoPanel()
    {
        clickedNo = true;
        BtnsPanel.SetActive(false);
        modelAnimator.SetBool("isWalking", false);
        audioSourceNo.Play();

    }

    //IEnumerator ShowGuide()
    //{

    //    yield return new WaitForSeconds(audioSourceYesPoem.clip.length);
    //    guideAvatar.SetActive(true);
    //    guideIsBack = true;
    //}



    //public void AudioControllerButtons()
    //{

    //    StopBtn.onClick.AddListener(() =>
    //    {
    //      audioSourceYesPoem.Stop();
    //      PlayBtn.gameObject.SetActive(true);
    //      PauseBtn.gameObject.SetActive(false);

    //    });

    //    PlayBtn.onClick.AddListener(() =>
    //    {
    //        audioSourceYesPoem.Play();
    //        PlayBtn.gameObject.SetActive(false);
    //        PauseBtn.gameObject.SetActive(true);
    //    });

    //    PauseBtn.onClick.AddListener(() =>
    //    {
    //        audioSourceYesPoem.Pause();
    //        PlayBtn.gameObject.SetActive(true);
    //        PauseBtn.gameObject.SetActive(false);

    //    });

    //    More30Btn.onClick.AddListener(() =>
    //    {

    //        if(audioSourceYesPoem.time + 30 > audioSourceYesPoem.clip.length)
    //        {
    //            audioSourceYesPoem.time = audioSourceYesPoem.clip.length;

    //        }
    //        else{
    //            audioSourceYesPoem.time = audioSourceYesPoem.time + 30;

    //        }
    //        Debug.Log("audioSourceYesPoem.time " + audioSourceYesPoem.time);

    //    });

    //    Minus30Btn.onClick.AddListener(() =>
    //    {

    //        if (audioSourceYesPoem.time - 30 < 0)
    //        {
    //            audioSourceYesPoem.time = 0;

    //        }
    //        else
    //        {
    //            audioSourceYesPoem.time = audioSourceYesPoem.time - 30;

    //        }
    //        Debug.Log("audioSourceYesPoem.time " + audioSourceYesPoem.time);

    //    });




    //}

    public bool isAnimationPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

}
