//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleScene : MonoBehaviour {

	[Comment("Chose a scene from all the scenes in your project, sorted by folder.", CommentType.Info)]
	public Scene myScene;

	[Comment("Or specify a folder where you want to start searching for scenes.", CommentType.Info)]
	[Scene("Illogika/HeavyDutyInspector/Examples/Scenes")]
	public Scene sortedScenes;

}
