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
        if (answer.isCorrect) correctAnswers++;
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
