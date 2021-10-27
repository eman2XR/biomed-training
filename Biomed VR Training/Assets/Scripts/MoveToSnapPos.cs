using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToSnapPos : MonoBehaviour
{
    public void FindTargetSnapPos()
    {
        if(!this.GetComponent<OVRGrabbable>().snapped)
            GameObject.FindObjectOfType<SnapPositionsManager>().SnapInAnyFreePostion(this.transform);
    }
}
