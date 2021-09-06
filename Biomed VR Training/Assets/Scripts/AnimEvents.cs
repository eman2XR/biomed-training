using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvents : MonoBehaviour
{
    public UnityEvent eventOne;
    public UnityEvent eventTwo;

    public void TriggerEventOne()
    {
        eventOne.Invoke();
    }

    public void TriggerEventTwo()
    {
        eventTwo.Invoke();
    }
}
