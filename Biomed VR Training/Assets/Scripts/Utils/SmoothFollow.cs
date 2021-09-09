using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public bool followPosition = true;
    public bool followRotation = true;
    public Transform target;
    public float smoothPos = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public float smoothRot = 0.3F;
    public float distance = 5.0F;

    public Vector3 offset;
    public bool keepInitialOffset;
    Vector3 initialOffset;

    private void Start()
    {
        initialOffset = transform.position - target.position;
    }

    private void Update()
    {
        // Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));
        if (keepInitialOffset)
            transform.position = Vector3.SmoothDamp(transform.position, target.position + initialOffset + offset, ref velocity, smoothPos);
        else
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothPos);

        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothRot);
    }
}
