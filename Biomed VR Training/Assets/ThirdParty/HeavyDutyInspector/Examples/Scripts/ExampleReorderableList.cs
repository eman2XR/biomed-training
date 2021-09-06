//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

public class ExampleReorderableList : MonoBehaviour {
	
	[Comment("Tired of dragging countless references all over again because you needed to insert an element in a List? Or worse, re-filling in every element manually because it was a list of custom serializable objects? With the ReorderableList attribute, you can move elements up and down and insert or delete new elements anywhere in the list, not just at the end.", CommentType.Info)]
	[ReorderableList]
	public List<string> vegetables;
}
