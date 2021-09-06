//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

[System.Serializable]
public class SectionExample : System.Object
{
	public int aVariable;
	public string anotherVariable;
	public float yetAnotherVariable;
}

public class ExampleComplexHeaders : MonoBehaviour {

	[ComplexHeader("Organize your Scripts", Style.Box, Alignment.Left, ColorEnum.White, ColorEnum.Blue)]
	[Comment("Use the new Complex Header attribute to display headers in your inspector to separate and organize your variables.", CommentType.Info)]
	public string organize;

	[ComplexHeader("Organize your Scripts", Style.Line, Alignment.Center, ColorEnum.White, ColorEnum.White)]
	[Comment("Headers come in two styles, Box and Line", CommentType.Info, 1)]
	public string organizeAgain;

	[Background(ColorEnum.Blue)]
	[Comment("Or add a background to a Serialized Object variable and use it as a foldable Header", CommentType.Info)]
	public SectionExample foldableHeader;

}
