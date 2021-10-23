using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastorWheels : MonoBehaviour
{
    public SwivelObject rotator;
    public List<Transform> wheels = new List<Transform>();
    float lastOutAngle;
    List<float> initalRot = new List<float>();

    private void Start()
    {
        foreach(Transform wheel in wheels)
            initalRot.Add(wheel.localEulerAngles.z);
    }

    private void Update()
    {
        if (Mathf.Abs(lastOutAngle - rotator.outAngle) > 5)
        {
            if (lastOutAngle > rotator.outAngle)
            {
                foreach(Transform wheel in wheels)
                    wheel.localEulerAngles = new Vector3(wheel.localEulerAngles.x, wheel.localEulerAngles.y, initalRot[wheels.IndexOf(wheel)] - 8);
                lastOutAngle = rotator.outAngle;
            }
            if (lastOutAngle < rotator.outAngle)
            {
                foreach (Transform wheel in wheels)
                    wheel.localEulerAngles = new Vector3(wheel.localEulerAngles.x, wheel.localEulerAngles.y, initalRot[wheels.IndexOf(wheel)] + 8);
                lastOutAngle = rotator.outAngle;
            }
        }
    }
}
