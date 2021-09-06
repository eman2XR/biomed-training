//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

public class ExampleTagList : MonoBehaviour {
	[Comment("Use Unity's tag popup box to select tags in a list of strings. Allows you to delete a tag from anywhere in the list.", CommentType.Info)]
	[TagList]
	public List<string> tagsToFind;
}	
