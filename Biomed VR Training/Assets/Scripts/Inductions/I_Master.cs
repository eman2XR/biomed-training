//=============================================================================
// Purpose: Master script that sends data futher down to its components 
//          I_Master -> I_Module -> I_Step -> I_Objective
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HeavyDutyInspector;
//using CurvedUI;
using TMPro;
//using VRTK;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.SceneManagement;

public class I_Master : MonoBehaviour {

    #region Variables
    [Comment("Master Script for creating inductions", CommentType.Warning)]
    public GameObject modulePrefab;
    public GameObject stepPrefab;
    public GameObject resultPrefab;

    public string inductionTitle;
    [TextArea]
    public string inductionDescription;

    [Header("Module 1")]
    public Sprite moduleOneIcon;
    public string moduleOneDescription;

    //Steps list
    [ReorderableList]
    public List<Steps> steps;
    [System.Serializable]
    public class Steps
    {
        public string stepName = "Step Name";
        public bool isCompulsory = false;
        [TextArea]
        public string text = "Step Description";
        public Sprite image;
        public bool enlargeImage;
        public RuntimeAnimatorController spriteAnimator;
        public List <GameObject> objectives = new List<GameObject>();
        public List<string> objectiveTypes = new List<string>();

        [Tooltip("tick this if you want the step to move to the next one after a few second/ no use input required")]
        public bool switchesAutomatically;
        [HideConditional(true, "switchesAutomatically")]
        public int switchDelay;

        public bool hasCustomObjective;

        public enum VariableType
        {
            Int,
            Bool,
        }

        [HideConditional(true, "hasCustomObjective", "Select the script that has the custom objective you want to use", CommentType.None)]
        [ComponentSelection]
        public Component customObjective;

        [HideConditional(true, "hasCustomObjective")]
        public string customObjectiveVariable;

        [HideConditional(true, "hasCustomObjective")]
        public VariableType variableType;

        [HideConditional(true, "variableType", (int)VariableType.Bool)]
        public bool customObjectiveVariableValueBool;

        [HideConditional(true, "variableType", (int)VariableType.Int)]
        public int customObjectiveVariableValueInt;

        public List<GameObject> objectsToHighlight = new List<GameObject>();
        public List<GameObject> labels = new List<GameObject>();
       
        public AudioClip audio;

        [Tooltip("tick this if you want the right hand to receive a notification and pointer to the target")]
        public bool handNotification;
        [HideConditional(true, "handNotification")]
        public Transform targetForHandNotification;

        public int eventDelay = 0;
        public UnityEvent triggerEvent;

        [Header("Skip step events")]
        [Tooltip("any objects that should be moved on step skipped")]
        public List<Transform> objectsToMove = new List<Transform>();
        public List<Transform> objectsToMoveTargets = new List<Transform>();
        public UnityEvent skipStepEvent;
    }

    //Steps list
    [ReorderableList]
    public List<Results> results;
    [System.Serializable]
    public class Results
    {
        public string resultName = "Result Name";
        //public bool isCompulsory = false;
        [TextArea]
        public string text = "Result Text";
        public Sprite image;
        public UnityEvent triggerEvent;
    }

    //events on pass and fail
    public UnityEvent inductionPassed;
    public UnityEvent inductionFailed;

    //references
    GameObject module;
    GameObject step;
    GameObject introPanel;
    GameObject modulesPanel;

    //testing buttons
    [Space(20)]
    [Button("Start Induction", "StartInduction", true)]
    public bool hidden;
    [Button("Start Module", "StartModule", true)]
    public bool hidden1;
    [Button("Skip Step", "SkipStep", true)]
    public bool hidden2;
    [Button("Start Previous Step", "StartPreviousStep", true)]
    public bool hidden3;
    [Button("Restart Induction", "RestartInduction", true)]
    public bool hidden4;

    //Ui buttons variables
    GameObject backButton;
    GameObject nextButton;
    GameObject startButton;
    GameObject stepsCounterButton;

    //track number of steps completed
    //[HideInInspector]
    public int currentStep;
    public float timer;
    bool runTimer = true;

    //other variables
    [System.Serializable]
    public class OtherSettings : System.Object
    {
        public bool printLogs;
        public AudioSource stepCompletedSound;
        public AudioSource moduleCompletedSound;
        public AudioSource inductionCompletedSound;
        public AudioSource changeStepSound;

