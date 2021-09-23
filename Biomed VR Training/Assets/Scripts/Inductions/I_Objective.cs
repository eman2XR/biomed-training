using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VRTK;

public class I_Objective : MonoBehaviour {

    #region Variables
    public string objectiveType;
    OVRGrabbable grabbable;
    I_Step stepScript;
    string str;

    //custom objective refferences
    public Component customObjective;
    public string customObjectiveVar;
    public bool customObjectiveVarValBool;
    public int customObjectiveVarValInt;

    bool customObjectiveIsBool;

    //custom objective
    #endregion

    private void Awake()
    {
        //get refferences
        if (GetComponent<OVRGrabbable>())
            grabbable = GetComponent<OVRGrabbable>();
        else if (GetComponentInParent<OVRGrabbable>())
            grabbable = GetComponentInParent<OVRGrabbable>();
    }

    public void Start()
    {
    }

    public void SetupObjective(I_Step stepScriptI, string objectiveTypeI)
    {
        stepScript = stepScriptI;
        objectiveType = objectiveTypeI;
        
        if (objectiveType == "grab")
        {
            grabbable.onGrabStart.AddListener(DoGrabbed);
        }
        else if (objectiveType == "touch")
        {
            if (grabbable == null)
            {
                grabbable = this.GetComponent<OVRGrabbable>();
            }
            grabbable.onTouchStart.AddListener(DoTouched);
        }
    }

    public void SetupCustomObjective(I_Step stepScriptI, Component customObjectiveI, string customObjectiveVarI, bool customObjectiveVarValBoolI, int customObjectiveVarValIntI)
    {
        stepScript = stepScriptI;
        //public refferences
        customObjective = customObjectiveI;
        customObjectiveVar = customObjectiveVarI;
        customObjectiveVarValBool = customObjectiveVarValBoolI;
        customObjectiveVarValInt = customObjectiveVarValIntI;

       // check if bool or an int
        if (customObjective.GetType().GetField(customObjectiveVar).GetValue(customObjective).GetType() == typeof(bool))
        {
            //print((bool)customObjective.GetType().GetField(customObjectiveVar).GetValue(customObjective));
            customObjectiveIsBool = true;
        }
        else
        {
            //print((int)customObjective.GetType().GetField(customObjectiveVar).GetValue(customObjective));
            customObjectiveIsBool = false;
        }
    }

    // separate start function so that it only checks during the step for performance reasons
    public void StartCustomObjective(I_Step stepScriptI)
    {
        stepScript = stepScriptI;
        StartCoroutine(CheckCustomObjective());
    }

    public void StartObjective(I_Step stepScriptI)
    {
        stepScript = stepScriptI;
    }

    IEnumerator CheckCustomObjective()
    {
        if (customObjectiveIsBool)
        {
            if ((bool)customObjective.GetType().GetField(customObjectiveVar).GetValue(customObjective) == customObjectiveVarValBool)
            {
                ObjectiveCompleted();
                yield break;
            }
        }
        else
        {
            if ((int)customObjective.GetType().GetField(customObjectiveVar).GetValue(customObjective) == customObjectiveVarValInt)
            {
                ObjectiveCompleted();
                yield break;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        //print(stepScript.gameObject.name + " is looking for " + customObjectiveVarValInt);
        StartCoroutine(CheckCustomObjective());
    }

    void ObjectiveCompleted()
    {
        stepScript.ObjectiveCompleted(this.gameObject);
        Destroy(this);
    }

    //void DoUsed(object sender, InteractableObjectEventArgs e)
    //{
    //    ObjectiveCompleted();
    //    Destroy(this);
    //}

    void DoGrabbed()
    {
        ObjectiveCompleted();
        Destroy(this);
    }

    void DoTouched()
    {
        ObjectiveCompleted();
    }

    //private void OnDisable()
    //{
    //    if (objectiveType == "use")
    //    {
    //        io.InteractableObjectUsed -= DoUsed;
    //    }
    //    else if (objectiveType == "grab")
    //    {
    //        io.InteractableObjectGrabbed -= DoGrabbed;
    //    }
    //    else if (objectiveType == "touch")
    //    {
    //        io.InteractableObjectTouched -= DoTouched;
    //    }
    //}
}
