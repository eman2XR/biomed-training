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

    public Transform targetPosition;
    bool isBusy;
    public bool leftHandOnly;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
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
        //rb.MovePosition(hand.position);
        //
        //
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
        yield return new WaitForSeconds(1.5f);
        handSwing.ParentBack();
        move = false;
        yield return new WaitForSeconds(1f);
        isBusy = false;
        //this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.y)
    }
}
