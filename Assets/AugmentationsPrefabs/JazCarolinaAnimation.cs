using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JazCarolinaAnimation : MonoBehaviour
{
    public GameObject mainAudio;
    public GameObject Carolina3DModel;
    public GameObject guideAvatar;
    public Button StopBtn;
    public Button PlayBtn;
    public Button PauseBtn;
    public Button More30Btn;
    public Button Minus30Btn;
    //public GameObject VotingTable;
    //public GameObject VotePaper;
    //public GameObject VotePaperPos;
  
    private AudioSource audioSourceMain;

    private Animator anim;

    private bool PoemPlaying;
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
        if (PoemPlaying && audioSourceMain.time == audioSourceMain.clip.length)
        {
            StopAugmentation();
        }
        {

        }

    }

    public void PlayMainAudio()
    {
        Carolina3DModel.SetActive(true);
        guideAvatar.SetActive(false);
        audioSourceMain.Play();
        PoemPlaying = true;
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
        PoemPlaying = false;
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

        More30Btn.onClick.AddListener(() =>
        {

            if (audioSourceMain.time + 30 > audioSourceMain.clip.length)
            {
                // audioSourceMain.time = audioSourceMain.clip.length;
                audioSourceMain.Stop();
                anim.enabled = false;

                Debug.Log("audioSourceYesPoem.time + 30 " + (audioSourceMain.time + 30f));
                Debug.Log("audioSourceYesPoem.length " + audioSourceMain.clip.length);

            }
            else
            {
                audioSourceMain.time = audioSourceMain.time + 30;

            }

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
