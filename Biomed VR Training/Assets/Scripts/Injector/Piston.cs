using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    public Knob knob;
    float initialY;

    void Start()
    {
        initialY = this.transform.localPosition.y;
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(transform.localPosition.x, initialY + ExtensionMethods.RemapValue(knob.value, 0, 100, 0, 0.1f), transform.localPosition.z);
    }


}
