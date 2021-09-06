//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleLayer : MonoBehaviour {

	[Comment("Easily select a layer from Unity's layer popup and store it as an integer", CommentType.Info)]
	[Layer]
	public int affectedLayer;

}
