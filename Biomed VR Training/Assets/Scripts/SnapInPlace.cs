using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnapInPlace : MonoBehaviour
{
    public string objectType;
    public bool snapped;
    public UnityEvent OnSnapped;
    public bool parent;

    OVRGrabbable grabbable;
    Collider grabbableCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (!snapped)
        {
            if (other.GetComponent<OVRGrabbable>() || other.transform.parent.GetComponent<OVRGrabbable>()) 
            {
                if (other.GetComponent<OVRGrabbable>())
                {
                    grabbable = other.GetComponent<OVRGrabbable>();
                    grabbableCollider = other;
                }
                else if (other.transform.parent.GetComponent<OVRGrabbable>())
                {
                    grabbable = other.transform.parent.GetComponent<OVRGrabbable>();
                    grabbableCollider = other;
                }

                if (grabbable.objectType == objectType && !grabbable.Snapped)
                {
                    snapped = true;
                    grabbable.Snapped = true;
                    this.GetComponent<Collider>().enabled = false;
                    other.enabled = false;

                    //force release the object
                    if (grabbable.grabbingHand)
                        grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);

                    //disable collider and physics
                    grabbable.isGrabbable = false;
                    grabbable.grabPoints[0].enabled = false;
                    grabbable.GetComponent<Rigidbody>().isKinematic = true;
                    grabbable.GetComponent<Outline>().enabled = false;
                    grabbableCollider.enabled = false;
                    //grabbable.isTouchable = false;
                    //grabbable.isGrabbable = false;

                    //snap to position
                    grabbable.transform.position = this.transform.position;
                    grabbable.transform.rotation = this.transform.rotation;

                    //audio
                    this.GetComponent<AudioSource>().Play();

                    if (parent)
                        grabbable.transform.parent = this.transform.parent;

                    StartCoroutine(DisableObject());

                    OnSnapped.Invoke();
                }
            }
        }
    }

    IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(0.1f);
        if (this.GetComponent<Renderer>())
            this.GetComponent<Renderer>().enabled = false;
        if (transform.childCount > 0)
            if (this.transform.GetChild(0).GetComponent<Renderer>())
                this.transform.GetChild(0).GetComponent<Renderer>().enabled = false;

        //this.gameObject.SetActive(false);
    }
}
