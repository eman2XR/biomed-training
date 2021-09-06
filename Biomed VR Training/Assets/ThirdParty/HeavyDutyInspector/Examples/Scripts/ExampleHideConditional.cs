//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleHideConditional : MonoBehaviour {

	public enum TARGET_TYPE {
		Self,
		Position,
		Object
	}

	[Comment("Use the HiddenConditional attribute to hide some of your variables until a given boolean or enumeration condition is achieved.", CommentType.Info)]
	public bool hasTarget;

	[HideConditional(true, "hasTarget", "I have a hidden comment.", CommentType.Info)]
	public TARGET_TYPE targetType;

	[HideConditional(true, "targetType", (int)TARGET_TYPE.Position)]
	public Vector3 positionTarget;

	[HideConditional(true, "targetType", (int)TARGET_TYPE.Object)]
	public GameObject objectTarget;

	[HideConditional(true, "ObjectIsNotNull")]
	public bool isWorking = true;

	public bool ObjectIsNotNull()
	{
		return objectTarget != null;
	}
}
