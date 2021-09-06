using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class I_Step : MonoBehaviour {

    #region Variables
    public I_Master inductionMaster;
    List<GameObject> labels = new List<GameObject>();
    List<GameObject> objectsToHighlight = new List<GameObject>();

    //objectives
    public List<GameObject> objectives = new List<GameObject>();
    public List<string> objectiveTypes = new List<string>();
    int objectivesCompleted;
    int numberOfObjectives;
    public bool allAbjectivesCompleted;

    //steps variables
    int stepNumber;
    List<I_Master.Steps> steps = new List<I_Master.Steps>();

    public bool isCompulsory;
    public string stepName;
    #endregion

    private void Awake()
    {
        //references
        inductionMaster = this.GetComponentInParent<I_Master>();
    }

    public void SetupStep(int stepNumberI, List<I_Master.Steps> stepsI)
    {
        //copy over step number (the step in the list this step is)
        stepNumber = stepNumberI;

        //rename the step starting from 1 instead of 0
        int nr = stepNumber + 1;
        this.gameObject.name = "Step " + nr.ToString();

        //copy list steps for later use
        steps = stepsI;

        //assing each text from the inspector onto that Step opbject script text
        GetComponentInChildren<TextMeshProUGUI>().text = steps[stepNumber].text;

        //check if text needs to be reduced
        if (inductionMaster.otherSettings.reduceTextSize)
            GetComponentInChildren<TextMeshProUGUI>().fontSize = 0.06f;

        //assing step image and size 
        GetComponentInChildren<Image>().sprite = steps[stepNumber].image;
        if (steps[stepNumber].enlargeImage)
            GetComponentInChildren<Image>().gameObject.transform.localScale = new Vector3(2, 2, 2);

        //if no image then disable the Image GameObject
        if (steps[stepNumber].image == null)
            GetComponentInChildren<Image>().gameObject.SetActive(false);

        //add any animation
        if (steps[stepNumber].spriteAnimator)
        {
            ExtensionMethods.GetChild("Step Animation", transform).SetActive(true);
            ExtensionMethods.GetChild("Step Animation", transform).GetComponent<Animator>().runtimeAnimatorController = steps[stepNumber].spriteAnimator;
        }

        //copy labels list
        labels = steps[stepNumber].labels;

        //copy objectsToHighligh list
        objectsToHighlight = steps[stepNumber].objectsToHighlight;

        //copy list of objectives
        objectives = steps[stepNumber].objectives;

        //copy list of objectivesTypes
        objectives = steps[stepNumber].objectives;
        objectiveTypes = steps[stepNumber].objectiveTypes;

        //setup any audio
        if (steps[stepNumber].audio)
        {
            this.GetComponent<AudioSource>().clip = steps[stepNumber].audio;
        }

        //check if compulsory
        if (steps[stepNumber].isCompulsory)
            isCompulsory = true;

        //step name
        stepName = steps[stepNumber].stepName;
    }

    public void StartStep()
    {
        //inductionMaster.currentStep = inductionMaster.currentStep + 1;

        //run everyting with a slight delay to avoid conflicts 
        Run.After(0.7f, () => { 
        //activate any labels
        foreach (GameObject label in labels)
        {
            if(label)
            label.SetActive(true);
        }

        //setup objectives type
        foreach (GameObject objective in objectives)
        {
            objective.AddComponent<I_Objective>().SetupObjective(this, objectiveTypes[objectives.IndexOf(objective)]);

            //increase number of objectives needed to be completed
            numberOfObjectives++;
        }

        //setup custom objective
        if (steps[stepNumber].customObjective)
        {
            //increase number of objectives needed to be completed
            numberOfObjectives++;

            //add custom objective component and trigger SetpOobjective function
            steps[stepNumber].customObjective.gameObject.AddComponent<I_Objective>().SetupCustomObjective(this, steps[stepNumber].customObjective, steps[stepNumber].customObjectiveVariable, steps[stepNumber].customObjectiveVariableValueBool, steps[stepNumber].customObjectiveVariableValueInt);
            //print(this.gameObject.name + " " + steps[stepNumber].customObjectiveVariableValueInt); 
        }

        //start objective
        foreach (GameObject objective in objectives)
            objective.gameObject.GetComponent<I_Objective>().StartObjective(this);

        //start custom objective
        if (steps[stepNumber].customObjective)
        {
            steps[stepNumber].customObjective.gameObject.GetComponent<I_Objective>().StartCustomObjective(this);
        }

        //highlight objectives
        foreach(GameObject obj in steps[stepNumber].objectsToHighlight)
        //HighlightObject.instance.HighlightThisObject(obj);

        //debugs
        if(inductionMaster.otherSettings.printLogs)
        print(gameObject.name + " has " + numberOfObjectives + " objectives");
        });

        //play audio
        if (this.GetComponent<AudioSource>().clip)
            this.GetComponent<AudioSource>().Play();

        //hand notification ----------------------------------------------------------
        if (steps[stepNumber].handNotification)
        {
            if (inductionMaster.GetComponent<HandDirectionsAndHaptics>())
            {
                inductionMaster.GetComponent<HandDirectionsAndHaptics>().StartHandNotification(steps[stepNumber].targetForHandNotification);
            }
            else print("Hand notification selected, but no 'HandDirectionsAndHaptics' script found on the induction menu");
        }
    }

    public void StopStep()
    {
        //deactivate labels
        foreach (GameObject label in labels)
        {
            if (label)
                label.SetActive(false);
        }

        //stop all flashing
        //HighlightObject.instance.StopAllFlashing();

        //hand notification ----------------------------------------------------------
        if (steps[stepNumber].handNotification)
        {
            if (inductionMaster.GetComponent<HandDirectionsAndHaptics>())
            {
                inductionMaster.GetComponent<HandDirectionsAndHaptics>().EndHandNotification();
            }
        }

        //deactivate object
        this.gameObject.SetActive(false);
    }

    public void ObjectiveCompleted(GameObject objective)
    {
        objectivesCompleted++;
        //HighlightObject.instance.StopFlashing(objective);

        //disable any objective related labels [needs fix (this currently just disables the first label int the list)]
       // if (steps[stepNumber].labels.Count > 0)
            //steps[stepNumber].labels[0].gameObject.SetActive(false);

        //debugs
        if (inductionMaster.otherSettings.printLogs)
            print(objective.gameObject.name + " objective completed in step " + gameObject.name);

        if (objectivesCompleted == numberOfObjectives)
        {
            allAbjectivesCompleted = true;
            inductionMaster.StartNextStep();

            //sounds
            if (inductionMaster.otherSettings.stepCompletedSound)
                inductionMaster.otherSettings.stepCompletedSound.Play();


            //hand notification ----------------------------------------------------------
            if (steps[stepNumber].handNotification)
            {
                if (inductionMaster.GetComponent<HandDirectionsAndHaptics>())
                {
                    inductionMaster.GetComponent<HandDirectionsAndHaptics>().EndHandNotification();
                }
            }
        }
    }

}
