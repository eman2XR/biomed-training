using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasketLinePoints : MonoBehaviour
{
    public Transform hookTarget;
    public bool hooked;

    private void OnTriggerEnter(Collider other)
    {
        if (!hooked)
        {
            if (other.tag == "hook" || other.transform.parent.tag == "hook")
             {
                transform.parent.GetComponent<Gasket>().GasketHooked();
                this.gameObject.AddComponent<Follow>().target = hookTarget;
                hooked = true;
                //this.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
