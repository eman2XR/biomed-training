//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleButton : MonoBehaviour {

	[Comment("Add buttons and call functions by name using reflection", CommentType.Info)]
	[Button("Reset Position", "ResetPosition", true)]
	public bool hidden;

	[Comment("Or create buttons with an image by specifying its path.", CommentType.Info)]
	[ImageButton("Illogika/HeavyDutyInspector/Examples/Textures/illogika-logo.png", "ResetPosition", true)]
	public bool hidden2;

	void ResetPosition()
	{
		Debug.Log("ResetPosition");
		transform.position = Vector3.zero;
	}
}
