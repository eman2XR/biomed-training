using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRGrabExtended : MonoBehaviour
{
    void OnTriggerEnter(Collider otherCollider)
    {
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        if (grabbable.isTouchable)
        {
            //highlight
            if (otherCollider.gameObject.GetComponent<Outline>())
                otherCollider.gameObject.GetComponent<Outline>().enabled = true;

            if (otherCollider.transform.parent)
                if (otherCollider.transform.parent.gameObject.GetComponent<Outline>())
                    otherCollider.transform.parent.gameObject.GetComponent<Outline>().enabled = true;
        }
    }

    void OnTriggerExit(Collider otherCollider)
    {
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        if (grabbable.isTouchable)
        {
            //unhighlight
            if (otherCollider.gameObject.GetComponent<Outline>())
                otherCollider.gameObject.GetComponent<Outline>().enabled = false;

            if (otherCollider.transform.parent)
                if (otherCollider.transform.parent.gameObject.GetComponent<Outline>())
                    otherCollider.transform.parent.gameObject.GetComponent<Outline>().enabled = false;
        }
    }
}
