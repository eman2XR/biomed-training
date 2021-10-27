using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    Rigidbody rb;
    
    public Transform hand;
    public HandSwingTest handSwing;
    
    public bool move;
    public bool isScrew;
    public bool isTurning;

    public Transform targetPosition;
    public Transform targetPositionLeft;
    bool isBusy;
    public bool leftHandOnly;

    Screw screw;

    Vector3 initialHandRotation;
    Vector3 initialPivotRotation;
    float initialHandZ;

    //haptics 
    int currentToothIndex;
    int previousToothIndex;
    public int teethCount = 80;
    public int value;

    Quaternion lastToolRotation;

    public ScrewDriver screwdriver;
    float initiaZRot;

    public float unhookDistance = 0.25f;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        screw = this.GetComponentInParent<Screw>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBusy)
            return;

        if (other.transform.parent)
        {
            if (other.transform.parent.GetComponent<OVRGrabbable>())
            {
                if (other.transform.parent.GetComponent<OVRGrabbable>().grabbingHand)
                {
                    handSwing = other.transform.parent.GetComponent<OVRGrabbable>().grabbingHand.GetComponent<HandSwingTest>();                 
                    hand = handSwing.transform.parent;
                    //hand = handSwing.pivot;

                    if (isScrew)
                    {
                        screwdriver = other.transform.parent.GetComponent<ScrewDriver>();
                        screwdriver.HandInPlace(hand);

                        //other.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
                        //other.transform.parent.GetComponent<OVRGrabbable>().enableRigibodyOnDrop = false;
                    }

                    initialHandRotation = hand.localEulerAngles;
                    initialHandZ = hand.localEulerAngles.z;

                    if (hand.name.Contains("Left"))
                        ControllerHaptics.instance.CreateVibrateTime(5, 5, 15, OVRInput.Controller.LTouch, 0.1f);
                    else 
                        ControllerHaptics.instance.CreateVibrateTime(5, 5, 15, OVRInput.Controller.RTouch, 0.1f);
                }
            }
        }

        if (isScrew)
        {
            if (other.tag == "phillipsScrewdriver")
            {
                move = true;
                StartCoroutine(Attach());
            }
        }
        else
        {
            if (other.tag == "screwdriver")
            {
                move = true;
                StartCoroutine(Attach());
            }
        }
    }

    private void Update()
    {
        if (move)
            this.transform.LookAt(hand);
        
        if(isBusy && Vector3.Distance(hand.position, this.transform.position) > unhookDistance)
            Detach();

        //if(isBusy)
            //handSwing.transform.GetChild(0).localEulerAngles = new Vector3(-hand.localEulerAngles.z, handSwing.transform.GetChild(0).localEulerAngles.y, /handSwing.transform.GetChild(0).localEulerAngles.z);
    }

    public void Detach()
    {
        handSwing.ParentBack();
        move = false;
        isBusy = false;
        if (screwdriver) 
            screwdriver.DetachedFromScrew();
        if (isScrew && !screw.fullyUnscrewed)
            screw.transform.GetChild(2).gameObject.SetActive(true); //show ghosting
        //print("unhook");
    }

    IEnumerator Attach()
    {
        initiaZRot = this.transform.localEulerAngles.z;
        isBusy = true;
        if(isScrew) move = false;
        yield return new WaitForSeconds(0.1f);
        if(isScrew)
            handSwing.IsUsingScrew(this.transform, targetPosition, targetPositionLeft);
        else
            handSwing.IsUsingKnob(this.transform, targetPosition, leftHandOnly);
        //handSwing.transform.localPosition = targetPosition.localPosition;

        while (isBusy)
        {
            //print("screwdriver is in");

            if (Vector3.Distance(initialHandRotation, hand.localRotation.eulerAngles) > 60f)
            {             
                //-------haptics----------------------------------------------------------
                value = (int)Vector3.Distance(initialHandRotation, hand.localRotation.eulerAngles);

                if (isScrew)
                    currentToothIndex = Mathf.RoundToInt(screwdriver.movedAngles * teethCount - 0.5f);
                else
                    currentToothIndex = Mathf.RoundToInt(value * teethCount - 0.5f);

                if (currentToothIndex != previousToothIndex)
                {
                    if (hand.name.Contains("Left"))
                        ControllerHaptics.instance.CreateVibrateTime(2, 2, 3, OVRInput.Controller.LTouch, 0.15f);
                    else
                        ControllerHaptics.instance.CreateVibrateTime(2, 2, 3, OVRInput.Controller.RTouch, 0.15f);
                    previousToothIndex = currentToothIndex;
                }
                //----------------------------------------------------------------------------

                if (isScrew)
                {
                    screw.ScrewdriverTurned(screwdriver.movedAngles);
                }

                if (isScrew && !isTurning)
                {
                    isTurning = true;
                    //initialHandRotation = hand.localRotation.eulerAngles;
                }

                //print("turn screw");
                //yield break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(5f);
        handSwing.ParentBack();
        move = false;
        yield return new WaitForSeconds(1f);
        isBusy = false;
        ////this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //add a twist rotation from the wrist
        if (isBusy & isScrew)
        {
            if (screwdriver.movedAngles < 1)
            {
                //if (initiaZRot - transform.localEulerAngles.z > 2)
                //{
                    Vector3 targetRot = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + ((screwdriver.movedAngles)));
                    transform.localEulerAngles = targetRot;

                    screw.ScrewdriverTurned(screwdriver.movedAngles);

                //}
            }
            else
            {
                //Vector3 targetRot = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + ((10)));
                //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetRot, Time.deltaTime * 10);
            }

            //Vector3 targetRot = new Vector3(screwdriver.transform.localEulerAngles.x, screwdriver.transform.localEulerAngles.y, screwdriver.transform.localEulerAngles.z - ((screwdriver.movedAngles)));
            //screwdriver.transform.localEulerAngles = Vector3.Lerp(screwdriver.transform.localEulerAngles, targetRot, Time.deltaTime * 25);
        }
    }

}
