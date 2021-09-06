//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public interface IMyInterface
{
	void DoSomething();
}

public class ExampleInterfaceReference : MonoBehaviour {

	[Comment("Restrict references to an interface type.")]
	[InterfaceTypeRestriction(typeof(IMyInterface))]
	public MonoBehaviour implementsMyInterface;

	// Unity cannot serialize interfaces though, so you will have to save it as something Unity can serialize and cast it when you need to use it
	private IMyInterface ImplementsMyInterface
	{
		get
		{
			return (IMyInterface)implementsMyInterface;
		}
	}

	void Awake()
	{
		ImplementsMyInterface.DoSomething();
	}
}
