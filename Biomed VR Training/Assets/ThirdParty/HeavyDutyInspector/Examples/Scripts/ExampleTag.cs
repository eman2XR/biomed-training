//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleTag : MonoBehaviour {

	[Comment("Use Unity's tag popup box to select a tag and store it in a string. No more typo errors or having to remember tag names.", CommentType.Info)]
	[Tag]
	public string tagToFind;
}
