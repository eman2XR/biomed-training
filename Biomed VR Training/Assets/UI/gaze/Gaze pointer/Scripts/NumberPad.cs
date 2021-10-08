using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NumberPad : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    bool keyPressed;

    public void KeyPressed(string key)
    {
        if (!keyPressed)//if it's the first time key was pressed, delete the placeholder text
        {
            inputText.text = key;
            keyPressed = true;
        }
        else
        {
            if (key == "del")
            {
                if (inputText.text != "")
                {
                    inputText.text = inputText.text.Substring(0, inputText.text.Length - 1);
                }
            }
            else
                inputText.text = inputText.text + key;
        }
    }
}
