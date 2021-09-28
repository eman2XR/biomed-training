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

    private void OnEnable()
    {
        print("answers " + evManager.correctAnswers);
        print("questions " + evManager.questionsList.childCount);
        float percentage = (((float)evManager.correctAnswers / (float)evManager.questionsList.childCount)) * 100;
        percentage = (int)percentage;
        scorePercentageText.text = percentage.ToString();

        if(percentage <= 50)
        {
            passText.SetActive(false);
            failText.SetActive(true);
        }
    }
}
