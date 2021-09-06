using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Injector : MonoBehaviour
{
    //used by Induction master to track if step completed
    public bool isInverted;

    void Update()
    {
        if(!isInverted)
        {
            if (this.transform.localEulerAngles.z == 170)
            {
                isInverted = true;
            }
        }
    }
}
