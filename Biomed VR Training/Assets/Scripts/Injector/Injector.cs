using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Injector : MonoBehaviour
{
    //used by Induction master to track if step completed
    public bool isInverted;
    public bool isHorizontal;
    public bool is45Foward;
    public bool isUninverted;

    public bool backScrewsRemoved;
    public bool lensCapLoosened;
    public bool strainReliefLoosened;
    public bool backPanelOpened;
    public bool connector3Removed;
    public bool connector1Removed;
    public bool connector2Removed;
    public bool connector1and2Removed;
    public bool frontScrewRemoved;
    public bool visualInspectionComplete;
    public bool knobsBackIn;
    public bool pistonHeadsUp;
    public bool pistonRubberHeadsRemoved;
    public bool pistonHeadsGreased;
    public bool pistonHeadsReinstalled;
    public bool pistonRubberSleevesDown;
    public bool pistonHeadsGreasedAgain;
    public bool pistonRubberSleevesUp;
    public bool opticalCable1Unplugged;
    public bool opticalCable2Unplugged;
    public bool board1ScrewsRemoved;
    public bool board2ScrewsRemoved;
    public bool newGasketIn;

    public Transform backPanel;
    public Transform newGasket;
    public Transform oldGasket;
    public GameObject horizontalDirections;

    int counter;
    int counter1;
    int counter2;
    int counter3;
    int counter4;
    int counter5;
    int counter6;
    int counter7;
    int counter8;
    int counter9;
    int counter10;
    int counter11;
    int counter12;
    int counter13;

    OVRGrabbable grabbable;

    public UnityEvent onKnobsBackIn;

    private void Start()
    {
        grabbable = this.GetComponent<OVRGrabbable>();
    }

    void Update()
    {
        if (!isInverted)
        {
            if (this.transform.localEulerAngles.z >= 165 && this.transform.localEulerAngles.z <= 185)
            {
                if (grabbable.grabbingHand)
                    grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
                isInverted = true;
            }
        }

        if (isInverted && !isHorizontal)
        {
            if (Mathf.Abs(this.transform.localEulerAngles.z) >= 260 && Mathf.Abs(this.transform.localEulerAngles.z) <= 280)
            {
                if (grabbable.grabbingHand)
                    grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
                horizontalDirections.SetActive(false);
                isHorizontal = true;
            }
        }

        //print(this.transform.localEulerAngles.z);

        //if (isInverted && is45Back && !is45Foward)
        //{
        //    if (this.transform.localEulerAngles.z >= 180)
        //    {
        //        this.GetComponent<Collider>().enabled = false;
        //        grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
        //        is45Foward = true;
        //    }
        //}

        //if (isInverted && is45Back && is45Foward && !isUninverted)
        //{
        //    if (this.transform.localEulerAngles.z <= 5)
        //    {
        //        this.GetComponent<Collider>().enabled = false;
        //        grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
        //        isUninverted = true;
        //    }
        //}

    }

    //triggered when screws are placed in the snap positions
    public void LensCapScrewRemoved()
    {
        counter13++;
        if (counter13 == 2)
            lensCapLoosened = true;
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
        yield return new WaitForSeconds(1f);
        if (connector == "1")
        {
            connector1Removed = true;
            if (connector2Removed)
                connector1and2Removed = true;
        }
        else if (connector == "2")
        {
            connector2Removed = true;
            if (connector1Removed)
                connector1and2Removed = true;
        }
        else if (connector == "3")
            connector3Removed = true;

        //force release the object
        if (grabbable.grabbingHand)
            grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);

        grabbable.GetComponent<Collider>().enabled = false;
        grabbable.GetComponent<Connector>().enabled = false;
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

    //triggered by the knobs snap pos
    public void KnobBackIn()
    {
        counter4++;
        if (counter4 == 2)
        {
            knobsBackIn = true; 
            onKnobsBackIn.Invoke();
        }
    }

    //triggered by the piston heads script
    public void PistonHeadUp()
    {
        counter5++;
        if (counter5 == 2)
            pistonHeadsUp = true;
    }

    //triggered by the piston heads Grabbable
    public void PistonRubberHeadRemoved()
    {
        counter6++;
        if (counter6 == 2)
            pistonRubberHeadsRemoved = true;
    }

    //triggered by the piston heads trigger collider
    public void PistonHeadGreased()
    {
        counter7++;
        if (counter7 == 2)
        {
            pistonHeadsGreased = true;
        }
        if (counter7 == 4)
        {
            pistonHeadsGreasedAgain = true;
        }
    }

    //triggered by the piston heads trigger collide
    public void PistonHeadReinstalled()
    {
        counter8++;
        if (counter8 == 2)
            pistonHeadsReinstalled = true;
    }

    //triggered by the piston heads rubber sleeve grabbable
    public void PistonRubberSleeveDown()
    {
        counter9++;
        if (counter9 == 2)
            pistonRubberSleevesDown = true;
    }

    //triggered by the piston heads rubber sleeve animation
    public void PistonRubberSleeveUp()
    {
        counter10++;
        if (counter10 == 2)
            pistonRubberSleevesUp = true;
    }

    public void OpticalConnector1Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(OpticalConectorGrabbedDelay(grabbable, "1"));
    }


    public void OpticalConnector2Grabbed(OVRGrabbable grabbable)
    {
        StartCoroutine(OpticalConectorGrabbedDelay(grabbable, "2"));
    }

    IEnumerator OpticalConectorGrabbedDelay(OVRGrabbable grabbable, string connector)
    {
        yield return new WaitForSeconds(1f);
        if (connector == "1")
            opticalCable1Unplugged = true;
        else if (connector == "2")
            opticalCable2Unplugged = true;

        //force release the object
        if (grabbable.grabbingHand)
            grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);

        grabbable.GetComponent<Collider>().enabled = false;
        grabbable.GetComponent<Connector>().enabled = false;
        grabbable.transform.parent = this.transform;
    }

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

        grabbable.transform.parent = this.transform;

        //audio
        grabbable.GetComponent<AudioSource>().Play();

        newGasketIn = true;
    }


    //triggered when screws are placed in the snap positions
    public void Board1ScrewRemoved()
    {
        counter11++;
        if (counter11 == 2)
            board1ScrewsRemoved = true;
    }

    //triggered when screws are placed in the snap positions
    public void Board2ScrewRemoved()
    {
        counter12++;
        if (counter12 == 2)
            board2ScrewsRemoved = true;
    }

}
