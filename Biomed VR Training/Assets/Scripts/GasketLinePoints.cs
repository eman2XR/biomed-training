using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GasketLinePoints : MonoBehaviour
{
    public Transform hookTarget;
    public bool hooked;
    public UnityEvent onHooked;

    private void OnTriggerEnter(Collider other)
    {
        if (!hooked)
        {
            if (other.tag == "hook" || other.transform.parent.tag == "hook")
             {
                transform.parent.GetComponent<Gasket>().GasketHooked();
                this.gameObject.GetComponent<Follow>().enabled = true;
                onHooked.Invoke();
                hooked = true;
                //this.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
