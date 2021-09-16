using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontPanel : MonoBehaviour
{
    Transform newGasket;
    Transform oldGasket;

    public Transform newGasket1;
    public Transform oldGasket1;

    [Space(10)]
    public Transform newGasket2;
    public Transform oldGasket2;
   
    [Space(10)]
    public Transform newGasket3;
    public Transform oldGasket3;

    [Space(10)]
    public bool newGasketsIn;
    int counter;

    //triggered by the collider
    public void PlaceNewGasket(int gasketNumber)
    {
        if (gasketNumber == 1) { newGasket = newGasket1; oldGasket = oldGasket1; }
        else if (gasketNumber == 2) { newGasket = newGasket2; oldGasket = oldGasket1; }
        else if (gasketNumber == 3) { newGasket = newGasket3; oldGasket = oldGasket2; }

        //force release the object
        OVRGrabbable grabbable = newGasket.GetComponent<OVRGrabbable>();
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

        counter++;
        if(counter == 3)
             newGasketsIn = true;
    }
}
