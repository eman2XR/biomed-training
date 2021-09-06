//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

public class FakeInterfaceImplementation : MonoBehaviour, IMyInterface
{
	public void DoSomething()
	{
		Debug.Log("I called a referenced interface");
	}
}
