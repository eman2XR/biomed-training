//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleComment : MonoBehaviour {

	[Comment("Quickly add comments to your variables.", CommentType.Info)]
	public bool isCommented = true;
}
