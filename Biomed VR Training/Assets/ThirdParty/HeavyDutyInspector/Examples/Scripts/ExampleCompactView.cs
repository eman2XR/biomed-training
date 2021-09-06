//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

[System.Serializable]
public class EventForKey : System.Object
{
	public List<KeyCode> keys;
	public List<string> events;

	public string name;
}

public class ExampleCompactView : MonoBehaviour {

	[Comment("Organize lists of serialized objects into something compact and a lot more redeable than Unity's default multiple levels of foldouts.")]
	[CompactView(HeaderStyle.VariableName, DisplayStyle.SideBySide, "keys", "events")]
	public List<EventForKey> eventsForKeys;

	[CompactView(HeaderStyle.FirstElement, DisplayStyle.SingleLineWithLabel, "name", "keys", "events")]
	public List<EventForKey> eventsForKeys2;

}
