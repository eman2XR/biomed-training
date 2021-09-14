using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Injector : MonoBehaviour
{
    //used by Induction master to track if step completed
    public bool isInverted;
    public bool isHorizontal;
    public bool backScrewsRemoved;

    int counter;

    void Update()
    {
        if(!isInverted)
        {
            if (this.transform.localEulerAngles.z == 170)
            {
                isInverted = true;
            }
        }

        if (isInverted && !isHorizontal)
        {
            if (this.transform.localEulerAngles.z < -90)
            {
                isHorizontal = true;
            }
        }
    }

    //triggered when screws are placed in the snap positions
    public void BackPanelScrewRemoved()
    {
        counter++;
        if (counter == 4)
            backScrewsRemoved = true;
    }
}
