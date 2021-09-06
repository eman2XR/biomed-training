using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    AudioSource audioS;
    public AudioClip onHighlightSound;

    void Awake()
    {
        audioS = this.GetComponent<AudioSource>();
        audioS.volume = 0.35f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioS.clip = onHighlightSound;
        audioS.Play();
    }
    public void OnSelect(BaseEventData eventData)
    {
        //do your stuff when selected
    }
}
