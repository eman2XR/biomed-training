﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Screw : MonoBehaviour
{
    public bool isEngaged;
    public float movingSpeed = 1;
    public float range = 0.02f;
    Collider collider;
    AudioSource audio;
    public UnityEvent onDown;
    public UnityEvent onUp;
    public UnityEvent onTouch;
    bool hasBeenUsed;

    public bool useZAxis;
    public bool useMinusZAxis;

    private void Start()
    {
        collider = this.GetComponent<Collider>();
        audio = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "phillipsScrewdriver")
        {
            if (!hasBeenUsed)
            {
                onTouch.Invoke();
                StartCoroutine(EngageScrew());
                audio.Play();
                collider.enabled = false;
                hasBeenUsed = true;
            }
        }
    }

    IEnumerator EngageScrew()
    {
        if (isEngaged)
        {
            float elapsedTime = 0;
            float duration = 2f;
            while (elapsedTime < duration)
            {
                this.transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, 180), Time.deltaTime * movingSpeed);
                this.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, -range, 0), Time.deltaTime * movingSpeed);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            isEngaged = false;
            yield return new WaitForSeconds(0.5f);
            onDown.Invoke();
            collider.enabled = true;
            collider.isTrigger = false;
        }
        else
        {
            float elapsedTime = 0;
            float duration = 2f;
            while (elapsedTime < duration)
            {
                if (useZAxis)
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                    transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, 0, range), Time.deltaTime * movingSpeed);
                }
                else if (useMinusZAxis)
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                    transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, 0, range), Time.deltaTime * movingSpeed);
                }
                else 
                {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                    transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, range, 0), Time.deltaTime * movingSpeed);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            isEngaged = true;
            yield return new WaitForSeconds(0.5f);
            onUp.Invoke();
            collider.enabled = true;
            collider.isTrigger = false;
        }
    }
}