        //extra module [needs fix]
        [ReorderableList]
        public List<ExtraModules> extraModules;
        [System.Serializable]
        public class ExtraModules
        {
            [TextArea]
            public string text;
            public Sprite image;
        }

        public GameObject extraModulePrefab;
        public string sceneToStartOnExtraModule; //for now, for any extra module, we start a scene

        public bool inductionCompleted;
        public string sceneName;

        public bool reduceTextSize;

        public bool skipToStart;//to skip to the first step in the induction

        public E_Results results;
        public EvaluationManager evManager;

    }
    [Background(ColorEnum.Grey)]
    public OtherSettings otherSettings;
#endregion

    private void Awake()
    {
        //get refferences-----------------------------------
        introPanel = ExtensionMethods.GetChild("Intro Panel", transform);
        modulesPanel = ExtensionMethods.GetChild("Modules Panel", transform);

        //ui button refferences
        backButton = ExtensionMethods.GetChild("Back", transform);
        nextButton = ExtensionMethods.GetChild("Next", transform);
        startButton = ExtensionMethods.GetChild("Start", transform);
        stepsCounterButton = ExtensionMethods.GetChild("Steps Counter", transform);
        //--------------------------------------------------

        //activate/deactivate the right objects-------------
        introPanel.SetActive(true);
        modulesPanel.SetActive(false);
        ExtensionMethods.GetChild("Induction Title", transform).SetActive(true);
        ExtensionMethods.GetChild("Induction Description", transform).SetActive(true);

        //ui buttons
        backButton.SetActive(false);
        nextButton.SetActive(false);
        startButton.SetActive(true);
        //--------------------------------------------------

        //[needs fix] deactivate the canvas for the CurvedUI components otherwise error
        this.transform.GetChild(0).gameObject.SetActive(false);
        
    }

    IEnumerator Start()
    {
        ExtensionMethods.GetChild("Induction Title", transform).GetComponent<TextMeshProUGUI>().text = inductionTitle;
        ExtensionMethods.GetChild("Induction Description", transform).GetComponent<TextMeshProUGUI>().text = inductionDescription;

        //instantiate 1 module
        module = Instantiate(modulePrefab, ExtensionMethods.GetChild("Modules List", transform).transform);

        //assing the module icon
        ExtensionMethods.GetChild("Module Icon", module.transform).GetComponent<Image>().sprite = moduleOneIcon;

        //assing the module text
        ExtensionMethods.GetChild("Module Description", module.transform).GetComponent<TextMeshProUGUI>().text = moduleOneDescription;

        //[needs fix] activate the canvas for the CurvedUI components otherwise error
        yield return new WaitForSeconds(1);
        this.transform.GetChild(0).gameObject.SetActive(true);

        //check if we want to skip to the first step in the induction
        if (otherSettings.skipToStart) { StartInduction(); StartModule(false); };

        //string text = "";
        //foreach(Steps step in steps)
        //    text += "\n" + "\n" + step.stepName + "\n" + step.text;
        //print(text);
    }

    public void StartInduction()
    {
        //activate modules panel
        modulesPanel.SetActive(true);

        //deactivate intro panel
        introPanel.SetActive(false);

        StartModule(false);

        //deactivate the start Ui button
        startButton.SetActive(false);

        //instantiate any extra modules
        for(int i = 0; i < otherSettings.extraModules.Count; i++)
        {
            GameObject extraModule = Instantiate(otherSettings.extraModulePrefab, ExtensionMethods.GetChild("Modules List", modulesPanel.transform).transform);

            //add text and description
            ExtensionMethods.GetChild("Module Description", extraModule.transform).GetComponent<TextMeshProUGUI>().text = otherSettings.extraModules[i].text;
            ExtensionMethods.GetChild("Module Icon", extraModule.transform).GetComponent<Image>().sprite = otherSettings.extraModules[i].image;

            //lock the module
            //ExtensionMethods.GetChild("Module Icon", extraModule.transform).GetComponent<Button>().enabled = false;
            //ExtensionMethods.GetChild("locked icon", extraModule.transform).gameObject.SetActive(true);
        }
    }

