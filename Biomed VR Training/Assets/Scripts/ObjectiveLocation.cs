using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveLocation : MonoBehaviour
{
    public Transform objective;
    public ObjectiveLocation linkedObjective;

    public float delay = 1f;
    public float distance = 0.05f;
    bool completed;
    public bool fullyCompleted;

    public UnityEvent OnObjectiveCompleted;
    public UnityEvent onTriggerEnter;

    
    void OnTriggerStay(Collider other)
    {
        if (other.transform == objective || other.transform.parent == objective)
        {
            completed = true;
            onTriggerEnter.Invoke();
            //StartCoroutine(CheckIfObjectiveCompleted());
            if (linkedObjective.completed == true && !fullyCompleted)
            {
                StartCoroutine(ObjectiveCompleted());
                fullyCompleted = true;
                //StartCoroutine(ObjectiveCompleted());
                //print("COMPLETED");
            }
        }
    }

    IEnumerator CheckIfObjectiveCompleted()
    {
        yield return new WaitForSeconds(0.1f);
        if (linkedObjective.completed == true)
        {
            StartCoroutine(ObjectiveCompleted());
            yield break;
        }
        StartCoroutine(CheckIfObjectiveCompleted());
    }

    IEnumerator ObjectiveCompleted()
    {
        yield return new WaitForSeconds(delay);
        OnObjectiveCompleted.Invoke();
        fullyCompleted = true;
        //this.gameObject.SetActive(false);
    }

}
