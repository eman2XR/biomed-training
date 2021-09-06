//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

public class ExampleDictionary : NamedMonoBehaviour, ISerializationCallbackReceiver {

	[Dictionary("dictionaryExampleValues")]
	public List<string> dictionaryExample;
	[HideInInspector]
	public List<GameObject> dictionaryExampleValues;
#pragma warning disable 414
	private Dictionary<string, GameObject> actualDictionary = new Dictionary<string,GameObject>();
#pragma warning restore 414

	public void OnBeforeSerialize()
	{

	}

	public void OnAfterDeserialize()
	{
		actualDictionary = DictionaryHelper.InitDictionary<string, GameObject>(dictionaryExample, dictionaryExampleValues);
	}

}
