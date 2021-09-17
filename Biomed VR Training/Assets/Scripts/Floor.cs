using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SnapToOrigin>())
            StartCoroutine(Delay(other.GetComponent<SnapToOrigin>()));

        if(other.transform.parent)
         if (other.transform.parent.GetComponent<SnapToOrigin>())
                StartCoroutine(Delay(other.transform.parent.GetComponent<SnapToOrigin>()));
    }

    IEnumerator Delay(SnapToOrigin obj)
    {
        yield return new WaitForSeconds(1);
        obj.SnapToOriginalLocation();
    }
}
