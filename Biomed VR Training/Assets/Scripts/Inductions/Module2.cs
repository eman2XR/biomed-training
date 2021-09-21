/***************************************************************************************
Purpose: set up scene with the objects and components in the right places for Module 2
***************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Module2 : MonoBehaviour
{
    public List<Transform> objToBeMoved = new List<Transform>();
    public List<Transform> objToBeMovedTargets = new List<Transform>();

    public UnityEvent onModule2Start;

    public I_Master induction;
    public int startingStep = 10;

    public int secondEventDelay = 1;
    public UnityEvent onModuleStartWithDelay;

    private void OnEnable()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        induction.StartStep(startingStep);

        foreach (Transform trans in objToBeMoved)
        {
            if (!trans.name.Contains("Injector"))
                trans.parent = null;
            trans.position = objToBeMovedTargets[objToBeMoved.IndexOf(trans)].position;
            trans.rotation = objToBeMovedTargets[objToBeMoved.IndexOf(trans)].rotation;
        }

        onModule2Start.Invoke();

        yield return new WaitForSeconds(secondEventDelay);
        onModuleStartWithDelay.Invoke();
    }
}
