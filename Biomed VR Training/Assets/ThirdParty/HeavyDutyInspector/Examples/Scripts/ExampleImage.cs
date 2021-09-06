//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 201  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleImage : MonoBehaviour {

	[Comment("Add images or logos in the Inspector", CommentType.Info, 0)] // When chaining DecoratorDrawers, the last one is displayed first
	// Your path can be relative to the Assets folder
	[Image("Illogika/HeavyDutyInspector/Examples/Textures/illogika-logo.png", Alignment.Center, 1)]
	// Or contain Assets/
	[Image("Assets/Illogika/HeavyDutyInspector/Examples/Textures/Resources/Textures/heavy_duty_logo.png", Alignment.Center, 2)]
	[HideVariable]
	public bool hidden;
}
