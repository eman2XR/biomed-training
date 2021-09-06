using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensCap : MonoBehaviour
{
    public bool firstCapOpen;
    public bool secondCapOpen;

    public void CapOpened(int capNumber)
    {
        if (capNumber == 1)
            firstCapOpen = true;
        else if (capNumber == 2)
            secondCapOpen = true;
    }
}