    public void StartModule(bool isExtraModule)
    {
        if (isExtraModule)
        {
            SceneManager.LoadScene(otherSettings.sceneToStartOnExtraModule);
            return;
        }

        //start timer (for checking how long the induction tool to complete)
        //Coroutine timerCo;
        //timerCo = StartCoroutine(Timer());
        StartCoroutine(otherSettings.evManager.StartTimer());

        //deactivate modules panel title
        ExtensionMethods.GetChild("Modules Panel Title", transform).SetActive(false);

        //start the module script (only does 1 module)
        module.GetComponent<I_Module>().StartModule(stepPrefab, steps);

        //activate next/back arrows
        nextButton.SetActive(true);
        backButton.SetActive(true);

        //check for events
        //print("invoking event");
        steps[currentStep].triggerEvent.Invoke();

        //steps counter ui
        stepsCounterButton.SetActive(true);
        stepsCounterButton.GetComponent<TextMeshProUGUI>().text = "1/" + (steps.Count - 1);

        //check for automatic step switching
        if (steps[currentStep].switchesAutomatically)
        {
            //mark objectives as completed
            foreach (I_Step step in this.GetComponentsInChildren<I_Step>())
            {
                step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().allAbjectivesCompleted = true;
            }

            int curStep = currentStep;

            Run.After(steps[currentStep].switchDelay, () =>
            {
                //if it's still the same step then switch (in case the user clicked the next arrow)
                if (curStep == currentStep)
                {
                    print("switched step automatically");
                    StartNextStep();
                }
            });
        }
    }

    public void StartNextStep()
    {
        StopAllCoroutines();

        currentStep++;

        if(currentStep == steps.Count)
        {
            print("Induction Finished");
            EndOfInduction();
            return;
        }

        //check for automatic step switching
        if (steps[currentStep].switchesAutomatically)
        {
            //mark objectives as completed
            foreach (I_Step step in this.GetComponentsInChildren<I_Step>())
            {
                step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().allAbjectivesCompleted = true;
            }

            int curStep = currentStep;

            Run.After(steps[currentStep].switchDelay, () =>
            {
                //if it's still the same step then switch (in case the user clicked the next arrow)
                if (curStep == currentStep)
                {
                    print("switched step automat");
                    StartNextStep();
                }
            });
        }

        //deactivate and activate next and back buttons to avoid pressing it twice in succession
        nextButton.SetActive(false);
        backButton.SetActive(false);
        Run.After(1, () => 
        {
            nextButton.SetActive(true);
            backButton.SetActive(true);
        });

        //steps counter ui
        stepsCounterButton.GetComponent<TextMeshProUGUI>().text = currentStep + 1 + "/" + (steps.Count - 1);

        //sound fx
        otherSettings.changeStepSound.Play();
        
        //debugs
        if (otherSettings.printLogs)
            print("starting next step");

        //send message to module to start next step
          module.GetComponent<I_Module>().StartNextStep();

        //check for events
        Run.After(steps[currentStep].eventDelay, () =>
        {
            steps[currentStep].triggerEvent.Invoke();
        });  
    }

    public void SkipStep()
    {
        StopAllCoroutines();

        //tell the results script a step was skipped
        otherSettings.results.StepSkipped();

        //destroy custom objective if still active
        foreach (I_Step step in this.GetComponentsInChildren<I_Step>())
        {
            if (step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().customObjective)
                Destroy(step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().customObjective); //destroy custom objective
        }

        //move any objects
        foreach (Transform trans in steps[currentStep].objectsToMove)
        {
            if (!trans.name.Contains("INJECTOR"))
                trans.parent = null;
            trans.position = steps[currentStep].objectsToMoveTargets[steps[currentStep].objectsToMove.IndexOf(trans)].position;
            trans.rotation = steps[currentStep].objectsToMoveTargets[steps[currentStep].objectsToMove.IndexOf(trans)].rotation;
        }

        steps[currentStep].skipStepEvent.Invoke();

        currentStep++;

        if (currentStep == steps.Count)
        {
            print("Induction Finished");
            EndOfInduction();
            return;
        }

        //check for automatic step switching
        if (steps[currentStep].switchesAutomatically)
        {
            //mark objectives as completed
            foreach (I_Step step in this.GetComponentsInChildren<I_Step>())
            {
                step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().allAbjectivesCompleted = true;
            }

            int curStep = currentStep;

            Run.After(steps[currentStep].switchDelay, () =>
            {
                //if it's still the same step then switch (in case the user clicked the next arrow)
                if (curStep == currentStep)
                {
                    //print("switched step automat");
                    StartNextStep();
                }
            });
        }

        //deactivate and activate next and back buttons to avoid pressing it twice in succession
        nextButton.SetActive(false);
        backButton.SetActive(false);
        Run.After(1, () =>
        {
            nextButton.SetActive(true);
            backButton.SetActive(true);
        });

        //steps counter ui
        stepsCounterButton.GetComponent<TextMeshProUGUI>().text = currentStep + 1 + "/" + (steps.Count - 1);

        //sound fx
        otherSettings.changeStepSound.Play();

        //debugs
        if (otherSettings.printLogs)
            print("starting next step");

        //send message to module to start next step
        module.GetComponent<I_Module>().StartNextStep();

        //check for events
        Run.After(steps[currentStep].eventDelay, () =>
        {
            steps[currentStep].triggerEvent.Invoke();
        });
    }

