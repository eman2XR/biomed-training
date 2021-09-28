using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class I_Module : MonoBehaviour {

    GameObject step;
    GameObject currentStep;

    List<GameObject> stepsList = new List<GameObject>();

    bool waited = true;

	public void StartModule(GameObject stepPrefab, List<I_Master.Steps> steps)
    {
        //deactivate all transforms in Module 1
        foreach (Transform trans in transform)
        {
            //except for the Module itself
            if (trans != transform)
            {
                trans.gameObject.SetActive(false);
            }
        }

        //deactivate the other modules
        foreach (Transform trans in transform.parent)
        {
            //except for the Module itself
            if (trans != transform)
            {
                trans.gameObject.SetActive(false);
            }
        }

        //activate steps list
        GetChildByName("Steps").SetActive(true);

        //instatiate each step
        for (int i = 0; i < steps.Count; i++)
        {
            //instantiate the prefab Step object;
            step = Instantiate(stepPrefab, GetChildByName("Steps").transform);

            //setup the step with each component
            step.GetComponent<I_Step>().SetupStep(i, steps);

            //deactivate all steps objects except for the first
            if (i != 0)
                step.SetActive(false);
            else
                currentStep = step;

            //start first step
            if(i == 0)
            step.GetComponent<I_Step>().StartStep();
        }
    }

    public void StartNextStep()
    {
        if (waited)
        {
            StartCoroutine(Delay());
            //print("starting next step");

            //deactivate current step
            currentStep.GetComponent<I_Step>().StopStep();

            //find the next step and activate it
            if (NextChild(currentStep.transform))
            {
                NextChild(currentStep.transform).gameObject.SetActive(true);

                //make next step currentStep
                currentStep = NextChild(currentStep.transform).gameObject;

                //start the step
                currentStep.GetComponent<I_Step>().StartStep();
            }
            else
            {
                print("steps finished");
            }
        }
    }

    public void StartStep(int stepToStart)
    {
        StartCoroutine(Delay());

        //deactivate current step
        currentStep.GetComponent<I_Step>().StopStep();

        //find the step and activate it
        if (GetChildByName("Steps").transform.GetChild(stepToStart+1))
        {
            GetChildByName("Steps").transform.GetChild(stepToStart+1).gameObject.SetActive(true);

            //make it step currentStep
            currentStep = GetChildByName("Steps").transform.GetChild(stepToStart+1).gameObject;

            //start the step
            currentStep.GetComponent<I_Step>().StartStep();
        }
        else
            print("steps finished");
    }

    public void StartPreviousStep()
    {
        //find the previous step and activate it
        PreviousChild(currentStep.transform).gameObject.SetActive(true);

        //deactivate current step
        currentStep.GetComponent<I_Step>().StopStep();
        currentStep.gameObject.SetActive(false);

        //start the previous step
        PreviousChild(currentStep.transform).gameObject.GetComponent<I_Step>().StartStep();

        //make previous step the current step
        currentStep = PreviousChild(currentStep.transform).gameObject;
    }

    //seperate function to be triggered from UI button
    public void StartModuleInternal(bool isExtraModule = false)
    {
        this.transform.root.GetComponent<I_Master>().StartModule(isExtraModule);
    }

    //helper function to get the child gameObject by name
    GameObject GetChildByName(string name)
    {
        //loop through all childern
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            { return child.gameObject; }
        }
        return null;
    }

    private Transform NextChild(Transform trans)
    {
        // Check where the transform is
        int thisIndex = trans.GetSiblingIndex();

        // We have a few cases to rule out
        if (trans.transform.parent == null)
            return null;
        if (trans.transform.parent.childCount <= thisIndex + 1)
            return null;

        // Then return whatever was next, now that we're sure it's there
        return trans.transform.parent.GetChild(thisIndex + 1);
    }

    private Transform PreviousChild(Transform trans)
    {
        // Check where the transform is
        int thisIndex = trans.GetSiblingIndex();

        // We have a few cases to rule out
        if (trans.transform.parent == null)
            return null;
        if (trans.transform.parent.childCount <= thisIndex - 1)
            return null;

        // Then return whatever was next, now that we're sure it's there
        return trans.transform.parent.GetChild(thisIndex - 1);
    }

    IEnumerator Delay()
    {
        waited = false;
        yield return new WaitForSeconds(1);
        waited = true;
    }
}
