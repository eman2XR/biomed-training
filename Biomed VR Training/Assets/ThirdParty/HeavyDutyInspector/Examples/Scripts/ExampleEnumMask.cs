//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

[ExecuteInEditMode]
public class ExampleEnumMask : MonoBehaviour {

    // You need to use bitshifting to assign your enum values for this to work properly.
    public enum Directions
    {
        Up = 1,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Front = 1 << 4,
        Back = 1 << 5
    }

    [Comment("Use the enum mask attribute to create a bitmask")]
    [EnumMask]
	public Directions validDirection;
}
