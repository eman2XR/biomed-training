using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    public Transform questionsList;
    public GameObject endPanel;
    int currentQuestion;

    public int correctAnswers;
    public int timer;

    public I_Master instructionsM;

    public void StartEvaluation()
    {
        currentQuestion = 0;
        correctAnswers = 0;
        questionsList.GetChild(0).gameObject.SetActive(true);
        //StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(1);
        timer++;
        StartCoroutine(StartTimer());
    }

    public void AnswerGiven(E_Answer answer)
    {
        StartCoroutine(AnswerWasGiven(answer));
    }

    IEnumerator AnswerWasGiven(E_Answer answer)
    {
        if (answer.isCorrect)
        {
            correctAnswers++;
            answer.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);//show the tick mark
        }
        else
        {
            //get the correct answers and show the tick next to it
            foreach(E_Answer ans in answer.transform.parent.GetComponentsInChildren<E_Answer>())
                if(ans.isCorrect) ans.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
            answer.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);//show the X
        }

        yield return new WaitForSeconds(2);
        questionsList.GetChild(currentQuestion).gameObject.SetActive(false);
        currentQuestion++;

        if (currentQuestion < questionsList.childCount)
        {
            questionsList.GetChild(currentQuestion).gameObject.SetActive(true);
        }
        else
        {
            print("evaluation complete");
            endPanel.SetActive(true);
            StopAllCoroutines();
        }
    }

}
