using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwivelObject : MonoBehaviour
{
    Transform hand;

    public float value;

    float initialControllerPosT;
    bool initialControllerPosFoundT;
    float initialTurntablePos;
    public float turntableSensitivity;

    bool objectInUse;
    public bool isSpring;
    public float springStrenght = 2;

    [Tooltip("How many haptic pulses along the rotation")]
    public int teethCount = 80;

    [Tooltip("Strenght of the haptic pulse (0-1)")]
    public float strenght = 0.5f;

    private int previousToothIndex = -1;

    public AudioSource audio;

    //----------------------------------------------
    [HeaderAttribute("From Valve")]

    [Tooltip("If true, the transform of the GameObject this component is on will be rotated accordingly")]
    public bool rotateGameObject = true;

    public float minOutAngle = 0;
    public float maxOutAngle = 10000;
    float tempOutAngle;
    public UnityEvent onMaxOutAngleReached;
    public UnityEvent onMaxOutAngleReleased;
    bool isAtMaxAngle;
    [Tooltip("The output angle value of the drive in degrees, unlimited will increase or decrease without bound, take the 360 modulus to find number of rotations")]
    public float outAngle;

    public enum Axis_t
    {
        XAxis,
        YAxis,
        ZAxis
    };

    int currentToothIndex;

    [Tooltip("The axis around which the circular drive will rotate in local space")]
    public Axis_t axisOfRotation = Axis_t.XAxis;

    private Vector3 lastHandProjected;
    private bool frozen = false;
    private float frozenAngle = 0.0f;
    private Vector3 frozenHandWorldPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 frozenSqDistanceMinMaxThreshold = new Vector2(0.0f, 0.0f);
    private Vector3 worldPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 localPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);
    [HeaderAttribute("Limited Rotation")]
    [Tooltip("If true, the rotation will be limited to [minAngle, maxAngle], if false, the rotation is unlimited")]
    public bool limited = false;
    public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);
    public UnityEvent onFrozenDistanceThreshold;
    [HeaderAttribute("Limited Rotation Min")]
    [Tooltip("If limited is true, the specifies the lower limit, otherwise value is unused")]
    public float minAngle = -45.0f;
    [Tooltip("If limited, set whether drive will freeze its angle when the min angle is reached")]
    public bool freezeOnMin = false;
    [Tooltip("If limited, event invoked when minAngle is reached")]
    public UnityEvent onMinAngle;
    private Quaternion start;
    [HeaderAttribute("Limited Rotation Max")]
    [Tooltip("If limited is true, the specifies the upper limit, otherwise value is unused")]
    public float maxAngle = 45.0f;
    [Tooltip("If limited, set whether drive will freeze its angle when the max angle is reached")]
    public bool freezeOnMax = false;
    [Tooltip("If limited, event invoked when maxAngle is reached")]
    public UnityEvent onMaxAngle;
    // If the drive is limited as is at min/max, angles greater than this are ignored 
    private float minMaxAngularThreshold = 1.0f;
    private bool driving = false;
    Quaternion initialRot;

    bool vibrating;

    void Start()
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

        UpdateAll();

        initialRot = this.transform.localRotation;
    }

    public void Grabbed()
    {
        objectInUse = true;

        hand = this.GetComponent<OVRGrabbable>().grabbingHand;
        
        //make collider bigger so it's easier to manipulate-------------------------------------------
        //SphereCollider[] colliders = gameObject.GetComponentsInChildren<SphereCollider>();
        //foreach (SphereCollider collider in colliders)
            //collider.radius = collider.radius * 10;
        //--------------------------------------------------------------------------------------------

        // Trigger was just pressed
        lastHandProjected = ComputeToTransformProjected(hand.transform);

        StartCoroutine(IsUsing());
    }

    IEnumerator IsUsing()
    {
        while (objectInUse)
        {
            driving = true;

            ComputeAngle(hand.transform);
            UpdateAll();

            //-------haptics----------------------------------------------------------
            currentToothIndex = Mathf.RoundToInt(value * teethCount - 0.5f);

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

            if (!audio.isPlaying)
                audio.Play();

            yield return null;
        }
        yield return null;
    }

    public void Ungrabbed()
    {
        objectInUse = false;

        hand = null;

        initialControllerPosFoundT = false;
        if (isSpring)
        {
            StartCoroutine(SpringBackToPosition());
        }

        //return collider to original size
        //SphereCollider[] colliders = gameObject.GetComponentsInChildren<SphereCollider>();
        //foreach (SphereCollider collider in colliders)
        //collider.radius = collider.radius / 10;

        audio.Stop();
    }

    IEnumerator SpringBackToPosition()
    {
        float elapsedTime = 0;
        while (elapsedTime < springStrenght)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * springStrenght);
            value = Mathf.Lerp(value, 0, Time.deltaTime * springStrenght);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //check to zero out values 
        if (elapsedTime > springStrenght)
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            value = 0;
        }
    }

    //-------------------------------------------------
    // Updates the Debug TextMesh with the linear mapping value and the angle
    //-------------------------------------------------
    private void UpdateAll()
    {
        UpdateLinearMapping();
        UpdateGameObject();
        // UpdateDebugText();
    }


    //-------------------------------------------------
    // Updates the Rotation of the GamObject
    //-------------------------------------------------
    private void UpdateGameObject()
    {
        if (rotateGameObject)
        {
            transform.localRotation = start * Quaternion.AngleAxis(outAngle, localPlaneNormal);
            //print(outAngle);
        }
    }

    //-------------------------------------------------
    // Updates the LinearMapping value from the angle
    //-------------------------------------------------
    private void UpdateLinearMapping()
    {
        if (limited)
        {
            // Map it to a [0, 1] value
            value = (outAngle - minAngle) / (maxAngle - minAngle);
        }
        else
        {
            // Normalize to [0, 1] based on 360 degree windings
            float flTmp = outAngle / 360.0f;
            value = flTmp - Mathf.Floor(flTmp);
            //print(value);
        }

        //UpdateDebugText();
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
                if (frozen)
                {
                    float frozenSqDist = (hand.transform.position - frozenHandWorldPos).sqrMagnitude;
                    if (frozenSqDist > frozenSqDistanceMinMaxThreshold.x)
                    {
                        outAngle = frozenAngle + Random.Range(-1.0f, 1.0f);

                        //float magnitude = Util.RemapNumberClamped(frozenSqDist, frozenSqDistanceMinMaxThreshold.x, frozenSqDistanceMinMaxThreshold.y, 0.0f, 1.0f);
                        //if (magnitude > 0)
                        //{
                            //StartCoroutine(HapticPulses(hand.controller, magnitude, 10));
                        //}
                       // else
                        //{
                            // StartCoroutine(HapticPulses(hand.controller, 0.5f, 10));
                        //}

                        if (frozenSqDist >= frozenSqDistanceMinMaxThreshold.y)
                        {
                            onFrozenDistanceThreshold.Invoke();
                        }
                    }
                }
                else
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
                            if (freezeOnMin)
                            {
                                //Freeze(hand.transform);
                            }
                        }
                        else if (angleTmp == maxAngle)
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                            onMaxAngle.Invoke();
                            print("Limit reached");
                            //hand.GetComponent<OVRGrabber>().ForceRelease(GetComponent<OVRGrabbable>());
                            if (freezeOnMax)
                            {
                                //Freeze(hand.transform);
                            }
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
    }

    //-------------------------------------------------
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

    ////-------------------------------------------------
    //private void Freeze(Transform hand)
    //{
    //    frozen = true;
    //    frozenAngle = outAngle;
    //    frozenHandWorldPos = hand.position;
    //    frozenSqDistanceMinMaxThreshold.x = frozenDistanceMinMaxThreshold.x * frozenDistanceMinMaxThreshold.x;
    //    frozenSqDistanceMinMaxThreshold.y = frozenDistanceMinMaxThreshold.y * frozenDistanceMinMaxThreshold.y;
    //}

    ////-------------------------------------------------
    //private void UnFreeze()
    //{
    //    frozen = false;
    //    frozenHandWorldPos.Set(0.0f, 0.0f, 0.0f);
    //}

    //Vector3 grabPos;
    //public bool isGrabbed;
    //public Transform hand;
    //public UnityEvent onGrab;
    //public UnityEvent onUngrab;
    //public UnityEvent onTouch;
    //public UnityEvent onUntouch;

    ////Quaternion initialRot;
    //float initialRotZ;

    //private void Start()
    //{
    //    initialRot = this.transform.rotation;
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.name == "GrabVolumeBig")
    //    {
    //        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
    //        {
    //            Grabbed();
    //            //print("grabbed");
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
    //        Ungrabbed();
    //}

    //public void Grabbed()
    //{
    //    isGrabbed = true;
    //    grabPos = hand.localPosition;
    //    initialRot = this.transform.rotation;
    //    initialRotZ = this.transform.localEulerAngles.z;

    //    StartCoroutine(WhileGrabbed());
    //}

    //public void Ungrabbed()
    //{
    //    isGrabbed = false;
    //}

    //IEnumerator WhileGrabbed()
    //{
    //    while (isGrabbed)
    //    {
    //        //transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, initialRot.z + (hand.position.y - grabPos.y));
    //        float angle = (((hand.localPosition.y - grabPos.y) * 200) + ((grabPos.z - hand.localPosition.z) * 200));
    //        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, initialRotZ + angle);

    //        //float angleDeltaDegrees = (Mathf.Atan2(newRelativeHandPosition.z, newRelativeHandPosition.x) - Mathf.Atan2(relativeHandPosition.z, relativeHandPosition.x)) * Mathf.Rad2Deg;

    //        //calculate angle
    //        //float angle = Vector3.Angle(grabPos - hand.localPosition, transform.position);
    //        //print(angle);

    //        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, initialRotZ + angle);

    //        //transform.LookAt(hand);
    //        //transform.rotation *= Quaternion.Euler(initialRot);

    //        //Vector3 diffRotation = transform.rotation.eulerAngles - initialRot.eulerAngles;
    //        //transform.eulerAngles = transform.eulerAngles - diffRotation;

    //        yield return null;
    //    }
    //}

}
