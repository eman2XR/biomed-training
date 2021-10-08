using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class E_Results : MonoBehaviour
{
    public EvaluationManager evManager;
    public TextMeshProUGUI scorePercentageText;
    public GameObject failText;
    public GameObject passText;

    public TextMeshProUGUI secondsText;
    public TextMeshProUGUI incorrectAnsText;
    public TextMeshProUGUI pointsText;

    public float score;
    public int scoreQuestions;
    public int skippedSteps;
    public string time;

    private void OnEnable()
    {
        //print("answers " + evManager.correctAnswers);
        //print("questions " + evManager.questionsList.childCount);
        float percentage = (((float)evManager.correctAnswers / (float)evManager.questionsList.childCount)) * 100;
        percentage = (int)percentage;
        scorePercentageText.text = percentage.ToString();

        if (percentage <= 50)
        {
            passText.SetActive(false);
            failText.SetActive(true);
        }

        float minutes = Mathf.Floor(evManager.timer / 60);
        float seconds = Mathf.RoundToInt(evManager.timer % 60);
        secondsText.text = minutes + ":" + seconds;

        incorrectAnsText.text = (evManager.questionsList.childCount - evManager.correctAnswers) + " incorrect answers";
        //pointsText.text = percentage/10 + " out of 10 points";
        pointsText.text = skippedSteps + " steps skipped";
        
        scoreQuestions = (int)percentage;
        time = secondsText.text;

        //calculate an overall score based on correct answes, skipped steps and time
        float timePenalty = evManager.timer/100;
        print(timePenalty);
        score = percentage - (Mathf.Clamp(skippedSteps*5, 0, 20) + Mathf.Clamp(timePenalty, 0, 20));
        print("score : " + score);
    }

    //triggered by the I_master
    public void StepSkipped()
    {
        skippedSteps++;
    }
}
