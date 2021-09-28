using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    OVRGrabbable grabbable;
    Vector3 initialPos;
    float range = 0.05f;

    private void OnEnable()
    {
        grabbable = this.GetComponent<OVRGrabbable>();
        initialPos = this.transform.position;
    }

    private void Update()
    {
        this.transform.position = new Vector3(Mathf.Clamp(transform.position.x, initialPos.x - range, initialPos.x + range),
                                                Mathf.Clamp(transform.position.y, initialPos.y - range, initialPos.y + range),
                                                Mathf.Clamp(transform.position.z, initialPos.z - range, initialPos.z + range));

        if (Vector3.Distance(transform.position, initialPos) > 0.35f)
        {
            //this.transform.position = 
            //if (grabbable.grabbingHand)
            //{
            //    grabbable.GetComponent<Outline>().enabled = false;
            //    grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
            //}
        }
    }
}
