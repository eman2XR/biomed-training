using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEvents : MonoBehaviour
{
    public UnityEvent onStart;
    public UnityEvent onEnable;
    public int delay;

    private void OnEnable()
    {
        StartCoroutine(OnEnableEvents());
    }

    IEnumerator OnEnableEvents()
    {
        yield return new WaitForSeconds(delay);
        onEnable.Invoke();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        onStart.Invoke();
    }

}
