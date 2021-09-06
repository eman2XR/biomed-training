//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2015  Illogika
//----------------------------------------------

using UnityEngine;
using HeavyDutyInspector;

[System.Serializable]
public abstract class NamedMonoBehaviour : MonoBehaviour {

    [NMBName]
	public string	scriptName;

	[NMBColor]
	public Color	scriptNameColor = Color.white;
}
