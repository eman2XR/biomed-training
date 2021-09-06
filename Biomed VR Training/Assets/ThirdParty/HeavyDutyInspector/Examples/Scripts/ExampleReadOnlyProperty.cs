//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleReadOnlyProperty : MonoBehaviour
{
    [Comment("Display Read Only Properties in the Inspector. Readonly Properties are displayed with a darker background to differentiate them from Readonly variables.")]
    [ReadonlyProperties(true, "ReadonlyInt", "ReadonlyVector", "ReadonlyCollider")]
    public bool hidden;

    public int ReadonlyInt
    {
        get
        {
            return 10;
        }
    }

    public Vector3 ReadonlyVector
    {
        get
        {
            return Vector3.one;
        }
    }

    public Collider ReadonlyCollider
    {
        get
        {
            return GetComponentInChildren<Collider>();
        }
    }
}
