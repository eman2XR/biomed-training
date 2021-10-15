using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEvents : MonoBehaviour
{
    public UnityEvent eventOne;

    public void TriggerEventOne()
    {
        eventOne.Invoke();
    }
}
