//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

public enum VegetableType { Bulb, Grain, Fruit, Plant, Squash };

[System.Serializable]
public class Vegetable : System.Object
{
	public string name;

	public float plantHeight;
	public VegetableType vegetableType;
	public Texture2D image;

	[ComponentSelection]
	public Component component;
}

public class ExampleReorderableListCustomSerializableObject : MonoBehaviour {
	[Comment("Tired of dragging countless references all over again because you needed to insert an element in a List? Or worse, re-filling in every element manually because it was a list of custom serializable objects? With the ReorderableList attribute, you can move elements up and down and insert or delete new elements anywhere in the list, not just at the end.\n\nYou can even use ComponentSelection and NamedMonoBehaviour attributes in custom serializable objects inside your ReorderableList. Just add a flag to the constructor to have reorderable list calculate the right vertical spacing.", CommentType.Info)]
	[ReorderableList(true)]
	public List<Vegetable> vegetables;
}

