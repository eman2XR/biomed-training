using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public AudioSource soundFx;

    public Transform instructionsPanel;
    public AudioSource dropObjectClip1;
    public AudioSource dropObjectClip2;
    public AudioSource introVO;

    int droppedCounter;
    bool waited;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(55); //we don't want the VO to get triggered during the intro
        waited = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SnapToOrigin>())
            StartCoroutine(Delay(other.GetComponent<SnapToOrigin>()));

        if (other.transform.parent)
            if (other.transform.parent.GetComponent<SnapToOrigin>())
                StartCoroutine(Delay(other.transform.parent.GetComponent<SnapToOrigin>()));

        if (waited)
        {
            StartCoroutine(WaitForDelay());
            droppedCounter++;
            if (droppedCounter == 1)
                if (!dropObjectClip1.isPlaying)
                {
                    StartCoroutine(ReenableVO());
                    dropObjectClip1.Play();
                }
            if (droppedCounter == 2)
                if (!dropObjectClip2.isPlaying)
                {
                    StartCoroutine(ReenableVO());
                    dropObjectClip2.Play();
                }
        }
    }

    IEnumerator Delay(SnapToOrigin obj)
    {
        yield return new WaitForSeconds(1);
        obj.SnapToOriginalLocation();
        soundFx.Play();
    }

    IEnumerator WaitForDelay()
    {
        waited = false;
        yield return new WaitForSeconds(3);
        waited = true;
    }

    IEnumerator ReenableVO()
    {
        if (introVO) introVO.Pause();
        AudioSource curAudio = null;
        foreach (Transform child in instructionsPanel.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<I_Step>() && child.GetComponent<AudioSource>().isPlaying)
            {
                curAudio = child.GetComponent<AudioSource>();
                curAudio.Pause();
            }
        }
        yield return new WaitForSeconds(4.5f);
        if(curAudio) curAudio.UnPause();
        if (introVO.enabled) introVO.UnPause();
    }
}
