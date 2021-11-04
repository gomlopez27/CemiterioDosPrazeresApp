using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject model;
    AudioSource audioCarolina;
    bool hasStoped;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = model.GetComponent<Animator>();
        audioCarolina = this.GetComponent<AudioSource>();
        print(audioCarolina.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStoped)
        {
            StartCoroutine(AudioPlaying());

           
        }
    }

    IEnumerator AudioPlaying()
    {
        yield return new WaitForSeconds(audioCarolina.clip.length);
        hasStoped = true;
        anim.enabled = false;
        //anim.SetBool("audioPlaying", false);

    }
}
