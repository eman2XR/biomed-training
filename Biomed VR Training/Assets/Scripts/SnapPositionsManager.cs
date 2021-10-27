using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPositionsManager : MonoBehaviour
{
    float counter;
    public List<SnapInPlace> screwsSnapPoints = new List<SnapInPlace>();

    public void SnapInAnyFreePostion(Transform trans)
    {
        SnapInPlace freeSnapPoint = null;

        foreach(SnapInPlace snapPoint in screwsSnapPoints)
        {
            if (snapPoint.gameObject.active && !snapPoint.snapped)
            {
                freeSnapPoint = snapPoint;
                trans.position = snapPoint.transform.position;
            }
        }

        if(freeSnapPoint)
            screwsSnapPoints.Remove(freeSnapPoint);
    }
}
