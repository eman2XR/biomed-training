using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Radio : MonoBehaviour
{
    public List<AudioClip> station1Clips = new List<AudioClip>();
    public List<AudioClip> station2Clips = new List<AudioClip>();
    public List<AudioClip> station3Clips = new List<AudioClip>();

    public int currentStation = 1;
    public int stationCurrentClip = 0;

    public GameObject popuop;

    AudioSource audioSource;

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

        StopAllCoroutines();
        StartCoroutine(HideWithDelay());
    }

    private void Update()
    {
        if (Input.GetKeyDown("["))
            ChangeStation();
    }

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
        yield return new WaitForSeconds(1);
        if (!audioSource.isPlaying && audioSource.enabled)
        {
            if (currentStation == 1)
                audioSource.clip = station1Clips[stationCurrentClip + 1];
            
            if (currentStation == 2)
                audioSource.clip = station2Clips[stationCurrentClip + 1];

            if (currentStation == 3)
                audioSource.clip = station3Clips[stationCurrentClip + 1];

            stationCurrentClip++;
            if (stationCurrentClip == 3) stationCurrentClip = 0;
            audioSource.Play();
            print("change clip");
        }
        StartCoroutine(CheckAudio());
    }
    
    IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(2);
        popuop.SetActive(false);
    }


}
