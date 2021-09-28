using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Knob : MonoBehaviour
{
    [Space(20)]
    [Tooltip("The axis along which the object will rotate")]
    public string axis = "z";

    //public TextMeshProUGUI debugText; //a text field to display the output value

    // bool isOn;

    bool objectInUse;
    public float value;
    float initialControllerRot;
    Vector3 initialControllerLocalRot;
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

    private Vector3 worldPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 lastHandProjected;
    Transform hand;
    public bool limited;
    [Tooltip("The output angle value of the drive in degrees, unlimited will increase or decrease without bound, take the 360 modulus to find number of rotations")]
    public float outAngle;
    public float minAngle = 0;
    public float maxAngle = 180;
    // If the drive is limited as is at min/max, angles greater than this are ignored 
    private float minMaxAngularThreshold = 1.0f;

    public float minOutAngle = 0;
    public float maxOutAngle = 10000;
    float tempOutAngle;
    public UnityEvent onMaxOutAngleReached;
    public UnityEvent onMaxOutAngleReleased;
    bool isAtMaxAngle;

    [Tooltip("If limited, set whether drive will freeze its angle when the min angle is reached")]
    public bool freezeOnMin = false;
    [Tooltip("If limited, event invoked when minAngle is reached")]
    public UnityEvent onMinAngle;
    private Quaternion start;
    [Tooltip("If limited, set whether drive will freeze its angle when the max angle is reached")]
    public bool freezeOnMax = false;
    [Tooltip("If limited, event invoked when maxAngle is reached")]
    public UnityEvent onMaxAngle;
    public enum Axis_t
    {
        XAxis,
        YAxis,
        ZAxis
    };

    [Tooltip("The axis around which the circular drive will rotate in local space")]
    public Axis_t axisOfRotation = Axis_t.XAxis;
    private Vector3 localPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);

    private void Start()
    {
        worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f);
        worldPlaneNormal[(int)axisOfRotation] = 1.0f;

        localPlaneNormal = worldPlaneNormal;

        if (transform.parent)
        {
            worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormal).normalized;
        }

        if (limited)
        {
            start = Quaternion.identity;
            outAngle = transform.localEulerAngles[(int)axisOfRotation];
        }
        else
        {
            start = Quaternion.AngleAxis(transform.localEulerAngles[(int)axisOfRotation], localPlaneNormal);
            outAngle = 0.0f;
        }

        //UpdateAll();
    }

    public void Grabbed()
    {
        hand = this.GetComponent<OVRGrabbable>().grabbingHand;

        // Trigger was just pressed
        lastHandProjected = ComputeToTransformProjected(hand.transform);

        //controllerReference = VRTK_ControllerReference.GetControllerReference(controller.gameObject);
        objectInUse = true;

        //----------------------------------------------------
        //if (axis == "y")
        //{
        //    initialControllerRot = controller.transform.eulerAngles.y;
        //    initialKnobRot = transform.localEulerAngles.y;
        //    //initialControllerPos = controller.transform.position.x;
        //}
        //else if (axis == "z")
        //{
        //    initialControllerRot = controller.transform.rotation.z;
        //    initialKnobRot = transform.localRotation.eulerAngles.z;
        //    initialControllerPos = controller.transform.position.x;
        //}
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
            //update rotation
            ComputeAngle(hand.transform);
            if(invertValue)
                transform.localRotation = start * Quaternion.AngleAxis(Mathf.Clamp(outAngle*2, -10000, 0), localPlaneNormal);
            else
                transform.localRotation = start * Quaternion.AngleAxis(Mathf.Clamp(-outAngle*2, -10000, 0), localPlaneNormal);

            //UpdateAll();

            //if (axis == "y")
            //{
            //    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, ((initialControllerRot - controller.transform.eulerAngles.y) * (knobSensitivity * 1)) + initialKnobRot, transform.localEulerAngles.z);
            //    //transform.localEulerAngles = transform.localEulerAngles * 
            //}

            //-------haptics----------------------------------------------------------
            currentToothIndex = Mathf.RoundToInt(outAngle * teethCount - 0.5f);

            if (currentToothIndex != previousToothIndex)
            {
                // starts vibration on the right Touch controller
                if (hand.name.Contains("Left"))
                    ControllerHaptics.instance.CreateVibrateTime(2, 2, 3, OVRInput.Controller.LTouch, 0.15f);
                else
                    ControllerHaptics.instance.CreateVibrateTime(2, 2, 3, OVRInput.Controller.RTouch, 0.15f);
                //VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strenght, 0.01f, 0.01f);
                previousToothIndex = currentToothIndex;
            }
            //----------------------------------------------------------------------------


            //if (transform.localEulerAngles.y - lastAngle > 5)
            //{
            //    value++;
            //    lastAngle = transform.localEulerAngles.y;
            //}
            //else if (lastAngle - transform.localEulerAngles.y > 5)
            //{
            //    value--;
            //    lastAngle = transform.localEulerAngles.y;
            //}

            //GetNormalizedValue();

            yield return null;
        }
        yield return null;
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

    //-------------------------------------------------
    // Computes the angle to rotate the game object based on the change in the transform
    //-------------------------------------------------
    void ComputeAngle(Transform hand)
    {
        Vector3 toHandProjected = ComputeToTransformProjected(hand);

        if (!toHandProjected.Equals(lastHandProjected))
        {
            float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);

            if (absAngleDelta > 0.0f)
            {
                
                    Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                    float dot = Vector3.Dot(worldPlaneNormal, cross);

                    float signedAngleDelta = absAngleDelta;

                    if (dot < 0.0f)
                    {
                        signedAngleDelta = -signedAngleDelta;
                    }

                    if (limited)
                    {
                        float angleTmp = Mathf.Clamp(outAngle + signedAngleDelta, minAngle, maxAngle);

                        if (outAngle == minAngle)
                        {
                            if (angleTmp > minAngle && absAngleDelta < minMaxAngularThreshold)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                            }
                        }
                        else if (outAngle == maxAngle)
                        {
                            if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThreshold)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                            }
                        }
                        else if (angleTmp == minAngle)
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                            onMinAngle.Invoke();
                            print("Limit reached");
                            //hand.GetComponent<OVRGrabber>().ForceRelease(GetComponent<OVRGrabbable>());
                        }
                        else if (angleTmp == maxAngle)
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                            onMaxAngle.Invoke();
                            print("Limit reached");
                            //hand.GetComponent<OVRGrabber>().ForceRelease(GetComponent<OVRGrabbable>());
                        }
                        else
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                        }
                    }
                    else
                    {
                        //outAngle += signedAngleDelta;
                        outAngle = Mathf.Clamp(outAngle += signedAngleDelta, minOutAngle, maxOutAngle);

                        //angle limits events
                        if (outAngle > (maxOutAngle - 40))
                        {
                            if (!isAtMaxAngle)
                            {
                                onMaxOutAngleReached.Invoke();
                                tempOutAngle = outAngle;
                                isAtMaxAngle = true;
                                //print("max angle reached");
                            }
                        }
                        if (isAtMaxAngle && outAngle < (tempOutAngle - 40))
                        {
                            onMaxOutAngleReleased.Invoke();
                            isAtMaxAngle = false;
                            //print("max angle released");
                        }

                        lastHandProjected = toHandProjected;
                        //print(signedAngleDelta);
                    }
            }
        }
    }

    private Vector3 ComputeToTransformProjected(Transform xForm)
    {
        Vector3 toTransform = (xForm.position - transform.position).normalized;
        Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

        // Need a non-zero distance from the hand to the center of the CircularDrive
        if (toTransform.sqrMagnitude > 0.0f)
        {
            toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormal).normalized;
        }
        else
        {
            Debug.LogFormat("The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString());
            Debug.Assert(false, string.Format("The collider needs to be a minimum distance away from the CircularDrive GameObject {0}", gameObject.ToString()));
        }
        return toTransformProjected;
    }

}