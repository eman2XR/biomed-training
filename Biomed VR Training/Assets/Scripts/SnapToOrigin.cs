using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToOrigin : MonoBehaviour
{
    Vector3 originalPos;
    Quaternion originalRot;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        originalPos = this.transform.position;
        originalRot = this.transform.rotation;
    }

    public void SnapToOriginalLocation()
    {
        transform.position = originalPos;
        transform.rotation = originalRot;
    }
}
