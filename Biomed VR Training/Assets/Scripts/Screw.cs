using System.Collections;
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
    public bool useMinusYAxis;

    public int curRotations;

    GameObject screwdriverPivot;

    private void Start()
    {
        collider = this.GetComponent<Collider>();
        audio = this.GetComponent<AudioSource>();
        screwdriverPivot = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "phillipsScrewdriver")
            if (!hasBeenUsed)
                onTouch.Invoke();
    }

    public void ScrewdriverTurned()
    {
        if (!hasBeenUsed)
        {
            StartCoroutine(EngageScrew());
            audio.Play();
            collider.enabled = false;
        }
    }

    IEnumerator EngageScrew()
    {
        float elapsedTime = 0;
        float duration = 1.5f;
        //curRotations++;
        
        while (elapsedTime < duration)
        {
            if (useZAxis)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, 0, range), Time.deltaTime * movingSpeed);
            }
            else if (useMinusYAxis)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, range, 0), Time.deltaTime * movingSpeed);
            }
            else 
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -180), Time.deltaTime * 2);
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, range, 0), Time.deltaTime * movingSpeed);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audio.Stop();
        screwdriverPivot.GetComponent<PivotPoint>().isTurning = false;

        //if (curRotations >= 3)
        //{
        isEngaged = true;
        //yield return new WaitForSeconds(0.25f);
        onUp.Invoke();
        collider.enabled = true;
        collider.isTrigger = false;
        screwdriverPivot.GetComponent<PivotPoint>().Detach();
        screwdriverPivot.SetActive(false);
        hasBeenUsed = true;
        //}
    }

    public void ActivateScrew()
    {
        collider.enabled = true;
        screwdriverPivot.SetActive(true);
    }

    public void DeactivateScrew()
    {
        collider.enabled = false;
        screwdriverPivot.SetActive(false);
    }
}