    public void StartStep(int stepToStart)
    {
        StopAllCoroutines();

        currentStep = stepToStart;

        if (currentStep == steps.Count)
        {
            print("Induction Finished");
            EndOfInduction();
            return;
        }

        //check for automatic step switching
        if (steps[currentStep].switchesAutomatically)
        {
            //mark objectives as completed
            foreach (I_Step step in this.GetComponentsInChildren<I_Step>())
            {
                step.transform.parent.GetChild(currentStep).GetComponent<I_Step>().allAbjectivesCompleted = true;
            }

            int curStep = currentStep;

            Run.After(steps[currentStep].switchDelay, () =>
            {
                //if it's still the same step then switch (in case the user clicked the next arrow)
                if (curStep == currentStep)
                {
                    print("switched step automat");
                    StartNextStep();
                }
            });
        }

        //deactivate and activate next and back buttons to avoid pressing it twice in succession
        nextButton.SetActive(false);
        backButton.SetActive(false);
        Run.After(1, () =>
        {
            nextButton.SetActive(true);
            backButton.SetActive(true);
        });

        //steps counter ui
        stepsCounterButton.GetComponent<TextMeshProUGUI>().text = currentStep + 1 + "/" + (steps.Count - 1);

        //sound fx
        otherSettings.changeStepSound.Play();

        //debugs
        if (otherSettings.printLogs)
            print("starting step " + currentStep);

        //send message to module to start the step
        module.GetComponent<I_Module>().StartStep(stepToStart);

        //check for events
        Run.After(steps[currentStep].eventDelay, () =>
        {
            steps[currentStep].triggerEvent.Invoke();
        });
    }

    public void EndOfInduction()
    {
        //end timer
        runTimer = false;

        otherSettings.inductionCompletedSound.Play();
        otherSettings.inductionCompleted = true;

        //steps counter ui
        stepsCounterButton.SetActive(false);

        //mark the last step (pass screen) as complete
        // GetComponentInChildren<I_Step>().transform.parent.GetChild(currentStep).GetComponent<I_Step>().allAbjectivesCompleted = //true;

        //disable the last step
        Run.After(0.5f, () => {
            GetComponentInChildren<I_Step>().transform.parent.GetChild(currentStep -1).gameObject.SetActive(false);
            Instantiate(resultPrefab, this.GetComponentInChildren<I_Module>().transform);
        });

        if (otherSettings.printLogs)
        {
            print("Induction completed");
        }
    }

    public void StartPreviousStep()
    {
        StopAllCoroutines();

        currentStep--;

        //steps counter ui
        stepsCounterButton.GetComponent<TextMeshProUGUI>().text = currentStep + 1 + "/" + (steps.Count - 1);

        //check for automatic step switching
        if (steps[currentStep].switchesAutomatically)
        {
            Run.After(steps[currentStep].switchDelay, () => { StartNextStep(); });
        }

        if (otherSettings.printLogs)
            print("starting previous step");

        module.GetComponent<I_Module>().StartPreviousStep();

        //check for events
        steps[currentStep].triggerEvent.Invoke();
    }

