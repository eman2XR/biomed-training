using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    public string colliderTag;
    public UnityEvent onTriggerEnter;

    void OnTriggerEnter(Collider other )
    {
        if (other.tag == colliderTag)
            onTriggerEnter.Invoke();
    }
    
}
