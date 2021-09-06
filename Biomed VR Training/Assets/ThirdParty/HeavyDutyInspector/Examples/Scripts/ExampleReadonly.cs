//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleReadonly : MonoBehaviour {

    [Comment("You can define variables that are visible in the inspector but whose value cannot be directly edited, protecting these values from unwanted changes. You can still change the value by switching the inspector to Debug mode.")]
	[Readonly]
	public int readonlyInt;

	[Readonly]
	public Color readonlyColor = Color.black;

	[Readonly]
	public AnimationCurve readonlyAnimCurve;

	[Readonly]
	public GameObject readonlyGameObject;

}
