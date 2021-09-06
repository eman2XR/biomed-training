//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleComponentField : MonoBehaviour {

	[ComponentFieldRestriction(typeof(Texture), typeof(MeshRenderer))]
	public ComponentField textureToLocalize;
}
