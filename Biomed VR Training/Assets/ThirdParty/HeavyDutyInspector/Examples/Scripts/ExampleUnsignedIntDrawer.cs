//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using HeavyDutyInspector;

public class ExampleUnsignedIntDrawer : MonoBehaviour {

	[Comment("Unity's Inspector for a UInt64 displays ulong.MaxValue as being -1. This is obviously wrong, so I made a new Drawer for UInt64s that works properly.")]
	public UInt64 ulongExample = ulong.MaxValue; 
}
