using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitMovement : MonoBehaviour
{
    Vector3 initialPos;

    private void Start()
    {
        initialPos = this.transform.position;
    }

    private void LateUpdate()
    {
        this.transform.position = initialPos;
    }
}
