using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationButton : MonoBehaviour
{
    GameObject button;

    private void Start()
    {
        button = this.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            button.SetActive(true);
        }
        if (OVRInput.GetUp(OVRInput.RawButton.B))
        {
            button.SetActive(false);
        }
    }
}
