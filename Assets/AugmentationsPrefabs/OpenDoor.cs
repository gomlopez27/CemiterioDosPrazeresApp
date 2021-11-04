using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    public GameObject model;
    public GameObject audioS;
    public Transform target;
    public float speed;
    private Animator modelAnimator;
    private float startTime;
    private float openedSpaceTime;
    private bool hasDoorOpened;
    private bool hasDoorClosed;
    private bool isPlayingAudio;
    private bool isModelMoving;
    private bool isModelWaving;
    private int openCount;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        openCount = 0;
        modelAnimator = model.GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDoorOpened && Time.time - startTime > 2f && openCount < 1)
        {
            open();
            openCount = 1;
        }

        if (hasDoorOpened && Time.time - openedSpaceTime > 5f)
        {
            close();
        }

        if (isModelMoving)
        {
            modelAnimator.SetBool("isWalking", true);
        }

        if (!isModelMoving)
        {
            modelAnimator.SetBool("isWalking", false);
            modelAnimator.SetBool("isWaving", true);

  
        }


        if (hasDoorClosed && !isPlayingAudio)
        {
            StartCoroutine(playSound());

        }
    }

    void FixedUpdate()
    {

        //Move towards

        if (hasDoorOpened)
        {
            Vector3 a = model.transform.position;
            Vector3 b = target.position;
            isModelMoving = true;
            model.transform.position = Vector3.MoveTowards(a, b, speed);
            if (model.transform.position.Equals(b))
            {
                isModelMoving = false;
            }
        }
    }

    public void open()
    {
        door.transform.Rotate(Vector3.up, -90);
        openedSpaceTime = Time.time;
        hasDoorOpened = true;
        hasDoorClosed = false;
    }

    public void close()
    {
        door.transform.Rotate(Vector3.up, 90);
        hasDoorOpened = false;
        hasDoorClosed = true;

    }

    IEnumerator playSound()
    {
        AudioSource audioComp = audioS.GetComponent<AudioSource>();
        isPlayingAudio = true;
        audioComp.Play();
        yield return new WaitForSeconds(audioComp.clip.length);
        isPlayingAudio = false;
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
