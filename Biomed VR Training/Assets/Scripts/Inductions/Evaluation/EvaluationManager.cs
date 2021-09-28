using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    public Transform questionsList;
    public GameObject endPanel;
    public int currentQuestion;

    public int correctAnswers;

    public void StartEvaluation()
    {
        questionsList.GetChild(0).gameObject.SetActive(true);
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
        }
    }

}
