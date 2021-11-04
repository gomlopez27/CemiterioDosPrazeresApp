using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeGuideAvatar : MonoBehaviour
{
    public GameObject model;
    public GameObject audioPoem;
    public GameObject audioIntro;
    public GameObject audioNoPoem;
    public Transform[] Positions;
    public float speed;
    public GameObject BtnPoemPanel;
    public GameObject CesarioPlaceHolder;
    public Button StopBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    public Button More30Btn;
    public Button Minus30Btn;

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
    private int openCount;
    private AudioSource audioSourceIntro;
    private AudioSource audioSourceYesPoem;
    private AudioSource audioSourceNoPoem;

    // Start is called before the first frame update
    void Start()
    {
        //startTime = Time.time;
        //openCount = 0;
        NextPos = Positions[0];
        modelAnimator = model.GetComponent<Animator>();
        audioSourceIntro = audioIntro.GetComponent<AudioSource>();
        audioSourceYesPoem = audioPoem.GetComponent<AudioSource>();
        audioSourceNoPoem = audioNoPoem.GetComponent<AudioSource>();

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
        if (model.transform.position == NextPos.position)
        {
            NextPosIndex++;
            if (NextPosIndex >= Positions.Length)
            {
                NextPosIndex = Positions.Length - 1; //To stop at the last pos
                hasFineshedMoving = true;

            }
            NextPos = Positions[NextPosIndex];
        }
        else
        {
            model.transform.position = Vector3.MoveTowards(model.transform.position, NextPos.position, speed);
        }
    }

    IEnumerator PlayIntroAudio()
    {
        isPlayingIntroAudio = true;
        audioSourceIntro.Play();
        yield return new WaitForSeconds(audioSourceIntro.clip.length);
        audioSourceIntro.Stop();
        isPlayingIntroAudio = false;
        hasFineshedPlayingIntroAudio = true;


    }


    public void PlayPoem()
    {
        clickedYes = true;
        CesarioPlaceHolder.SetActive(true);
        modelAnimator.SetBool("isIdle", true);
        BtnPoemPanel.SetActive(false);
        audioSourceYesPoem.Play();

    }

    public void DontPlayPoem()
    {
        clickedNo = true;
        BtnPoemPanel.SetActive(false);
        audioSourceNoPoem.Play();
    }


    public bool isAnimationPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

}
