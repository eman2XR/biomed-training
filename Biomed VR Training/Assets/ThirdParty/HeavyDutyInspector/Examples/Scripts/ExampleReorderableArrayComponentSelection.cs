//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleReorderableArrayComponentSelection : MonoBehaviour {
	[Comment("When applied to a list of objects extending the Component class, reorderable list will display these references with the ComponentSelection drawer.", CommentType.Info)]
	[ReorderableArray]
	public FakeState[] states;
}

