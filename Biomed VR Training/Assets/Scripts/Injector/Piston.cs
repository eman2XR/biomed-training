using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Piston : MonoBehaviour
{
    public Knob knob;
    float initialX;
    public float targetPos;
    public UnityEvent onTargetReached;

    void Start()
    {
        initialX = this.transform.localPosition.x;
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(initialX - ExtensionMethods.RemapValue(knob.outAngle, 0, 100, 0, 0.3f), transform.localPosition.y, transform.localPosition.z);
        if(this.transform.localPosition.x < targetPos)
        {
            onTargetReached.Invoke();
            this.enabled = false;
        }
    }

}
