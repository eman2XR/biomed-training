using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    [Space(20)]
    [Tooltip("The axis along which the object will rotate")]
    public string axis = "z";

    //public TextMeshProUGUI debugText; //a text field to display the output value

    // bool isOn;
    Transform controller; //reference to grabbing controller
    //VRTK_ControllerReference controllerReference; //for haptics

    bool objectInUse;
    public float value;
    float initialControllerRot;
    bool initialControllerPosFoundT;

    float initialKnobPos;

    [Tooltip("How quickly the knob moves")]
    public float knobSensitivity = 1;

    //haptics
    [Tooltip("How many haptic pulses along the rotation")]
    public int teethCount = 80;

    [Tooltip("Strenght of the haptic pulse (0-1)")]
    public float strenght = 0.5f;

    private int previousToothIndex = -1;
    int currentToothIndex;

    public bool invertValue;


    float initialKnobRot;
    float initialControllerPos;

    float lastAngle;

    private void Start()
    {
        
    }

    public void Grabbed()
    {
        controller = this.GetComponent<OVRGrabbable>().grabbingHand;
        //controllerReference = VRTK_ControllerReference.GetControllerReference(controller.gameObject);
        objectInUse = true;


        //----------------------------------------------------
        if (axis == "y")
        {
            initialControllerRot = controller.transform.rotation.y;
            initialKnobRot = transform.localEulerAngles.y;
            //initialControllerPos = controller.transform.position.x;
        }
        else if (axis == "z")
        {
            initialControllerRot = controller.transform.rotation.z;
            initialKnobRot = transform.localRotation.eulerAngles.z;
            initialControllerPos = controller.transform.position.x;
        }
        //----------------------------------------------------

        //make collider bigger so it's easier to manipulate-------------------------------------------
        //this.GetComponent<Collider>().radius = collider.radius * 6;
        //--------------------------------------------------------------------------------------------

        // Trigger was just pressed
        //lastHandProjected = ComputeToTransformProjected(controller.transform);

        StartCoroutine(IsUsing());
    }

    IEnumerator IsUsing()
    {
        while (objectInUse)
        {
            //if (rotateKnob)
            //{
            //    if (axis == "z")
            //    {
            //        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.rotation.y, ((initialControllerRot - controller.transform.rotation.z) * (knobSensitivity * 20)) + initialKnobRot);
            //    }
            //}

            //rotate knob /UPDATE: this value now is very low so that most of the movement is done by moving the controller left and right instead of rotating your wrist


            if (axis == "y")
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, ((initialControllerRot - controller.transform.rotation.y) * (knobSensitivity * 1)) + initialKnobRot, transform.localEulerAngles.z);
            }
            //print(controller.transform.rotation.z);

            //float sideMovementMultiplier = (controller.transform.position.x - initialControllerPos) * 7;

            //x rotation is 0 at 0 degress rotation and 1 at 180 degrees rotation--------------------------------------------------------
            //if (axis == "x")
            //    if (!invertValue)
            //        value = this.transform.localRotation.x + sideMovementMultiplier;
            //    else value = -this.transform.localRotation.x + sideMovementMultiplier;

            //if (axis == "y")
            //    if (!invertValue)
            //        value = this.transform.localEulerAngles.y;
            //    else value = -this.transform.localEulerAngles.y;

            //else if (axis == "z")
            //    if (!invertValue)
            //        value = this.transform.localRotation.z + sideMovementMultiplier;
            //    else value = -this.transform.localRotation.z + sideMovementMultiplier;
            //----------------------------------------------------------------------------------------------------------------------------

            if(transform.localEulerAngles.y - lastAngle > 5)
            {
                value++;
                lastAngle = transform.localEulerAngles.y;
            }
            else if (lastAngle - transform.localEulerAngles.y > 5)
            {
                value--;
                lastAngle = transform.localEulerAngles.y;
            }

            GetNormalizedValue();

            //-------haptics----------------------------------------------------------
            currentToothIndex = Mathf.RoundToInt(value * teethCount - 0.5f);
            if (currentToothIndex != previousToothIndex)
            {
                //VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strenght, 0.01f, 0.01f);
                //previousToothIndex = currentToothIndex;
            }
            //----------------------------------------------------------------------------

            //debug text
            //if (debugText)
                //debugText.text = value.ToString("F2");

            yield return null;
        }
        yield return null;
    }

    public void GetNormalizedValue()
    {
        
    }

    public void Ungrabbed()
    {
        objectInUse = false;
        initialControllerPosFoundT = false;

        //return collider to original size
        //SphereCollider[] colliders = gameObject.GetComponentsInChildren<SphereCollider>();
        //foreach (SphereCollider collider in colliders)
            //collider.radius = collider.radius / 6;   
    }

}