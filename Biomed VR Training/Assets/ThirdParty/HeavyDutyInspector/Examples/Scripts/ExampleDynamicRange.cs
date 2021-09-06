//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleDynamicRange : MonoBehaviour {

	public float minValue;
	public float maxValue;

	public float baseValue;
	public float multiplier;
	public float bonusValue;

	public float GetMaximum()
	{
		return baseValue * multiplier + bonusValue;
	}

    [Comment("Range between MinValue and 25")]
	[DynamicRange("minValue", 25f)]
	public float dynamicMin;

    [Comment("Range between 0 and MaxValue")]
    [DynamicRange(0, "maxValue")]
	public float dynamicMax;

    [Comment("Range between MinValue and MaxValue")]
    [DynamicRange("minValue", "maxValue")]
	public float dynamicBoth;

    [Comment("Range between 0 and BaseValue * Multiplier + BonusValue")]
    [DynamicRange(0, "GetMaximum")]
	public float usingFunction;
}
