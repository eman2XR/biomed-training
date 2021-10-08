using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensCap : MonoBehaviour
{
    public bool firstCapOpen;
    public bool secondCapOpen;

    public Transform newGasket;
    public Transform oldGasket;

    public bool newGasketIn;

    public void CapOpened(int capNumber)
    {
        if (capNumber == 1)
            firstCapOpen = true;
        else if (capNumber == 2)
            secondCapOpen = true;
    }

    //triggered by the lens collider
    public void PlaceNewGasket()
    {
        //force release the object
        OVRGrabbable grabbable = newGasket.GetComponent<OVRGrabbable>();
        if(grabbable.grabbingHand)
            grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);

        //disable collider and physics
        newGasket.GetComponent<Collider>().enabled = false;
        grabbable.GetComponent<Rigidbody>().isKinematic = true;
        grabbable.GetComponent<Outline>().enabled = false;

        //snap to position
        grabbable.transform.position = oldGasket.transform.position;
        grabbable.transform.rotation = oldGasket.transform.rotation;

        //audio
        this.GetComponent<AudioSource>().Play();

        newGasketIn = true;
    }
}
