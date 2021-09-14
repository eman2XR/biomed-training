using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Injector : MonoBehaviour
{
    //used by Induction master to track if step completed
    public bool isInverted;
    public bool isHorizontal;
    public bool backScrewsRemoved;
    public bool strainReliefLoosened;
    public bool backPanelOpened;
    public bool connector3Removed;

    public Transform backPanel;

    int counter;
    int counter1;

    void Update()
    {
        if(!isInverted)
        {
            if (this.transform.localEulerAngles.z == 170)
            {
                isInverted = true;
            }
        }

        if (isInverted && !isHorizontal)
        {
            if (this.transform.localEulerAngles.z < -90)
            {
                isHorizontal = true;
            }
        }
    }

    //triggered when screws are placed in the snap positions
    public void BackPanelScrewRemoved()
    {
        counter++;
        if (counter == 4)
            backScrewsRemoved = true;
    }

    //triggered when screws are placed in the snap positions
    public void StrainReliefScrewRemoved()
    {
        counter1++;
        if (counter1 == 2)
            strainReliefLoosened = true;
    }

    //triggered by the animation
    public void BackPanelOpened()
    {
        backPanelOpened = true;
    }

    //triggered by the grabbale obj
    public void Connector3Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(CoonectorGrabbedDelay(grabbable));
    }

    IEnumerator CoonectorGrabbedDelay(OVRGrabbable grabbable)
    {
        yield return new WaitForSeconds(0.5f);
        connector3Removed = true;
        //force release the object
        if (grabbable.grabbingHand)
            grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);

        //disable collider and physics
        grabbable.GetComponent<Rigidbody>().isKinematic = true;
        grabbable.GetComponent<Outline>().enabled = false;
        grabbable.GetComponent<Collider>().enabled = false;

        grabbable.transform.parent = backPanel;
    }

}
