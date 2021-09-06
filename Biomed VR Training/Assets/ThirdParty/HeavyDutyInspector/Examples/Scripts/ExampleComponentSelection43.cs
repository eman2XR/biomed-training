//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleComponentSelection43 : MonoBehaviour {

	[Comment("Select components through a drop down menu that displays numbered components to easily identify which is which on overcharged GameObjects. Specify the name of a field belonging to this component to have its value displayed after the component's type and numbering, making it even easier to know which component you want to select.", CommentType.Info)]
	[ComponentSelection("clip")]
	public AudioSource footstepsAudioSource;

    [Comment("Use the ComponentSelection Attribute to choose which component you want to select on another GameObject without having to open a second inspector.", CommentType.Info)]
    [ComponentSelection]
    public FakeState idleState;

    [Comment("Use the ComponentSelection Attribute with arrays or lists.", CommentType.Info)]
	[ComponentSelection]
	public FakeStateAttack[] attackStates;
}
