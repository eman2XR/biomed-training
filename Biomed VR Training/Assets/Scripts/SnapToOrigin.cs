using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToOrigin : MonoBehaviour
{
    public Transform snapPoint; //optional transform to snap to differnt location
    Vector3 originalPos;
    Quaternion originalRot;

    Collider collider;

    IEnumerator Start()
    {
        if (this.GetComponent<OVRGrabbable>())
            collider = this.GetComponent<OVRGrabbable>().grabPoints[0];

        yield return new WaitForSeconds(1);
        originalPos = this.transform.position;
        originalRot = this.transform.rotation;
    }

    public void SnapToOriginalLocation()
    {
        if (snapPoint == null)
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
        }
        else
        {
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
        }

        collider.enabled = true;
    }
}