    public void RestartInduction()
    {
        //check if scene is one of the scenes included in the main app
        if (otherSettings.sceneName == "Home Setup" || otherSettings.sceneName == "Main Workshop" || otherSettings.sceneName == "Chop Saw")
        {
            //fade headset to black
            //if (GameObject.FindObjectOfType<VRTK.VRTK_HeadsetFade>()) GameObject.FindObjectOfType<VRTK.VRTK_HeadsetFade>().Fade(Color.black, 2);
            //Run.After(2, () =>
            //{
                SceneManager.LoadScene(otherSettings.sceneName);
            //});
        }

        //otherwise check if it's in the asset bundles
        //else
        //{
            //if (GameObject.FindObjectOfType<VRTK.VRTK_HeadsetFade>()) GameObject.FindObjectOfType<VRTK.VRTK_HeadsetFade>().Fade(Color.black, 2);
            //Run.After(2, () =>
            //{
                //print(otherSettings.sceneName);
                //the asset bundle loads from the Home Setup so if this is in the editor starting from the scene itself it won't work
                //SceneManager.LoadScene(AssetBundleLoader.instance.scenePaths[AssetBundleLoader.instance.assetBundlesNames.IndexOf(otherSettings.sceneName)]);
            //});
        //}
    }

    IEnumerator Timer()
    {
        while (runTimer)
        {
            yield return new WaitForSeconds(1);
            timer += 1;
        }
        yield return null;
    }

    #region Button Functions
    public void StartButtonPressed()
    {
        StartInduction();
    }

    public void NextButtonPressed()
    {
        StartNextStep();
    }

    public void BackButtonPressed()
    {
        StartPreviousStep();
    }
    #endregion

}

//[CustomEditor(typeof(I_Master))]
//public class I_MasterEditor : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        EditorGUILayout.HelpBox("Master script for Machine Induction.", MessageType.Info);

//        //I_Master myScript = target as I_Master;

//        ////------------------------------------------------------------------------------------------------------
//        //switch (myScript.numberOfModules)
//        //{
//        //    case I_Master.NumberOfMudules.one:
//        //        myScript.numberOfStepsM1 = EditorGUILayout.IntField("numberOfStepsM1", myScript.numberOfStepsM1);
//        //        break;

//        //    case I_Master.NumberOfMudules.two:
//        //        myScript.numberOfStepsM1 = EditorGUILayout.IntField("numberOfStepsM1", myScript.numberOfStepsM1);
//        //        myScript.numberOfStepsM2 = EditorGUILayout.IntField("numberOfStepsM2", myScript.numberOfStepsM2);
//        //        break;
//        //    case I_Master.NumberOfMudules.three:
//        //        myScript.numberOfStepsM1 = EditorGUILayout.IntField("numberOfStepsM1", myScript.numberOfStepsM1);
//        //        myScript.numberOfStepsM2 = EditorGUILayout.IntField("numberOfStepsM2", myScript.numberOfStepsM2);
//        //        myScript.numberOfStepsM3 = EditorGUILayout.IntField("numberOfStepsM3", myScript.numberOfStepsM3);
//        //        break;
//        //}
//        ////-------------------------------------------------------------------------------------------------------

//        //if(myScript.numberOfStepsM1 > 0)
//        //    myScript.objectivesM1Step1 = EditorGUILayout.IntField("objectivesM1Step1", myScript.objectivesM1Step1);
//        //else if(myScript.numberOfStepsM1 == 2)
//        //    myScript.objectivesM1Step1 = EditorGUILayout.IntField("objectivesM1Step1", myScript.objectivesM1Step1);
//        //myScript.objectivesM1Step2 = EditorGUILayout.IntField("objectivesM1Step2", myScript.objectivesM1Step2);

//        //switch (myScript.myType)
//        //{
//        //    case I_Master.MyTypes.TypeOne:
//        //        myScript.Height = EditorGUILayout.IntField("Height", myScript.Height);
//        //        break;

//        //    case I_Master.MyTypes.TypeTwo:
//        //        myScript.Width = EditorGUILayout.IntField("Width", myScript.Width);
//        //        break;
//        //}

//        //myScript.myBool = EditorGUILayout.Toggle("MyBool", myScript.myBool);
//        ////Create an input field for the int only if the bool field is true.
//        //if (myScript.myBool)
//        //{
//        //    myScript.myInt = EditorGUILayout.IntField("MyInt", myScript.myInt);
//        //}

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(target);
//        }

//        GUILayout.Space(10);
//        base.OnInspectorGUI();
//    }
//}
