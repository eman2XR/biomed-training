using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GasketLinePoints : MonoBehaviour
{
    public Transform hookTarget;
    public bool hooked;
    public UnityEvent onHooked;
    public GasketLinePoints hookPoint;
    public bool isHookPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!hooked)
        {
            if( other.transform.parent)
            {
                if (other.transform.parent.tag == "hook" && !other.transform.parent.GetComponent<HookTool>().hasGasket)
                {
                    if (!hooked)
                    {
                        other.transform.parent.GetComponent<HookTool>().hasGasket = true;

                        if (isHookPoint)
                        {
                            transform.parent.GetComponent<Gasket>().GasketHooked();
                            this.gameObject.GetComponent<Follow>().enabled = true;
                            onHooked.Invoke();
                            hooked = true;
                        }
                        else
                            hookPoint.IsBeingHooked();
                    }
                } }
        }
    }

    public void IsBeingHooked()
    {
        if (!hooked)
        {
            transform.parent.GetComponent<Gasket>().GasketHooked();
            this.gameObject.GetComponent<Follow>().enabled = true;
            onHooked.Invoke();
            hooked = true;
        }
    }
}
