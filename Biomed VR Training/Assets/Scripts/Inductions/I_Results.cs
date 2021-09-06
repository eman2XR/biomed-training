using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class I_Results : MonoBehaviour {

    public I_Master iMaster;
    public bool foundIcompleteCompulsoryStep;
    GameObject exampleStep;

    private void Start()
    {
        iMaster = this.GetComponentInParent<I_Master>();
        
        //deactivate next and back arrows
        ExtensionMethods.GetChild("Back", iMaster.transform).SetActive(false);
        ExtensionMethods.GetChild("Next", iMaster.transform).SetActive(false);

        //get the text from the pass result
        ExtensionMethods.GetChild("Results Text", transform).GetComponent<TextMeshProUGUI>().text = iMaster.results[0].text;

        //assign time
        string minutes = Mathf.Floor(iMaster.timer / 60).ToString("00");
        string seconds = (iMaster.timer % 60).ToString("00");
        ExtensionMethods.GetChild("Timer text", transform).GetComponent<TextMeshProUGUI>().text = minutes + ":" + seconds;

        //steps
        foreach (Transform child in ExtensionMethods.GetChild("Steps", transform.parent).transform)
        {
            if (child.GetComponent<I_Step>().isCompulsory)
            {
                exampleStep = ExtensionMethods.GetChild("Example Step", transform);
                if (child.GetComponent<I_Step>().allAbjectivesCompleted)
                {
                    GameObject step = Instantiate(exampleStep, ExtensionMethods.GetChild("Steps", transform).transform);
                    step.GetComponentInChildren<TextMeshProUGUI>().text = child.GetComponent<I_Step>().stepName + " :";
                    ExtensionMethods.GetChild("Checkmark fail", step.transform).SetActive(false);
                    ExtensionMethods.GetChild("Checkmark pass", step.transform).SetActive(true);
                }
                else
                {
                    //assign fail image
                    ExtensionMethods.GetChild("Results Image", transform).GetComponent<Image>().sprite = iMaster.results[1].image;

                    //
                    foundIcompleteCompulsoryStep = true;

                    //trigger event 
                    iMaster.inductionFailed.Invoke();

                    GameObject step = Instantiate(exampleStep, ExtensionMethods.GetChild("Steps", transform).transform);
                    step.GetComponentInChildren<TextMeshProUGUI>().text = child.GetComponent<I_Step>().stepName + " :";
                    ExtensionMethods.GetChild("Checkmark fail", step.transform).SetActive(true);
                    ExtensionMethods.GetChild("Checkmark pass", step.transform).SetActive(false);
                }
            }
        }

        //if not clicked on 'Results' button after 5 seconds show the results
        Run.After(3, () =>
        {
            if (ExtensionMethods.GetChild("Results Panel", transform).activeSelf == false)
                ViewMetrics();
        });
    }

    public void ViewMetrics()
    {
        ExtensionMethods.GetChild("Results Button", transform).SetActive(false);
        ExtensionMethods.GetChild("Main Menu Button", transform).SetActive(false);
        ExtensionMethods.GetChild("Results Panel", transform).SetActive(true);

        //deactivate the last child which is the example step
        ExtensionMethods.GetChild("Example Step", transform).SetActive(false);

        if (!foundIcompleteCompulsoryStep)
            iMaster.inductionPassed.Invoke();

        //deactivate the restart button and activate it a bit later so they don't click on it right away
        ExtensionMethods.GetChild("Restart Button", transform).SetActive(false);
        Run.After(3.5f, () =>
        {
            ExtensionMethods.GetChild("Restart Button", transform).SetActive(true);
            ExtensionMethods.GetChild("Main Menu Button", transform).SetActive(true);
        });
    }

    public void RestartInduction()
    {
        iMaster.RestartInduction();
        Destroy(this.gameObject);
    }

    public void MainMenu()
    {
        //open the main menu
       // FindObjectOfType<UI_LaserPointer>().OpenHomeMenu();
    }

    public void NextModule()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Welding 2");
    }
}
