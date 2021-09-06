using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimits : MonoBehaviour
{
     public float maxY = 50;
     public float minY = -50;

    public AudioSource audioToPlayAtLimits;
    Vector3 curRot;
    float tempPos;
    bool waited = true;

    void Start()
    {
        // Get initial rotation
        curRot = this.transform.localRotation.eulerAngles;
    }

    void LateUpdate()
    {
        float angle = transform.localEulerAngles.z;

        //calculate negative euler
        if (angle > 180) angle = angle - 360;
        //angle = (angle > 180) ? angle - 360 : angle;

        //restrict rotation 
        curRot.z = Mathf.Clamp(angle, minY, maxY);

        // Set the object rotation
        this.transform.localRotation = Quaternion.Euler(curRot);

        if (audioToPlayAtLimits)
        {
            if (angle >= maxY || angle <= minY)
            {
                if (!audioToPlayAtLimits.isPlaying && angle != tempPos && waited)
                {
                    tempPos = angle;
                    waited = false;
                    //StartCoroutine(Delay());

                    audioToPlayAtLimits.Play();
                }
            }
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        waited = true;
    }
}

