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
    public bool useMinusZAxis;
    public bool useMinusYAxis;

    public int curRotations;

    GameObject screwdriverPivot;

    public float screwValue = 0f;
    float lastValue;
    Vector3 initLocPos;
    bool isTurning;

    private void Start()
    {
        collider = this.GetComponent<Collider>();
        audio = this.GetComponent<AudioSource>();
        screwdriverPivot = transform.GetChild(0).gameObject;
        initLocPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "phillipsScrewdriver")
            if (!hasBeenUsed)
                onTouch.Invoke();
    }

    private void Update()
    {
        if (isTurning && audio.isPlaying)
            audio.Stop();
    }
    
    public void ScrewdriverTurned(float value)
    {
        if (lastValue != value && value < 0)
        {
            isTurning = true;
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 0, -value*10), Time.deltaTime * 2);
            if (useZAxis)
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, 0, (value / 750)), Time.deltaTime * movingSpeed);
            else if (useMinusZAxis)
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, 0, (value / 750)), Time.deltaTime * movingSpeed);
            else if (useMinusYAxis)
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0, (value / 750), 0), Time.deltaTime * movingSpeed);
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, (value / 750), 0), Time.deltaTime * movingSpeed);

            lastValue = value;
            if (!audio.isPlaying)
                audio.Play();
        }
        else
        {
            isTurning = false;
        }

        if (useZAxis || useMinusZAxis)
        {
            if (Mathf.Abs(transform.localPosition.z - initLocPos.z) > range)
                if (!hasBeenUsed)
                    FullyUnscrewed();
        }
        else
        {
            if (Mathf.Abs(transform.localPosition.y - initLocPos.y) > range)
                if (!hasBeenUsed)
                    FullyUnscrewed();
        }
       
    }

    void FullyUnscrewed()
    {
        //StartCoroutine(EngageScrew());
        collider.enabled = false;
        onUp.Invoke();
        collider.enabled = true;
        collider.isTrigger = false;
        screwdriverPivot.GetComponent<PivotPoint>().Detach();
        screwdriverPivot.SetActive(false);
        hasBeenUsed = true;
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

        // (curRotations >= 3)
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
