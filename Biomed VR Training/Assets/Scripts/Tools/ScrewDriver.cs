using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScrewDriver : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    float _lastTransformPositionRotationAngleOnProgressAccumulatingAxis;
    public Transform hand;

    public float movedAngles;
    OVRGrabbable grabbable;

    private void Start()
    {
        grabbable = this.GetComponent<OVRGrabbable>();
    }

    private void Update()
    {
        if(hand) GetMovedAngle();
        //debugText.text = hand.eulerAngles.z.ToString();
    }

    private void GetMovedAngle()
    {
        // Start our drawn line at the object's position in world space.
        Vector3 lineStart = hand.position;

        // From here, extend 3 units in worldspace along the object's local forward direction.
        Vector3 lineEnd = lineStart + 0.1f * hand.transform.forward;

        // Draw the resuting line.
        //Debug.DrawLine(lineStart, lineEnd, Color.blue, Time.deltaTime);

        var currentAngleOnProgressAccumulatingAxis = hand.eulerAngles.z;
            var rotationChangeAngle = Mathf.DeltaAngle(currentAngleOnProgressAccumulatingAxis, _lastTransformPositionRotationAngleOnProgressAccumulatingAxis);
            _lastTransformPositionRotationAngleOnProgressAccumulatingAxis = currentAngleOnProgressAccumulatingAxis;
            
        var absoluteDriverMovedAngle = Mathf.Abs(-rotationChangeAngle);
        
        if (absoluteDriverMovedAngle > 0.1f)
        {
            //screw.rotation;
            movedAngles = (rotationChangeAngle * 2);
            //UpdateRotation(absoluteDriverMovedAngle, -rotationChangeAngle);
        }
    }

    //triggered by the PivotPoint
    public void HandInPlace(Transform inHand)
    {
        hand = inHand;
        _lastTransformPositionRotationAngleOnProgressAccumulatingAxis = hand.eulerAngles.z;

        //make screwdriver fixed even if dropped
        grabbable.isGrabbable = false;
    }

    //triggered by the screw
    public void ScrewUndone()
    {
        //make screwdriver fixed even if dropped
        grabbable.isGrabbable = true;
        grabbable.GetComponent<Outline>().enabled = false;
        StartCoroutine(DropWithDelay());
    }

    IEnumerator DropWithDelay()
    {
        yield return new WaitForSeconds(1);
        if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            grabbable.GetComponent<Outline>().enabled = false;
            if(grabbable.grabbingHand)
                grabbable.grabbingHand.GetComponent<OVRGrabber>().ForceRelease(grabbable);
        }
    }

    void UpdateRotation(float absoluteDriverMovedAngle, float driverMovedAngle)
    {
        var maxAnglesPerSecond = 10;
        var maxAngleMoveTimeInterpolated = Mathf.Lerp(
            0f,
            maxAnglesPerSecond,
            Time.deltaTime
        );

        var movedAnglesAfterAdjustment = Mathf.Min(absoluteDriverMovedAngle, maxAngleMoveTimeInterpolated);
        movedAnglesAfterAdjustment = driverMovedAngle < 0 ? movedAnglesAfterAdjustment * -1 : movedAnglesAfterAdjustment;

        if (Mathf.Abs(movedAnglesAfterAdjustment) > 0)
        {
            movedAngles = movedAnglesAfterAdjustment;
            //transform.RotateAround(rotationPivot.position, transform.TransformDirection(Vector3.up), movedAnglesAfterAdjustment*5);
            //transform.Rotate(0, movedAnglesAfterAdjustment * 5, 0, Space.Self);
            //transform.rotation = _lastToolRotation * Quaternion.Euler(movedAnglesAfterAdjustment * 5, 0, 0);
            //_lastToolRotation = transform.rotation;
        }
    }

    //public void RotateAroundTracked(this Transform t, Vector3 point, Vector3 axis, float angle) => t.RotateAround(point, axis, angle);


}
