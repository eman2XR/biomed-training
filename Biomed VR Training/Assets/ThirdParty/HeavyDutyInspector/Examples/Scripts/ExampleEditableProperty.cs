//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleEditableProperty : MonoBehaviour {

	[Comment("This variable displays the value returned by its getter and is modified through its setter. In this example the setter will ensure the value is positive and clamped between 0 and 100.", order = 0)]
	[Comment("If your accessor has the same name as the part of your private variable following the underscore, it will find its accessors without needing you to specify them.", order = 1)]
	[HideVariable]
	public bool comments;
	// In Unity 5.0.0 there seems to be a bug where adding decorator drawers to a property drawer that is forwarding drawing its variable to the default implementation causes the decorator drawer to appear twice.

	[EditableProperty][SerializeField]
	private int _myInt;

	public int myInt
	{
		get
		{
			return _myInt;
		}
		set
		{
			_myInt = Mathf.Clamp(Mathf.Abs(value), 0, 100);
		}
	}

}
