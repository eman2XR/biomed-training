//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleType : MonoBehaviour {

	[Comment("Drag a monoscript to this field to store its type. The object containing the type serializes the type name in a string, but casts itself implicitely to a System.Type using reflection.")]
	public SType typeFromScript;

	[Comment("Or restrict selection to types that inherit from a single base class.")]
	[TypeInspector(typeof(NamedMonoBehaviour))]
	public SType typeFromList;

}
