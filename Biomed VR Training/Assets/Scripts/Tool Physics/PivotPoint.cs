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
        
        if(isBusy && Vector3.Distance(hand.position, this.transform.position) > 0.25f)
            Detach();

        //if(isBusy)
            //handSwing.transform.GetChild(0).localEulerAngles = new Vector3(-hand.localEulerAngles.z, handSwing.transform.GetChild(0).localEulerAngles.y, /handSwing.transform.GetChild(0).localEulerAngles.z);
    }

    public void Detach()
    {
        handSwing.ParentBack();
        move = false;
        isBusy = false;
        //print("unhook");
    }

    IEnumerator Attach()
    {
        isBusy = true;
        yield return new WaitForSeconds(0.1f);
        if(isScrew)
            handSwing.IsUsingScrew(this.transform, targetPosition, leftHandOnly);
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

                if (isScrew && !isTurning)
                {
                    screw.ScrewdriverTurned();
                    isTurning = true;
                    //initialHandRotation = hand.localRotation.eulerAngles;
                }

                //print("turn screw");
                //yield break;
            }
            yield return null;
        }

        //yield return new WaitForSeconds(5f);
        //handSwing.ParentBack();
        //move = false;
        //yield return new WaitForSeconds(1f);
        //isBusy = false;
        ////this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //add a twist rotation from the wrist
        if (isBusy)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - ((hand.localEulerAngles.z - initialHandZ)/1));
    }
}
