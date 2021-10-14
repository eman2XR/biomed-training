using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : MonoBehaviour
{
    public List<AudioClip> station1Clips = new List<AudioClip>();
    public List<AudioClip> station2Clips = new List<AudioClip>();
    public List<AudioClip> station3Clips = new List<AudioClip>();

    public int currentStation = 0;
    public int stationCurrentClip = 0;

    public GameObject popuop;

    AudioSource audioSource;
    bool muted;
    Coroutine co;
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(CheckAudio());
    }

    public void ChangeStation()
    {
        popuop.SetActive(true);
        if (currentStation == 0)
            popuop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Bossa Nova Radio";
        if (currentStation == 1)
            popuop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Reggae Radio";
        if (currentStation == 2)
            popuop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Lo-Fi Radio";
        if (currentStation == 3)
        {
            popuop.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Radio OFF";
            audioSource.clip = null;
        }

        if (currentStation == 3)
            currentStation = 0;
        else
            currentStation = currentStation + 1;

        PlayStation(currentStation);

        if(co != null) StopCoroutine(co);
        co = StartCoroutine(HideWithDelay());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown("["))
    //        ChangeStation();
    //}

    void PlayStation(int station)
    {
        audioSource.Stop();
        stationCurrentClip = 0;

        if (station == 1)
            audioSource.clip = station1Clips[0];
        if (station == 2)
            audioSource.clip = station2Clips[0];
        if (station == 3)
            audioSource.clip = station3Clips[0];

        audioSource.Play();
    }

    IEnumerator CheckAudio()
    {
        //print("checking audio");

        yield return new WaitForSeconds(1);
        if (audioSource.enabled && !audioSource.isPlaying)
        {
            stationCurrentClip++;
            if (stationCurrentClip == 3) stationCurrentClip = 0;

            if (currentStation == 1)
                audioSource.clip = station1Clips[stationCurrentClip];
            
            if (currentStation == 2)
                audioSource.clip = station2Clips[stationCurrentClip];

            if (currentStation == 3)
                audioSource.clip = station3Clips[stationCurrentClip];

            audioSource.Play();
            //print("change clip");
        }
        StartCoroutine(CheckAudio());
    }
    
    IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(2);
        popuop.SetActive(false);
    }


}
