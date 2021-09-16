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
    public bool connector1Removed;
    public bool connector2Removed;
    public bool frontScrewRemoved;
    public bool visualInspectionComplete;

    public Transform backPanel;

    int counter;
    int counter1;
    int counter2;
    int counter3;

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
    public void Connector1Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(CoonectorGrabbedDelay(grabbable, "1"));
    }

    //triggered by the grabbale obj
    public void Connector2Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(CoonectorGrabbedDelay(grabbable, "2"));
    }

    //triggered by the grabbale obj
    public void Connector3Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(CoonectorGrabbedDelay(grabbable, "3"));
        Destroy(backPanel.gameObject.GetComponent<Animator>());
    }

    IEnumerator CoonectorGrabbedDelay(OVRGrabbable grabbable, string connector)
    {
        yield return new WaitForSeconds(0.5f);
        if (connector == "1")
            connector1Removed = true;
        else if(connector == "2")
            connector2Removed = true;
        else if (connector == "3")
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

    //triggered when screws are placed in the snap positions
    public void FrontPanelScrewRemoved()
    {
        counter2++;
        if (counter2 == 4)
            frontScrewRemoved = true;
    }

    //triggered when gazing on each side of the injector (gaze buttons)
    public void VisualInspectionSideCompleted()
    {
        counter3++;
        if (counter3 == 4)
            visualInspectionComplete = true;
    }

}
