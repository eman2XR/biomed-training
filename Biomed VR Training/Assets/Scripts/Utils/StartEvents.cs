using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEvents : MonoBehaviour
{
    public UnityEvent onStart;
    public int delay;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        onStart.Invoke();
    }

}
