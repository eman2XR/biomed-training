using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SnapToOrigin>())
            other.GetComponent<SnapToOrigin>().SnapToOriginalLocation();

        if (other.transform.parent.GetComponent<SnapToOrigin>())
            other.transform.parent.GetComponent<SnapToOrigin>().SnapToOriginalLocation();
    }
}
