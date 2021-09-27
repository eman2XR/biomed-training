using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager : MonoBehaviour
{
    public Transform questionsList;
    public int currentQuestion;

    public void StartEvaluation()
    {
        questionsList.GetChild(0).gameObject.SetActive(true);
    }

    public void AnswerGiven(GameObject answer)
    {
        questionsList.GetChild(currentQuestion).gameObject.SetActive(false);
        currentQuestion++;
        questionsList.GetChild(currentQuestion).gameObject.SetActive(true);
    }

}
