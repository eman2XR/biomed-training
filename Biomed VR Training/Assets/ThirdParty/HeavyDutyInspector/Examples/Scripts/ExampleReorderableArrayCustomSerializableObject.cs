//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleReorderableArrayCustomSerializableObject : MonoBehaviour {
	[Comment("Tired of dragging countless references all over again because you needed to insert an element in a List? Or worse, re-filling in every element manually because it was a list of custom serializable objects? With the ReorderableList attribute, you can move elements up and down and insert or delete new elements anywhere in the list, not just at the end.\n\nYou can even use ComponentSelection and NamedMonoBehaviour attributes in custom serializable objects inside your ReorderableList. Just add a flag to the constructor to have reorderable list calculate the right vertical spacing.", CommentType.Info)]
	[ReorderableArray(true)]
	public Vegetable[] vegetables;
}

