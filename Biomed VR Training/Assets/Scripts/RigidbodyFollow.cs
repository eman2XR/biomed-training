using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyFollow : MonoBehaviour
{
    public Transform target;
    public bool keepInitialOffeset = true;
    Rigidbody rb;
    Vector3 intialOffset;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        intialOffset = transform.position - target.position;
    }

    void Update()
    {
        if (keepInitialOffeset)
            rb.position = rb.position + intialOffset;
        else
            rb.position = target.position;
    }
}
