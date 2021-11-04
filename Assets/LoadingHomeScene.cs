using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadingHomeScene : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text text;
    int progress = 0;
   
    public void OnSliderChanged(float value)
    {
        text.text = value.ToString();
    }

    public void UpdateProgress(int value)
    {
        int step = (int)(slider.maxValue / value);
        progress+=step;
        slider.value = progress;
    }

    public void FinalProgress()
    {
        slider.value = slider.maxValue;
    }

    public IEnumerator ShowDownloadProgress(UnityWebRequest www)
    {
        while (!www.isDone)
        {
            slider.value = www.downloadProgress;
            text.text = (string.Format("{0:0%}", www.downloadProgress));
            yield return new WaitForSeconds(.01f);
        }
        slider.value = 0;
    }
}
