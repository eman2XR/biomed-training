using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKnob : MonoBehaviour
{
    public float movedAngles;
    public float movingSpeed = 2;
    public float duration = 1;
    bool isBusy;

    //triggered by the PivotPoint
    public void Grabbed()
    {
        if (!isBusy)
            StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        isBusy = true;
        float startRotation = transform.localEulerAngles.y;
        float endRotation = startRotation - 90f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, yRotation, transform.localEulerAngles.z);
            yield return null;
        }
        isBusy = false;
    }

    
}
