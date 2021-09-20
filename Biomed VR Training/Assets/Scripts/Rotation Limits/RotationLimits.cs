using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimits : MonoBehaviour
{
    public string axis = "z";
    public float maxValue = 50;
    public float minValue = -50;

    public AudioSource audioToPlayAtLimits;
    Vector3 curRot;
    float tempPos;
    bool waited = true;

    float rotationY;

    void Start()
    {
        // Get initial rotation
        curRot = this.transform.localEulerAngles;
    }

    void LateUpdate()
    {
        float angle = transform.localEulerAngles.y;

        // Set the object rotation
        if (axis == "x")
            transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x, minValue, maxValue), transform.localEulerAngles.y, transform.localEulerAngles.z);
        else if (axis == "y")
        {
            rotationY = Mathf.Clamp(transform.eulerAngles.y, 30F, 180.0F);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotationY, transform.eulerAngles.z);
        }

        print("PPP: " + rotationY);
        //print("ACTUAL: " + transform.localEulerAngles.y);

        if (audioToPlayAtLimits)
        {
            //if (angle >= maxValue || angle <= minValue)
            //{
                //if (!audioToPlayAtLimits.isPlaying && angle != tempPos && waited)
                //{
                    //tempPos = angle;
                    waited = false;
                    //StartCoroutine(Delay());

                    //audioToPlayAtLimits.Play();
                //}
            //}
        }
    }

    public float Clamp0360(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        waited = true;
    }
}

