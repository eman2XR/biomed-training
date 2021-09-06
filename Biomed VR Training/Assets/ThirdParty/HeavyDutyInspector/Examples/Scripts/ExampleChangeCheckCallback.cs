//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleChangeCheckCallback : NamedMonoBehaviour {

	[Comment("Use the ChangeCheckCallback attribute to get notified when a variable changes. Try it and change the Target variable.", CommentType.Info)]
	[HideVariable]
	public bool comment;

	[ChangeCheckCallback("UpdateName")]
	public GameObject target;

	void UpdateName()
	{
		scriptName = "Waypoint (" + target.ToString() + ")";
	}
}
